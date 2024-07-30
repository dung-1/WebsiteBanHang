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

namespace WebsiteBanHang.ViewComponents
{
    [Authorize(Roles = "Customer")]
    public class PostTopTechnologyViewComponent : ViewComponent
    {
        private readonly ILogger<PostTopTechnologyViewComponent> _logger;
        private readonly ApplicationDbContext _context;

        public PostTopTechnologyViewComponent(ApplicationDbContext context, ILogger<PostTopTechnologyViewComponent> logger)
        {
            _context = context;
            _logger = logger;
        }
        public IViewComponentResult Invoke()
        {
            return View();
        }

    }
}