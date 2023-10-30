
using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata;

namespace WebsiteBanHang.Areas.Admin.Models
{
    public class UserModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string? Email { get; set; }

        [StringLength(32)]
        public string? MatKhau { get; set; }
        public DateTime NgayTao { get; set; }
        public Users_Details? userDetail { get; set; } // Reference navigation to dependent
        public ICollection<UserRoleModel> UserRole { get; } = new List<UserRoleModel>(); // Collection navigation containing dependents

    }
}
    