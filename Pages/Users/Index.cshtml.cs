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

namespace HomestayWeb.Pages.Users
{
    public class UserSearchDto
    {
        public string SearchValue { get; set; }
        public string Role { get; set; }
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

        public PaginatedList<User> User { get; set; } = default!;
        [BindProperty]
        public UserSearchDto SearchDto { get; set; }
        private int pageSize = 3;
        

        public async Task OnGetAsync(int? pageIndex)
        {
            if (pageIndex == null)
            {
                pageSize = 1;
            }

            SearchDto = new UserSearchDto()
            {
                Role = "USER",
                Status = true
            };

            if (_context.Users != null)
            {
                var users = _context.Users.Where(u => u.Status == SearchDto.Status && u.Role == SearchDto.Role);
                User = await PaginatedList<User>.CreateAsync(
                            users.AsNoTracking(),
                            pageIndex ?? 1, pageSize
                         );
            }
        }

        public async Task OnPostAsync(int? pageIndex)
        {
            if (pageIndex == null)
            {
                pageSize = 1;
            }

            var query = _context.Users.Where(u => u.Status == SearchDto.Status && u.Role == SearchDto.Role);

            if(!string.IsNullOrEmpty(SearchDto.SearchValue))
            {
                var searchValue = SearchDto.SearchValue.ToLower();
                query = query.Where(x => x.Fullname.ToLower().Contains(searchValue) 
                            || x.Username.ToLower().Contains(searchValue)
                            || x.Email.ToLower().Contains(searchValue)
                        );
            }

            User = await PaginatedList<User>.CreateAsync(
                             query.AsNoTracking(),
                             pageIndex ?? 1, pageSize
                          );

        }
    }
}
