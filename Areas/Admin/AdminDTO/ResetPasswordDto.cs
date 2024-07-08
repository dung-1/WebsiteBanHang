using System.ComponentModel.DataAnnotations;

namespace WebsiteBanHang.Areas.Admin.AdminDTO
{
    public class ResetPasswordDto
    {
        public string Token { get; set; }
        public string NewPassword { get; set; }
    }
}
