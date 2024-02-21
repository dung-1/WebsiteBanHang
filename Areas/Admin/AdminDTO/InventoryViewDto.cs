using System.ComponentModel.DataAnnotations;
using WebsiteBanHang.Areas.Admin.Models;

namespace WebsiteBanHang.Areas.Admin.AdminDTO
{
    public class InventoryViewDto
    {
        public int Id { get; set; }
        public string? MaKho { get; set; }
        public string? TenSanPham { get; set; } 
        public DateTime? NgayNhap { get; set; }
        public int SoLuong { get; set; }
    }
}
