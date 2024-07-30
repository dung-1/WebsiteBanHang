using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Web.Mvc;
using WebsiteBanHang.Areas.Admin.AdminDTO;
using WebsiteBanHang.Areas.Admin.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;
using X.PagedList;
using WebsiteBanHang.Areas.Admin.Models;
using WebsiteBanHang.Areas.Admin.Common;

namespace WebsiteBanHang.ViewComponents
{
    public class PostTopTechnologyViewComponent : ViewComponent
    {
        private readonly ILogger<PostTopTechnologyViewComponent> _logger;
        private readonly ApplicationDbContext _context;

        public PostTopTechnologyViewComponent(ApplicationDbContext context, ILogger<PostTopTechnologyViewComponent> logger)
        {
            _context = context;
            _logger = logger;
        }

        private async Task<IViewComponentResult> GetPostsTechnologyAsync()
        {
            try
            {
                var posts = await _context.Posts
                                 .Where(p => p.Category.Name == "Công Nghệ" && p.Status== StatusActivity.Active && p.FromDate <= DateTime.Now && p.ToDate>= DateTime.Now)
                                 .OrderByDescending(p => p.CreatedTime)
                                 .Take(4)
                                 .Select(p => new PostsViewDto
                                 {
                                     Id = p.Id,
                                     ExcerptImage = p.ExcerptImage,
                                     CreatedTime = p.CreatedTime,
                                     Title = p.Title
                                 }).ToListAsync();

                return View(posts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting technology posts.");
                return View();
            }
        }

        public Task<IViewComponentResult> InvokeAsync()
        {
            return GetPostsTechnologyAsync();
        }
    }
}