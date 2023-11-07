using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebsiteBanHang.Areas.Admin.Data;

namespace WebsiteBanHang.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class OrderDetailController : Controller
    {
        private readonly ILogger<OrderDetailController> _logger;

        private readonly ApplicationDbContext _context;
        public OrderDetailController(ApplicationDbContext context, ILogger<OrderDetailController> logger)
        {
            _context = context;
            _logger = logger;

        }
        [Authorize(Roles = "Admin,Employee")]

        public IActionResult Index()
        {
            // Sắp xếp lại danh sách theo ID giảm dần (mới nhất lên đầu)
            var sortedCategories = _context.Order_Detai.OrderByDescending(c => c.ID).ToList();
            return View(sortedCategories);
        }
        public IActionResult Delete(int? id)
        {
            var deleterecord = _context.Product.Find(id);
            if (deleterecord == null)
            {
                return NotFound();
            }
            _context.Product.Remove(deleterecord);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

    }
}
