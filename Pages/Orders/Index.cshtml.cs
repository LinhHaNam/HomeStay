using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using HomestayWeb.Models;
using Microsoft.AspNetCore.SignalR;
using HomestayWeb.Hubs;

namespace HomestayWeb.Pages.Orders
{
    public class IndexModel : PageModel
    {
        private readonly HomestayWeb.Models.ProjectHomeStayContext _context;
        private readonly IHubContext<ClientHub> _hubContext;

        public IndexModel(HomestayWeb.Models.ProjectHomeStayContext context, IHubContext<ClientHub> hub)
        {
            _context = context;
            _hubContext = hub;
        }

        public IList<Order> Order { get;set; } = default!;

        public async Task OnGetAsync()
        {
            if (_context.Orders != null)
            {
                Order = await _context.Orders
                .Include(o => o.Homestay)
                .Include(o => o.User)
                .Include(o => o.OrderDetails)
                .ToListAsync();
            }
        }

        public IActionResult OnPost(int id, string status)
        {
            var order = _context.Orders
                .Include(o => o.User)
                .SingleOrDefault(o => o.OrderId== id);
            if (order == null)
            {
                return NotFound();
            }
            order.Status = status;
            _context.Orders.Update(order);
            _context.SaveChanges();

            var message = "Your order's status was moved to " + status;
            _hubContext.Clients.All.SendAsync(order.User.Username, message);

            Order = _context.Orders
                .Include(o => o.Homestay)
                .Include(o => o.User)
                .Include(o => o.OrderDetails)
                .ToList();
            return Page();
        }
    }
}
