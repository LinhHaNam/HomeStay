using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using HomestayWeb.Models;
using System.ComponentModel.DataAnnotations;
using HomestayWeb.Service;

namespace HomestayWeb.Pages.ForgotPassword
{
    public class IndexModel : PageModel
    {
        private readonly ProjectHomeStayContext _dbContext;
        private readonly IEmailService _emailService;

        public IndexModel(ProjectHomeStayContext dbContext, IEmailService emailService)
        {
            _dbContext = dbContext;
            _emailService = emailService;
        }

        [BindProperty]
        public UserRequestModel UserRequest { get; set; }

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = _dbContext.Users.SingleOrDefault(u => u.Email == UserRequest.Email);
            if (user == null)
            {
                ModelState.AddModelError("", "Không tồn tại người dùng với địa chỉ email này.");
                return Page();
            }

            // Tạo mật khẩu ngẫu nhiên
            var random = new Random();
            var newPassword = new string(Enumerable.Repeat("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789", 8)
                .Select(s => s[random.Next(s.Length)]).ToArray());

            user.Password = newPassword; 
            _dbContext.SaveChanges();

            // Gửi email với mật khẩu mới
            try
            {
                await _emailService.SendEmailAsync(
                    UserRequest.Email,
                    "Mật khẩu mới của bạn",
                    $"Chào bạn,<br>Mật khẩu mới của bạn là: <strong>{newPassword}</strong><br>Vui lòng đăng nhập và đổi mật khẩu sau khi sử dụng.");

                TempData["Message"] = "Mật khẩu mới đã được gửi qua email. Vui lòng kiểm tra hộp thư của bạn.";
                return RedirectToPage("/Login/Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Lỗi khi gửi email: {ex.Message}");
                return Page();
            }
        }
    }

    public class UserRequestModel
    {
        [Required(ErrorMessage = "Vui lòng nhập địa chỉ Email.")]
        [EmailAddress(ErrorMessage = "Địa chỉ Email không hợp lệ.")]
        public string Email { get; set; }
    }
}