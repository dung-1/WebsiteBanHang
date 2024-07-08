using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Razorpay.Api;
using System.Globalization;
using WebsiteBanHang.Areas.Admin.AdminDTO;
using WebsiteBanHang.Areas.Admin.Data;
using WebsiteBanHang.Areas.Admin.Models;
using WebsiteBanHang.HubSignalR;
using X.PagedList;

namespace WebsiteBanHang.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,Employee")]
    public class ChatChitController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ChatChitController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var chatConnections = _context.ChatConnection.ToList();
            return View(chatConnections);
        }
        [HttpGet]
        public IActionResult Chat(int customerId)
        {
            var customer = _context.Customer.Find(customerId);
            if (customer == null)
            {
                return NotFound();
            }

            ViewBag.CustomerId = customerId;
            return View();
        }

    }
}
