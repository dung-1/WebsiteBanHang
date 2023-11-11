using System.ComponentModel.DataAnnotations;

namespace WebsiteBanHang.Areas.Admin.Models
{
    public class CustomerModel
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
        public Customer_Details? CustomerDetail { get; set; } // Reference navigation to dependent
        public ICollection<CustomerRoleModel> CustomerRole { get; } = new List<CustomerRoleModel>(); // Collection navigation containing dependents
        public ICollection<OrdersModel> Order { get; } = new List<OrdersModel>(); // Collection navigation containing dependents

    }
}
