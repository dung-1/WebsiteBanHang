
using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata;
using WebsiteBanHang.Models;

namespace WebsiteBanHang.Areas.Admin.Models
{
    public class UserModel
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
        public Users_Details? userDetail { get; set; } // Reference navigation to dependent
        public ICollection<UserRoleModel> UserRole { get; } = new List<UserRoleModel>(); // Collection navigation containing dependents
        public ICollection<OrderModel> Order { get; } = new List<OrderModel>(); // Collection navigation containing dependents

    }
}
    