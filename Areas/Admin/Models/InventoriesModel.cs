using System.ComponentModel.DataAnnotations;

namespace WebsiteBanHang.Areas.Admin.Models
{
    public class InventoriesModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(10)]
        public string? MaKho { get; set; }
        public int ProductId { get; set; } // Required foreign key property
        public ProductModel product { get; set; } = null!;
        public DateTime? NgayNhap { get; set; }
        public int SoLuong { get; set; }

        [DataType(DataType.Date)]
        public DateTime CreatedTime { get; set; } // Thêm thuộc tính thời gian tạo

        [DataType(DataType.Date)]
        public DateTime ModifiedTime { get; set; } // Thêm thuộc tính thời gian cập nhật
    }
}
