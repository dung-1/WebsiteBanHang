using System.ComponentModel.DataAnnotations;

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
        public ICollection<UserRoleModel> UserRole { get; } = new List<UserRoleModel>(); // Collection navigation containing dependents
        public ICollection<OrdersModel> Order { get; } = new List<OrdersModel>(); // Collection navigation containing dependents
        public Users_Details? userDetail { get; set; } = new Users_Details(); // Khởi tạo giá trị mặc định

    }
}
    