﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SuxrobGM.Sdk.Pagination;
using EC_Website.Core.Entities.Blog;
using EC_Website.Infrastructure.Data;

namespace EC_Website.Web.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public PaginatedList<BlogEntry> BlogEntries { get; set; }

        public async Task<IActionResult> OnGetAsync(int pageIndex = 1)
        {
            BlogEntries = await PaginatedList<BlogEntry>.CreateAsync(_context.BlogEntries.AsNoTracking(), pageIndex, 5);

            return Page();
        }
    }
}