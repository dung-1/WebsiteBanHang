using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace WebsiteBanHang.Areas.Admin.Models
{
    public class PermissionRoleModel
    {
        [Key]
        [Column(Order = 0)]
        [ForeignKey("Permission")]
        public int Permission_ID { get; set; } // Khóa ngoại đến UserModel
        [Key]
        [Column(Order = 1)]
        [ForeignKey("Role")]
        public int Role_ID { get; set; } // Khóa ngoại đến RoleModel
        public PermissionsModel? Permission { get; set; }
        public RoleModel? Role { get; set; }
    }
}

