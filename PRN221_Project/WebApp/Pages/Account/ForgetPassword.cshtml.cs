using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using DataAccessLayer.DbContext;
using DataAccessLayer.Models;

namespace WebApp.Pages.Account
{
    public class ForgetPasswordModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly EmailService _emailService;

        [BindProperty]
        public ForgetPasswordInputModel Input { get; set; }

        public ForgetPasswordModel(ApplicationDbContext context, EmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostSendOTPAsync()
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == Input.Email);
                if (user == null)
                {
                    return new JsonResult(new { success = false, message = "Email not found." });
                }

                var otp = GenerateOTP();
                HttpContext.Session.SetString("ResetOTP", otp);
                HttpContext.Session.SetString("ResetEmail", Input.Email);

                await _emailService.SendEmailAsync(Input.Email, "OTP for Password Reset", $"Your OTP is: {otp}");

                return new JsonResult(new { success = true, message = "OTP sent successfully." });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = $"Error sending OTP: {ex.Message}" });
            }
        }

        public async Task<IActionResult> OnPostConfirmOTPAsync()
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var savedOTP = HttpContext.Session.GetString("ResetOTP");
                    var email = HttpContext.Session.GetString("ResetEmail");

                    if (savedOTP == Input.OTP && email == Input.Email)
                    {
                        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
                        if (user != null)
                        {
                            user.Password = Input.NewPassword;
                            _context.Users.Update(user);
                            await _context.SaveChangesAsync();

                            return new JsonResult(new { success = true, message = "Password reset successfully." });
                        }
                        else
                        {
                            return new JsonResult(new { success = false, message = "User not found." });
                        }
                    }
                    else
                    {
                        return new JsonResult(new { success = false, message = "Invalid OTP or email." });
                    }
                }
                catch (Exception ex)
                {
                    return new JsonResult(new { success = false, message = $"Error confirming OTP: {ex.Message}" });
                }
            }

            return new JsonResult(new { success = false, message = "Invalid model state. Please check your inputs." });
        }

        private string GenerateOTP()
        {
            Random rand = new Random();
            return rand.Next(100000, 999999).ToString();
        }
    }

    public class ForgetPasswordInputModel
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; }

        public string OTP { get; set; }

        [Required(ErrorMessage = "New Password is required")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Confirm New Password is required")]
        [Compare("NewPassword", ErrorMessage = "Passwords do not match")]
        [DataType(DataType.Password)]
        public string ConfirmNewPassword { get; set; }
    }
}
