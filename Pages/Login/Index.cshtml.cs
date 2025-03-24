using HomestayWeb.Dtos;
using HomestayWeb.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace HomestayWeb.Pages.Login
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
        public UserRequest UserRequest { get; set; }

        public void OnGet()
        {
            var user = HttpContext.User;
            var username = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (username != null)
            {
                Response.Redirect("/Index");
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                User? user = _context.Users.SingleOrDefault(
                    x => x.Username == UserRequest.Username &&
                         x.Password == UserRequest.Password &&
                         x.Status == true
                    );

                if (user == null)
                {
                    ModelState.AddModelError("Error", "Wrong username or password.");
                    return Page();
                }

                await SignInUser(user);
                return RedirectToPage("/Index");
            }
            return Page();
        }

        public IActionResult OnGetGoogleLogin()
        {
            var properties = new AuthenticationProperties
            {
                RedirectUri = Url.Page("/Login/Index", pageHandler: "GoogleCallback"),
                IsPersistent = true
            };
            return new ChallengeResult(GoogleDefaults.AuthenticationScheme, properties);
        }

        public async Task<IActionResult> OnGetGoogleCallbackAsync()
        {
            var authenticateResult = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            if (!authenticateResult.Succeeded || authenticateResult?.Principal == null)
            {
                return RedirectToPage("/Login/Index", new { message = "Google authentication failed" });
            }

            var claims = authenticateResult.Principal.Identities.FirstOrDefault()?.Claims;
            var email = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var name = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;

            if (string.IsNullOrEmpty(email))
            {
                return RedirectToPage("/Login/Index", new { message = "Unable to retrieve email from Google" });
            }

            var user = _context.Users.FirstOrDefault(u => u.Email == email);

            // Kiểm tra nếu tài khoản tồn tại nhưng bị vô hiệu hóa
            if (user != null && user.Status == false)
            {
                return RedirectToPage("/Login/Index", new { message = "Tài khoản của bạn đã bị vô hiệu hóa!" });
            }

            if (user == null)
            {
                // Tạo tài khoản mới nếu chưa tồn tại
                user = new User
                {
                    Email = email,
                    Fullname = name,
                    Username = email.Split('@')[0] + "_" + Guid.NewGuid().ToString().Substring(0, 8),
                    Role = "User",
                    Status = true,
                    Password = Guid.NewGuid().ToString()
                };
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
            }

            await SignInUser(user);
            return RedirectToPage("/Index");
        }


        private async Task SignInUser(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Username),
                new Claim("Role", user.Role)
            };

            var claimsIdentity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                AllowRefresh = true,
                IsPersistent = true,
                IssuedUtc = DateTimeOffset.UtcNow
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);
        }
    }
}