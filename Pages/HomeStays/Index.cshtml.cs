using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using HomestayWeb.Models;
using Microsoft.AspNetCore.Authorization;
using HomestayWeb.Utils;

namespace HomestayWeb.Pages.HomeStays
{
    public class Search
    {
        public string SearchString { get; set; }
        public string City { get; set; }
        public string District { get; set; }
        public string Ward { get; set; }
        public bool Status { get; set; }
    }

    [Authorize(Policy = "Admin")]
    public class IndexModel : PageModel
    {
        private readonly HomestayWeb.Models.ProjectHomeStayContext _context;

        public IndexModel(HomestayWeb.Models.ProjectHomeStayContext context)
        {
            _context = context;
        }

        public PaginatedList<Homestay> Homestay { get; set; } = default!;
        [BindProperty]
        public Search Search { get; set; } = default!;
        private int pageSize = 3;

        public async Task OnGetAsync(int? pageIndex)
        {
            if(pageIndex == null)
            {
                pageIndex = 1;
            }

            IQueryable<Homestay> homestays = _context.Homestays;

            if (_context.Homestays != null)
            {
                Homestay = await PaginatedList<Homestay>.CreateAsync(
                            homestays.AsNoTracking(),
                            pageIndex ?? 1, pageSize
                         );
            }
            Search = new Search()
            {
                Status = true
            };
        }

        public async Task OnPostAsync(int? pageIndex)
        {
            var query = _context.Homestays.Include(h => h.Images).Where(h => h.Status == Search.Status);

            if (!string.IsNullOrEmpty(Search?.SearchString))
            {
                query = query.Where(h => h.HomestayName.Contains(Search.SearchString));
            }

            if (!string.IsNullOrEmpty(Search?.City))
            {
                query = query.Where(h => h.City.Equals(Search.City));
            }

            if (!string.IsNullOrEmpty(Search?.District))
            {
                query = query.Where(h => h.District.Equals(Search.District));
            }

            if (!string.IsNullOrEmpty(Search?.Ward))
            {
                query = query.Where(h => h.Commune.Equals(Search.Ward));
            }

            Homestay = await PaginatedList<Homestay>.CreateAsync(
                            query.AsNoTracking(),
                            pageIndex ?? 1, pageSize
                         );
        }
    }
}
