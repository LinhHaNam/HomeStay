using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HomestayWeb.Models;
using Microsoft.AspNetCore.Authorization;
using HomestayWeb.Validations;

namespace HomestayWeb.Pages.Discounts
{
    [Authorize(Policy = "Admin")]
    public class EditModel : PageModel
    {
        private readonly HomestayWeb.Models.ProjectHomeStayContext _context;

        public EditModel(HomestayWeb.Models.ProjectHomeStayContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Discount Discount { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.Discounts == null)
            {
                return NotFound();
            }

            var discount =  await _context.Discounts.FirstOrDefaultAsync(m => m.DiscountId == id);
            if (discount == null)
            {
                return NotFound();
            }
            Discount = discount;
           ViewData["HomstayId"] = new SelectList(_context.Homestays, "HomestayId", "HomestayId");
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            ViewData["HomstayId"] = new SelectList(_context.Homestays, "HomestayId", "HomestayId");
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(Discount).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DiscountExists(Discount.DiscountId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool DiscountExists(int id)
        {
          return (_context.Discounts?.Any(e => e.DiscountId == id)).GetValueOrDefault();
        }
    }
}
