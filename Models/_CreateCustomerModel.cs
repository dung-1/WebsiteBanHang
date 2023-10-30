namespace WebsiteBanHang.Models
{
    public class _CreateCustomerModel
    {
        public int Id { get; set; }
        public string TenTaiKhoan { get; set; }
        public string Email { get; set; }
        public int MatKhau { get; set; }
        public DateTime NgayTao { get; set; }
        public string HoTen { get; set; }
        public int SoDienThoai { get; set; }
        public string DiaChi { get; set; }
    }
}
