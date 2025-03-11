using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using HomestayWeb.Models;
using System.Security.Claims;
using System.Threading.Tasks;

namespace HomestayWeb.Pages.MyOrders
{
    public class VoteModel : PageModel
    {
        private readonly ProjectHomeStayContext _context;

        public VoteModel(ProjectHomeStayContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Vote Vote { get; set; }

        public async Task<IActionResult> OnGetAsync(int? homestayId)
        {
            if (homestayId == null || _context.Homestays == null)
            {
                return NotFound();
            }

            var homestay = _context.Homestays.FirstOrDefault(h => h.HomestayId == homestayId);
            if (homestay == null)
            {
                return NotFound();
            }

            var existingVote = _context.Votes.FirstOrDefault(v => v.HomestayId == homestay.HomestayId);

            if (existingVote != null)
            {
                TempData["Message"] = "Bạn đã đánh giá Homestay này rồi.";
                return RedirectToPage("/MyOrders/Index");
            }

            ViewData["HomestayName"] = homestay.HomestayName;
            ViewData["HomestayId"] = homestayId;

            return Page();
        }

        public async Task<IActionResult> OnPostVoteAsync(int homestayId)
        {
            var homestay = await _context.Homestays.FirstOrDefaultAsync(h => h.HomestayId == homestayId);
            if (homestay == null)
            {
                return NotFound();
            }

            var user = HttpContext.User;
            var username = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var userId = _context.Users.Where(u => u.Username == username)
                .Select(u => u.UserId).FirstOrDefault();

            int lastId = _context.Comments.OrderByDescending(x => x.Id).Select(x => x.Id).FirstOrDefault();

            var existingVote = _context.Votes.FirstOrDefault(v => v.HomestayId == homestay.HomestayId);

            if (existingVote != null)
            {
                TempData["Message"] = "Bạn đã đánh giá Homestay này rồi.";
            }
            else
            {
                Vote.HomestayId = homestayId;
                Vote.UserId = userId;
                _context.Votes.Add(Vote);
                _context.SaveChanges();

                TempData["Message"] = "Cảm ơn bạn đã đánh giá Homestay này.";
            }

            return RedirectToPage("/MyOrders/Index");
        }
    }
}