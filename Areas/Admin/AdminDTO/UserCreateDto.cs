using WebsiteBanHang.Areas.Admin.Models;

namespace WebsiteBanHang.Areas.Admin.AdminDTO
{
    public class UserCreateDto
    {
        public int Id { get; set; }
        public string? Email { get; set; }
        public string? MatKhau { get; set; }
        public DateTime NgayTao { get; set; }
        public string? HoTen { get; set; }
        public int SoDienThoai { get; set; }
        public string? DiaChi { get; set; }
        public int? VaiTroId { get; set; } // Lưu ID của RoleModel

    }
}
