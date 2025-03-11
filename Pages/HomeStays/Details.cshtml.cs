using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using HomestayWeb.Models;
using Microsoft.Identity.Client;
using HomestayWeb.Dtos;
using Newtonsoft.Json;
using System.Security.Claims;
using HomestayWeb.Contants;

namespace HomestayWeb.Pages.HomeStays
{
    public class DetailsModel : PageModel
    {
        private readonly HomestayWeb.Models.ProjectHomeStayContext _context;

        public DetailsModel(HomestayWeb.Models.ProjectHomeStayContext context)
        {
            _context = context;
        }

        public Homestay Homestay { get; set; } = default!;
        public List<Vote> HomestayVotes { get; set; } = new List<Vote>();

        [BindProperty]
        public CartItem CartItem { get; set; } = default!;
        public decimal PriceWhenSell { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.Homestays == null)
            {
                return NotFound();
            }

            var user = HttpContext.User;
            var username = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var role = user.FindFirst("Role")?.Value;

            if (!string.IsNullOrEmpty(role) && "ADMIN".Equals(role))
            {
                Homestay = await _context.Homestays
                    .Include(h => h.Images)
                    .Include(h => h.Discounts)
                    .Include(h => h.Votes)
                    .FirstOrDefaultAsync(m => m.HomestayId == id);
                try
                {
                    HomestayVotes = Homestay.Votes.ToList();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

                PriceWhenSell = getPriceSell(Homestay);

                return Page();
            }

            var homestay = await _context.Homestays
                 .Include(h => h.Images)
                 .Include(h => h.Discounts)
                 .Include(h => h.Votes)
                 .FirstOrDefaultAsync(m => m.HomestayId == id && m.Status);
            PriceWhenSell = getPriceSell(homestay);

            if (homestay == null)
            {
                return NotFound();
            }
            else
            {
                Homestay = homestay;
                var userIds = homestay.Votes.Select(v => v.UserId).ToList();
                var voters = await _context.Users
                .Where(u => userIds.Contains(u.UserId))
                .ToDictionaryAsync(u => u.UserId);
                HomestayVotes = homestay.Votes.ToList();
            }
            return Page();
        }

        private decimal getPriceSell(Homestay homestay)
        {
            var discount = homestay.Discounts;
            var priceWhenSell = homestay.Price;
            var currentDate = DateTime.Now;
            if (discount != null)
            {
                decimal totalDiscount = discount
                    .Where(d => d.DateStart <= currentDate && d.DateEnd >= currentDate)
                    .Sum(x => ((decimal)x.Discount1 / 100) * priceWhenSell);
                priceWhenSell -= totalDiscount;
            }
            return priceWhenSell;
        }

        public IActionResult OnPostAddToCart(string? password, int id)
        {
            try
            {
                // Authentication check
                var username = AuthenticateUser();

                if (!ModelState.IsValid)
                {
                    Homestay = _context.Homestays.Include(h => h.Images).SingleOrDefault(m => m.HomestayId == id && m.Status);
                    PriceWhenSell = getPriceSell(Homestay);
                    return Page();
                }

                // Check homestay availability
                CheckHomestayAvailability(id, CartItem.DateStart, CartItem.DateEnd);

                // Get user
                var customer = GetUser(username, password);

                // Create order
                var order = CreateOrder(customer.UserId, id);

                // Create order detail
                CreateOrderDetail(order.OrderId, CartItem.DateStart, CartItem.DateEnd, id);

                return RedirectToAction("/HomeStays/Details", new { id, message = "Order request is pending confirm" });
            }
            catch (Exception ex)
            {
                return RedirectToAction("/HomeStays/Details", new { id, message = ex.Message });
            }
        }

        private string AuthenticateUser()
        {
            var user = HttpContext.User;
            var username = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (username == null)
            {
                throw new Exception("Please login to order.");
            }

            return username;
        }

        private void CheckHomestayAvailability(int id, DateTime fromDate, DateTime endDate)
        {
            // Retrieve homestay with images
            Homestay = _context.Homestays
                .Include(h => h.Images)
                .SingleOrDefault(m => m.HomestayId == id && m.Status);

            if (Homestay == null)
            {
                throw new Exception($"Can not find homestay with id: {id}");
            }

            if (Homestay.Type != HomeStayType.AVAILABLE)
            {
                throw new Exception("This homestay is not available to order");
            }

            var currentDate = DateTime.Now;

            // Retrieve orders for the homestay where the current date is within the order period
            var orders = _context.Orders
                .Where(o => o.HomestayId == id && currentDate <= o.OrderDetails.FirstOrDefault().FromDate);

            // Check if there are any overlapping orders for the specified date range
            var isValidOrder = orders.Any(o =>
                    (o.OrderDetails.FirstOrDefault().FromDate <= fromDate && o.OrderDetails.FirstOrDefault().EndDate >= fromDate)
                    || (o.OrderDetails.FirstOrDefault().FromDate <= endDate && o.OrderDetails.FirstOrDefault().EndDate >= endDate));

            if (isValidOrder)
            {
                throw new Exception("Homestay in this date range was already ordered.");
            }
        }


        private User GetUser(string username, string password)
        {
            if (password == null)
            {
                throw new Exception("Please enter your password to order");
            }

            var customer = _context.Users.SingleOrDefault(u => u.Password == password && u.Username == username);

            if (customer == null)
            {
                throw new Exception("Password is invalid");
            }

            return customer;
        }

        private Order CreateOrder(int userId, int homestayId)
        {
            DateTime currentDate = DateTime.Now;
            Order order = new Order()
            {
                UserId = userId,
                HomestayId = homestayId,
                OrderDate = currentDate,
                Status = OrderStatus.PENDING_CONFIRM
            };

            _context.Orders.Add(order);
            _context.SaveChanges();

            return order;
        }

        private void CreateOrderDetail(int orderId, DateTime fromDate, DateTime endDate, int homestayId)
        {
            OrderDetail orderDetail = new OrderDetail()
            {
                OrderId = orderId,
                FromDate = fromDate,
                EndDate = endDate,
                PriceWhenSell = getPriceWhenSell(homestayId, DateTime.Now),
                IsPayment = false
            };

            _context.OrderDetails.Add(orderDetail);
            _context.SaveChanges();
        }

        private void UpdateHomestayStatus(int homestayId)
        {
            Homestay.Type = HomeStayType.ORDERED;
            _context.Homestays.Update(Homestay);
            _context.SaveChanges();
        }

        private decimal getPriceWhenSell(int homstayId, DateTime currentDate)
        {
            decimal price = 0;

            List<Discount> discounts = _context.Discounts
                .Where(d => d.HomstayId == homstayId && d.DateStart <= currentDate && d.DateEnd >= currentDate)
                .ToList();

            Homestay? homestay = _context.Homestays.SingleOrDefault(d => d.HomestayId == homstayId);

            decimal totalDiscount = 0;
            if (homestay != null)
            {
                decimal homestayPrice = homestay.Price;

                if (homestayPrice != 0 && discounts.Any())
                {
                    totalDiscount = discounts.Sum(x => ((decimal)x.Discount1 / 100) * homestayPrice);
                }

                price = homestay.Price - totalDiscount;

                return price < 0 ? 0 : price;
            }

            throw new Exception("Homestay not found");
        }

    }
}
