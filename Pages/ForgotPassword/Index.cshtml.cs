using System;
using System.Linq;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using HomestayWeb.Models;
using System.ComponentModel.DataAnnotations;

namespace HomestayWeb.Pages.ForgotPassword
{
    public class IndexModel : PageModel
    {
        [BindProperty]
        public UserRequestModel UserRequest { get; set; }

        private readonly ProjectHomeStayContext _dbContext;
        public IndexModel(ProjectHomeStayContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var user = _dbContext.Users.SingleOrDefault(u => u.Email == UserRequest.Email);

                if (user != null)
                {
                    try
                    {
                        var random = new Random();
                        var randomPassword = new string(Enumerable.Repeat("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789", 8)
                            .Select(s => s[random.Next(s.Length)]).ToArray());

                        user.Password = randomPassword;
                        _dbContext.SaveChanges();

                        var smtpClient = new SmtpClient("smtp.gmail.com")
                        {
                            Port = 587,
                            Credentials = new NetworkCredential("pau30012002@gmail.com", "pljf fqgx yycq ynhq"),
                            EnableSsl = true,
                        };

                        var mailMessage = new MailMessage
                        {
                            From = new MailAddress("pau30012002@gmail.com"),
                            Subject = "Forgot Password",
                            Body = $"Hello, your new password is: {randomPassword}",
                        };

                        mailMessage.To.Add(UserRequest.Email);

                        smtpClient.Send(mailMessage);

                        TempData["Message"] = "Yêu cầu đặt lại mật khẩu đã được gửi. Vui lòng kiểm tra email của bạn.";
                        return RedirectToPage("/Login/Index");
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("", $"Error sending email: {ex.Message}");
                        return Page();
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Không tồn tại người dùng với địa chỉ email này.");
                    return Page();
                }
            }

            return Page();
        }
    }

    public class UserRequestModel
    {
        [Required(ErrorMessage = "Vui lòng nhập địa chỉ Email.")]
        [EmailAddress(ErrorMessage = "Địa chỉ Email không hợp lệ.")]
        public string Email { get; set; }
    }
}