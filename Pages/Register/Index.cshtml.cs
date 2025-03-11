using HomestayWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Claims;

namespace HomestayWeb.Pages.Register
{
    [AllowAnonymous]
    public class IndexModel : PageModel
    {
        private readonly ProjectHomeStayContext _context;

        public IndexModel(ProjectHomeStayContext context)
        {
            _context = context;
        }

        [BindProperty]
        public User User { get; set; }

        public void OnGet()
        {
            var user = HttpContext.User;
            var username = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if(username != null)
            {
                Response.Redirect("/Index");
            }
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid || _context.Users == null || User == null)
            {
                return Page();
            }
            if (IsUsernameExist(User.Username))
            {
                ModelState.AddModelError("Error", "Username is already exist!");
                return Page();
            }
            if (IsEmailExist(User.Email))
            {
                ModelState.AddModelError("Error", "Email is already exist!");
                return Page();
            }
            else
            {
                User.Status = true;
                _context.Users.Add(User);
                _context.SaveChanges();
                string message = "Register success. Please login to order home stay.";
                return RedirectToPage("/Login/Index", new { message });
            }
        }

        private bool IsUsernameExist(string username)
        {
            return _context.Users.Any(u => u.Username == username);
        }

        private bool IsEmailExist(string email)
        {
            return _context.Users.Any(u =>u.Email == email);
        }
    }
}
