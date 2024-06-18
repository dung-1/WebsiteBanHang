namespace WebsiteBanHang.Areas.Admin.AdminDTO
{
    public class Product_exrepoting_Dto
    {
        public int Id { get; set; }
        public string? MaSanPham { get; set; }
        public string? TenSanPham { get; set; }
        public string? HangTen { get; set; } // Tên của Brand
        public string? LoaiTen { get; set; }
        public decimal GiaNhap { get; set; }
        public decimal GiaBan { get; set; }
        public decimal GiaGiam { get; set; }
        public string? Image { get; set; }
        public string? ThongTinSanPham { get; set; }
    }
}
