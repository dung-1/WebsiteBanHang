using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebsiteBanHang.Areas.Admin.Models
{
    public class PermissionsModel
    {
        [Key]
        public int   Id { get; set; } // Thay đổi tên thành PermissionId

        [Required]
        public bool Access { get; set; }
        public bool Show { get; set; }
        public bool Add { get; set; }
        public bool Edit { get; set; }
        public bool Remote { get; set; }

        public string ?FunctionName { get; set; } // Thay đổi tên của thuộc tính này nếu cần
        public ICollection<PermissionRoleModel> PermissionRole { get; } = new List<PermissionRoleModel>();

        // Các thuộc tính khác của 'PermissionsModel'
    }
}
