namespace WebsiteBanHang.Areas.Admin.AdminDTO
{
    public class Brand_exrepoting_Dto
    {
        public int Id { get; set; }
        public string? MaHang { get; set; }
        public string? TenHang { get; set; }
        public string? XuatXu { get; set; } // Tên của Brand
        public DateTime NgaySanXuat { get; set; }
    }
}