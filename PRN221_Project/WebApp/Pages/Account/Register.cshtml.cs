using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using DataAccessLayer.DbContext;
using DataAccessLayer.Models;

namespace WebApp.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly EmailService _emailService;

        [BindProperty]
        public RegisterInputModel Input { get; set; }

        public RegisterModel(ApplicationDbContext context, EmailService emailService)
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
                // Check if email or username already exists
                var emailExists = await _context.Users.AnyAsync(u => u.Email == Input.Email);
                var usernameExists = await _context.Users.AnyAsync(u => u.Username == Input.Username);

                if (emailExists)
                {
                    return new JsonResult(new { success = false, message = "Email already exists. Please enter a different email." });
                }

                if (usernameExists)
                {
                    return new JsonResult(new { success = false, message = "Username already exists. Please enter a different username." });
                }

                // Generate OTP
                var otp = GenerateOTP();

                // Save OTP in session
                HttpContext.Session.SetString("RegisterOTP", otp);

                // Send OTP via email
                await _emailService.SendEmailAsync(Input.Email, "OTP for Registration", $"Your OTP is: {otp}");

                // Hide registration form and show OTP form
                ViewData["ShowOTPForm"] = true;

                // Return success response as JSON
                return new JsonResult(new { success = true, message = "OTP sent successfully" });
            }
            catch (Exception ex)
            {
                // Return error response as JSON
                return new JsonResult(new { success = false, message = $"Error sending OTP: {ex.Message}" });
            }
        }

        public async Task<IActionResult> OnPostConfirmOTPAsync()
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Retrieve OTP from session or wherever you stored it
                    var savedOTP = HttpContext.Session.GetString("RegisterOTP");

                    // Check if OTP matches
                    if (savedOTP == Input.OTP)
                    {
                        // Create a new user entity
                        var user = new User
                        {
                            FirstName = Input.FirstName,
                            LastName = Input.LastName,
                            Email = Input.Email,
                            Phone = Input.Phone,
                            Address = Input.Address,
                            City = Input.City,
                            Country = Input.Country,
                            Username = Input.Username,
                            Password = Input.Password,
                            Role = "User" // Assign a default role or handle role assignment as per your application logic
                        };

                        // Add user to database
                        _context.Users.Add(user);
                        await _context.SaveChangesAsync();

                        // Perform additional login or redirect logic here
                        // Return success response as JSON
                        return new JsonResult(new { success = true, message = "OTP confirmed and user registered successfully" });
                    }
                    else
                    {
                        // Return error response as JSON
                        return new JsonResult(new { success = false, message = "Invalid OTP. Please try again." });
                    }
                }
                catch (Exception ex)
                {
                    // Return error response as JSON
                    return new JsonResult(new { success = false, message = $"Error confirming OTP: {ex.Message}" });
                }
            }

            // If ModelState is invalid, return error response as JSON
            return new JsonResult(new { success = false, message = "Invalid model state. Please check your inputs." });
        }

        private string GenerateOTP()
        {
            // Generate a random 6-digit OTP
            Random rand = new Random();
            return rand.Next(100000, 999999).ToString();
        }
    }

    public class RegisterInputModel
    {
        [Required(ErrorMessage = "First Name is required")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last Name is required")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Confirm Password is required")]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Phone is required")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Address is required")]
        public string Address { get; set; }

        [Required(ErrorMessage = "City is required")]
        public string City { get; set; }

        [Required(ErrorMessage = "Country is required")]
        public string Country { get; set; }

        [Required(ErrorMessage = "OTP is required")]
        public string OTP { get; set; } // Add OTP field to capture OTP input from form

        [Required(ErrorMessage = "Username is required")]
        public string Username { get; set; }
    }
}
