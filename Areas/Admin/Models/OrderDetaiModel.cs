using System.ComponentModel.DataAnnotations;
using WebsiteBanHang.Models;

namespace WebsiteBanHang.Areas.Admin.Models
{
    public class OrderDetaiModel
    {
        [Key]
        public int ID { get; set; }
        [Required]
        public int OrderId { get; set; } // Required foreign key property
        public OrderModel order { get; set; } = null!;

        public int ProductId { get; set; } // Required foreign key property
        public ProductModel product { get; set; } = null!;
        public int soLuong { get; set; }
        public float gia { get; set; }
    }
}
