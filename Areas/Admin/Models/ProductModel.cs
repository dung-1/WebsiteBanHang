using Microsoft.VisualBasic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing.Drawing2D;
using System.Reflection.Metadata;
using WebsiteBanHang.Areas.Admin.Common;
using WebsiteBanHang.Models;

namespace WebsiteBanHang.Areas.Admin.Models
{
    public class ProductModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(10)]
        public string? MaSanPham { get; set; }

        [Required]
        [StringLength(20)]
        public string? TenSanPham { get; set; }

        [Required]
        public int? HangId { get; set; }
        public BrandModel? Brand { get; set; } = null!;

        [Required]
        public int? LoaiId { get; set; }
        public CategoryModel? Category { get; set; } = null!;

        public decimal GiaNhap { get; set; }
        public decimal GiaBan { get; set; }
        public decimal GiaGiam { get; set; }

        [StringLength(255)]
        public string? Image { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(max)")]
        public string? ThongTinSanPham { get; set; }

        [DataType(DataType.Date)]
        public DateTime CreatedTime { get; set; }

        [DataType(DataType.Date)]
        public DateTime ModifiedTime { get; set; }
        public StatusActivity Status { get; set; }

        public ICollection<OrderDetaiModel> Order_Detai { get; } = new List<OrderDetaiModel>();
        public ICollection<InventoriesModel> Inventory { get; set; } = new List<InventoriesModel>();
        public ICollection<Cart_Item> CartItems { get; } = new List<Cart_Item>();

        // Thêm mối quan hệ với CommentModel
        public ICollection<CommentModel> Comments { get; set; } = new List<CommentModel>();
    }
}