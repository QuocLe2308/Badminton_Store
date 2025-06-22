using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using DataAccessLayer.DbContext;
using DataAccessLayer.Models;

namespace WebApp.Pages.Account
{
    public class LoginDTO
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }

    public class LoginModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        [BindProperty]
        public LoginDTO Input { get; set; }

        public LoginModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var user = _context.Users.FirstOrDefault(u => u.Email == Input.Email);
                    if (user != null)
                    {
                        // Perform password validation manually
                        if (user.Password == Input.Password) // Replace with your actual password validation logic
                        {
                            // Store user information in session
                            HttpContext.Session.SetInt32("UserId", user.CustomerID);
                            HttpContext.Session.SetString("UserName", user.Username);
							HttpContext.Session.SetString("Role", user.Role);
							// Successful login
							// Perform additional actions if needed, such as logging, token creation, etc.
							return LocalRedirect(returnUrl ?? "/Dashboard");
                        }
                        else
                        {
                            ModelState.AddModelError(string.Empty, "Invalid email or password.");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "User not found.");
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, $"Error: {ex.Message}");
                }
            }

            // Return page with errors
            return Page();
        }
    }
}
