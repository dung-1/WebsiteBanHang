using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebsiteBanHang.Areas.Admin.Models
{
    public class ProductModel
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(10)]
        public string maSanPham { get; set; }
        [Required]
        [StringLength(20)]
        public string tenSanPham { get; set; }
        [Required]
        [ForeignKey("BrandModel")]
        public virtual BrandModel Hang { get; set; }

        [Required]
        [ForeignKey("CategoryModel")]
        public virtual CategoryModel Loai { get; set; }
        [Required]
        [RegularExpression(@"^\d{1,9}(\.\d{1,2})?$", ErrorMessage = "Giá không hợp lệ.")]
        [DataType(DataType.Currency)]
        public decimal gia { get; set; }
        [StringLength(255)]
        public string? image { get; set; }

        [Required]
        [StringLength(100)]
        public string thongTinSanPham { get; set; }
    }
}
