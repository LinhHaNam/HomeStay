using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using HomestayWeb.Models;

namespace HomestayWeb.Pages.Orders
{
    public class CreateModel : PageModel
    {
        private readonly HomestayWeb.Models.ProjectHomeStayContext _context;

        public CreateModel(HomestayWeb.Models.ProjectHomeStayContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
        ViewData["HomestayId"] = new SelectList(_context.Homestays, "HomestayId", "HomestayId");
        ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId");
            return Page();
        }

        [BindProperty]
        public Order Order { get; set; } = default!;
        

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
          if (!ModelState.IsValid || _context.Orders == null || Order == null)
            {
                return Page();
            }

            _context.Orders.Add(Order);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
