using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection.Metadata;

namespace WebsiteBanHang.Areas.Admin.Models
{
    public class Users_Details
    {
        [Key]
        public int UserId { get; set; }
        [StringLength(64)]
        public string ?HoTen { get; set; }
        public int SoDienThoai { get; set; }
        [StringLength(64)]
        public string DiaChi { get; set; }
        [ForeignKey("UserId")] // Đây là thuộc tính làm khóa ngoại
        public UserModel User { get; set; } = null!; // Required reference navigation to principal

    }
}
