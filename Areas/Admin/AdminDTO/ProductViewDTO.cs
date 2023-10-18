namespace WebsiteBanHang.Areas.Admin.AdminDTO
{
    public class ProductViewDTO
    {
        public int Id { get; set; }
        public string MaSanPham { get; set; }
        public string TenSanPham { get; set; }
        public string HangTen { get; set; } // Tên của Brand
        public string LoaiTen { get; set; } // Tên của Category
        public decimal Gia { get; set; }
        public string ThongTinSanPham { get; set; }
        public string Image { get; set; } = string.Empty;
    }
}
