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

namespace HomestayWeb.Pages.HomeStays
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
        public Homestay Homestay { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.Homestays == null)
            {
                return NotFound();
            }

            var homestay =  await _context.Homestays.FirstOrDefaultAsync(m => m.HomestayId == id);
            if (homestay == null)
            {
                return NotFound();
            }
            Homestay = homestay;
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(Homestay).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!HomestayExists(Homestay.HomestayId))
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

        private bool HomestayExists(int id)
        {
          return (_context.Homestays?.Any(e => e.HomestayId == id)).GetValueOrDefault();
        }
    }
}
