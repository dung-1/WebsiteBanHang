namespace WebsiteBanHang.Areas.Admin.AdminDTO
{
    public class ProductViewDTO
    {
        public int Id { get; set; }
        public string? MaSanPham { get; set; }
        public string? TenSanPham { get; set; }
        public string? HangTen { get; set; } // Tên của Brand
        public string? LoaiTen { get; set; }
        public decimal Gia { get; set; }
        public decimal GiaGiam { get; set; }
        public string? Image { get; set; }
        public string? ThongTinSanPham { get; set; }

        // Checkboxes for filtering
        public bool CheckboxAll { get; set; }
        public bool CheckboxPrice1 { get; set; }
        public bool CheckboxPrice2 { get; set; }
        public bool CheckboxPrice3 { get; set; }
        public bool CheckboxPrice4 { get; set; }
        public bool CheckboxPrice5 { get; set; }

        internal ProductViewDTO Select(Func<object, ProductViewDTO> value)
        {
            throw new NotImplementedException();
        }
    }
}
