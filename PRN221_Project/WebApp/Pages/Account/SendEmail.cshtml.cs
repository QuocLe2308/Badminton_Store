using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

public class SendEmailModel : PageModel
{
    private readonly ILogger<SendEmailModel> _logger;
    private readonly EmailService _emailService;

    public SendEmailModel(ILogger<SendEmailModel> logger, EmailService emailService)
    {
        _logger = logger;
        _emailService = emailService;
    }

    [BindProperty]
    public string ToEmail { get; set; }

    [BindProperty]
    public string Subject { get; set; }

    [BindProperty]
    public string Body { get; set; }

    public async Task<IActionResult> OnPostAsync()
    {
        try
        {
            await _emailService.SendEmailAsync(ToEmail, Subject, Body);
            TempData["Message"] = "Email sent successfully!";
        }
        catch (Exception ex)
        {
            _logger.LogError($"Failed to send email: {ex.Message}");
            TempData["Error"] = "Failed to send email. Please try again later.";
        }

        return RedirectToPage();
    }
}
