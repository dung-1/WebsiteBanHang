using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebsiteBanHang.Areas.Admin.Models
{
    public class UserRoleModel
    {
        [Key]
        [Column(Order = 0)]
        [ForeignKey("User")]
        public int User_ID { get; set; } // Khóa ngoại đến UserModel
        [Key]
        [Column(Order = 1)]
        [ForeignKey("Role")]
        public int Role_ID { get; set; } // Khóa ngoại đến RoleModel
        public UserModel? User { get; set; }
        public RoleModel? Role { get; set; }
    }
}
