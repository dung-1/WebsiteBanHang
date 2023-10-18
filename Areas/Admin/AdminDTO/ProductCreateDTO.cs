namespace WebsiteBanHang.Areas.Admin.AdminDTO
{
    public class ProductCreateDTO
    {
        public int Id { get; set; }
        public string MaSanPham { get; set; }
        public string TenSanPham { get; set; }
        public int HangId { get; set; }
        public int LoaiId { get; set; }
        public decimal Gia { get; set; }
        public String ImageFile { get; set; }
        public string ThongTinSanPham { get; set; }
    }
}
