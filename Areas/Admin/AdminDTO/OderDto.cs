namespace WebsiteBanHang.Areas.Admin.AdminDTO
{
    public class OrderDto
    {
        public int Id { get; set; }
        public string? MaHoaDon { get; set; }
        public string? TenNhanVien { get; set; }
        public string? TenKhachHang { get; set; }
        public string? SoDienThoai { get; set; }
        public string? DiaChi { get; set; }
        public DateTime? NgayBan { get; set; }
        public string? TrangThai { get; set; }
        public string? LoaiHoaDon { get; set; }
        public List<ChiTietHoaDonDto>? ChiTietHoaDon { get; set; }

        public decimal?  TongCong { get; set; } 
    }

    public class ChiTietHoaDonDto
    {
        public string? img { get; set; }
        public string? TenSanPham { get; set; }
        public int SoLuong { get; set; }
        public decimal Gia { get; set; }
    }


}
