using System.ComponentModel.DataAnnotations;

namespace WebsiteBanHang.Areas.Admin.Models
{
    public class OrdersModel
    {

        [Key]
        [Required]
        public int id { get; set; }
        [Required]
        [StringLength(10)]
        public string? MaHoaDon { get; set; }
        public int? UserID { get; set; }
        public UserModel? user { get; set; }
        public int? CustomerID { get; set; }
        public CustomerModel? Customer { get; set; }
        public DateTime ngayBan { get; set; }
        public DateTime NgayGiaoHang { get; set; }
        public float tongTien { get; set; }
        [StringLength(20)]
        public string? trangThai { get; set; }
        [StringLength(50)]
        public string? LoaiHoaDon { get; set; }
        public ICollection<OrderDetaiModel> ctdh { get; } = new List<OrderDetaiModel>();
        public ICollection<OrderCancellationModel> OrderCancellations { get; } = new List<OrderCancellationModel>();
    }
}
