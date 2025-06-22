using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

public class ImageUploadService
{
    private readonly ImageUploadSettings _imageUploadSettings;

    public ImageUploadService(IOptions<ImageUploadSettings> imageUploadSettings)
    {
        _imageUploadSettings = imageUploadSettings.Value;

        if (string.IsNullOrEmpty(_imageUploadSettings.UploadPath))
            throw new ArgumentNullException(nameof(_imageUploadSettings.UploadPath));
        if (_imageUploadSettings.AllowedExtensions == null || _imageUploadSettings.AllowedExtensions.Count == 0)
            throw new ArgumentNullException(nameof(_imageUploadSettings.AllowedExtensions));
        if (_imageUploadSettings.MaxFileSize <= 0)
            throw new ArgumentOutOfRangeException(nameof(_imageUploadSettings.MaxFileSize));
    }

    public async Task<string> UploadImageAsync(IFormFile file)
    {
        if (file == null)
            throw new ArgumentNullException(nameof(file));

        string fileExtension = Path.GetExtension(file.FileName).ToLower();
        if (!_imageUploadSettings.AllowedExtensions.Contains(fileExtension))
            throw new InvalidOperationException("File type is not allowed.");

        if (file.Length > _imageUploadSettings.MaxFileSize)
            throw new InvalidOperationException("File size exceeds the maximum limit.");

        string fileName = Guid.NewGuid().ToString() + fileExtension;
        string fullPath = Path.Combine(_imageUploadSettings.UploadPath, fileName);

        try
        {
            Directory.CreateDirectory(_imageUploadSettings.UploadPath);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return fileName;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error uploading image: {ex.Message}");
        }
    }
}
