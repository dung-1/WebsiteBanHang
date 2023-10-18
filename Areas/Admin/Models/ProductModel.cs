using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing.Drawing2D;
using System.Reflection.Metadata;

namespace WebsiteBanHang.Areas.Admin.Models
{
    public class ProductModel
    {

        public int Id { get; set; }
        [Required]
        [StringLength(10)]
        public string MaSanPham { get; set; }
        [Required]
        [StringLength(20)]
        public string TenSanPham { get; set; }
        [Required]

        public int? HangId { get; set; } // Required foreign key property
        public BrandModel Brand { get; set; } = null!;

        [Required]

        public int? LoaiId { get; set; } // Required foreign key property
        public CategoryModel Category { get; set; } = null!;

        [Required]
        [RegularExpression(@"^\d{1,9}(\.\d{1,2})?$", ErrorMessage = "Giá không hợp lệ.")]
        [DataType(DataType.Currency)]
        public decimal Gia { get; set; }
        [StringLength(255)]
        public string? Image { get; set; }

        [Required]
        [StringLength(100)]
        public string ThongTinSanPham { get; set; }



    }
}