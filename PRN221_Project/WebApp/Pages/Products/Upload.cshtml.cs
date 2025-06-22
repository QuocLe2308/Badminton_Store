using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

public class UploadModel : PageModel
{
    private readonly ImageUploadService _imageUploadService;

    public UploadModel(ImageUploadService imageUploadService)
    {
        _imageUploadService = imageUploadService;
    }

    [BindProperty]
    public IFormFile File { get; set; }

    public UploadResult UploadResult { get; set; }

    public async Task<IActionResult> OnPostAsync()
    {
        if (File == null || File.Length == 0)
        {
            ModelState.AddModelError(string.Empty, "Please select a valid file.");
            return Page();
        }

        try
        {
            var fileName = await _imageUploadService.UploadImageAsync(File);
            var fileUrl = Url.Content($"~/uploads/{fileName}");
            UploadResult = new UploadResult
            {
                Url = fileUrl
            };
            return Page();
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, $"Internal server error: {ex.Message}");
            return Page();
        }
    }
}

public class UploadResult
{
    public string Url { get; set; }
}
