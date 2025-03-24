using HomestayWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace HomestayWeb.Pages.Profile
{
    public class IndexModel : PageModel
    {
        private readonly ProjectHomeStayContext _context;

        public IndexModel(ProjectHomeStayContext context)
        {
            _context = context;
        }

        [BindProperty]
        public User Customer { get; set; }

        [BindProperty]
        public string CurrentPassword { get; set; }

        [BindProperty]
        public string NewPassword { get; set; }

        [BindProperty]
        public string ConfirmPassword { get; set; }

        public string Message { get; set; }

        public void OnGet()
        {
            var user = HttpContext.User;
            var username = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (username != null)
            {
                Customer = _context.Users.SingleOrDefault(x => x.Username == username);
            }
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Check email uniqueness
            if (IsEmailExist(Customer.Email))
            {
                ModelState.AddModelError("Customer.Email", "Email is already in use.");
                return Page();
            }

            var user = _context.Users.SingleOrDefault(x => x.UserId == Customer.UserId);
            if (user == null)
            {
                return NotFound();
            }

            // Handle password change
            if (!string.IsNullOrEmpty(CurrentPassword) ||
                !string.IsNullOrEmpty(NewPassword) ||
                !string.IsNullOrEmpty(ConfirmPassword))
            {
                // Verify all password fields are filled
                if (string.IsNullOrEmpty(CurrentPassword) ||
                    string.IsNullOrEmpty(NewPassword) ||
                    string.IsNullOrEmpty(ConfirmPassword))
                {
                    ModelState.AddModelError("", "Please fill all password fields to change password.");
                    return Page();
                }

                // Verify current password (assuming password is hashed)
                if (!VerifyPassword(user.Password, CurrentPassword))
                {
                    ModelState.AddModelError("CurrentPassword", "Current password is incorrect.");
                    return Page();
                }

                // Verify new password matches confirmation
                if (NewPassword != ConfirmPassword)
                {
                    ModelState.AddModelError("ConfirmPassword", "New password and confirmation do not match.");
                    return Page();
                }

                // Update password (you should hash the new password)
                user.Password = HashPassword(NewPassword);
            }

            // Update other fields
            user.Fullname = Customer.Fullname;
            user.Email = Customer.Email;
            user.Gender = Customer.Gender;

            _context.Users.Update(user);
            _context.SaveChanges();

            Message = "Profile updated successfully";
            return Page();
        }

        private bool IsEmailExist(string email)
        {
            var user = HttpContext.User;
            var username = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return _context.Users.Any(y => y.Email == email && y.Username != username);
        }

        // You need to implement these methods based on your password hashing mechanism
        private string HashPassword(string password)
        {
            // Implement password hashing (e.g., using BCrypt or ASP.NET Identity)
            // This is a placeholder - replace with actual implementation
            return password;
        }

        private bool VerifyPassword(string hashedPassword, string providedPassword)
        {
            // Implement password verification
            // This is a placeholder - replace with actual implementation
            return hashedPassword == providedPassword;
        }
    }
}