using HomestayWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

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

        public void OnGet()
        {
            var user = HttpContext.User;
            var username = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (username != null)
            {
                Customer = _context.Users.SingleOrDefault(x => x.Username == username);
            }
        }

        public string Message { get; set; }

        public IActionResult OnPost()
        {
            if(ModelState.IsValid)
            {
                if(IsEmailExist(Customer.Email))
                {
                    ModelState.AddModelError("Error", "Email is already exist.");
                    return Page();
                }

                _context.Users.Update(Customer);
                _context.SaveChanges();
            }
            Message = "Update profile success";
            return Page();
        }

        public Boolean IsEmailExist(string email)
        {
            var user = HttpContext.User;
            var username = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return _context.Users.Any(y => y.Email == email && y.Username != username);
        }
    }
}
