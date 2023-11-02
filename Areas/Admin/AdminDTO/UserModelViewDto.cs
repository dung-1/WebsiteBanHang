namespace WebsiteBanHang.Areas.Admin.AdminDTO
{
    public class UserModelViewDto
    {
        public int Id { get; set; }
        public string? MaNguoiDung { get; set; }
        public string? HoTen { get; set; }
        public string? Email { get; set; }
        public string? VaiTro { get; set; }
        internal UserModelViewDto Select(Func<object, UserModelViewDto> value)
        {
            throw new NotImplementedException();
        }
    }
}
