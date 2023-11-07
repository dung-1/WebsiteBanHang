using System.ComponentModel.DataAnnotations;
using WebsiteBanHang.Areas.Admin.Models;

namespace WebsiteBanHang.Models
{
    public class OrderModel
    {

        [Key]
        [Required]
        public int id { get; set; }

        [Required]
        [StringLength(10)]
        public string? MaHoaDon { get; set; }
        public int? UserID { get; set; } // Required foreign key property
        public UserModel user { get; set; } = null!;
        public DateTime ngayBan { get; set; }
        public float tongTien { get; set; }
        [StringLength(20)]
        public string? trangThai { get; set; }
        [StringLength(20)]
        public string? LoaiHoaDon { get; set; }
        public ICollection<OrderDetaiModel> ctdh { get; } = new List<OrderDetaiModel>();

    }
}
