using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WebsiteBanHang.Areas.Admin.Models
{
    public class Customer_Details
    {
        [Key]
        public int CustomerId { get; set; }
        [StringLength(64)]
        public string? HoTen { get; set; }
        [StringLength(10)]
        public string? SoDienThoai { get; set; }
        [StringLength(64)]
        public string? DiaChi { get; set; }
        [ForeignKey("CustomerId")] // Đây là thuộc tính làm khóa ngoại
        public CustomerModel? Customer { get; set; } // Required reference navigation to principal

    }
}
