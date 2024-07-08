using System.ComponentModel.DataAnnotations;

namespace WebsiteBanHang.Areas.Admin.Models
{
    public abstract class User
    {

        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(10)]
        public string? MaNguoiDung { get; set; }

        [StringLength(100)]
        public string? Email { get; set; }

        [StringLength(32)]
        public string? MatKhau { get; set; }
        public DateTime NgayTao { get; set; }
        public ICollection<CommentModel> Comments { get; set; } = new List<CommentModel>();
        public string? ResetPasswordToken { get; set; }

        public DateTime? ResetPasswordTokenExpiry { get; set; }

    }
}
