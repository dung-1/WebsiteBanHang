using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata;

namespace WebsiteBanHang.Areas.Admin.Models
{
    public class RoleModel
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(50)]
        public string? Name { get; set; }
        public ICollection<UserRoleModel> UserRole { get; } = new List<UserRoleModel>(); // Collection navigation containing dependents
        public ICollection<PermissionRoleModel> PermissionRole { get; } = new List<PermissionRoleModel>(); // Collection navigation containing dependents

    }
}
