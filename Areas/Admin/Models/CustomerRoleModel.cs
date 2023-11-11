using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebsiteBanHang.Areas.Admin.Models
{
    public class CustomerRoleModel
    {
        [Key]
        [Column(Order = 0)]
        [ForeignKey("Customer")]
        public int Customer_ID { get; set; } // Khóa ngoại đến CustomerModel
        [Key]
        [Column(Order = 1)]
        [ForeignKey("Role")]
        public int Role_ID { get; set; } // Khóa ngoại đến RoleModel
        public CustomerModel? Customer { get; set; }
        public RoleModel? Role { get; set; }
    }
}
