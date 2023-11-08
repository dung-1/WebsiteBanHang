namespace WebsiteBanHang.Areas.Admin.AdminDTO
{
    public class OderDto
    {
        public int Id { get; set; }
        public string MaHoaDon { get; set; }
        public string ?TenNhanVien { get; set; }
        public DateTime NgayBan { get; set; }
        public string TrangThai { get; set; }
        public string LoaiHoaDon { get; set; }
    }
}
