using System.ComponentModel.DataAnnotations;

namespace WebsiteBanHang.Areas.Admin.AdminDTO
{
    public class ForgotPasswordDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
