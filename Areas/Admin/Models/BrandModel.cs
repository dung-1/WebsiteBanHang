﻿
using System.ComponentModel.DataAnnotations;

namespace WebsiteBanHang.Areas.Admin.Models
{
    public class BrandModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(10)]
        public string MaHang { get; set; }

        [Required]
        [StringLength(10)]
        public string TenHang { get; set; } // Thêm thuộc tính Tên Hãng

        [Required]
        [StringLength(20)]
        public string XuatXu { get; set; } // Thêm thuộc tính Xuất Xứ

        [Display(Name = "Ngày Xuất Xứ")]
        [DataType(DataType.Date)]
        public DateTime NgaySanXuat { get; set; } // Thêm thuộc tính Ngày Xuất Xứ

        [DataType(DataType.Date)]
        public DateTime CreatedTime { get; set; } // Thêm thuộc tính thời gian tạo

        [DataType(DataType.Date)]
        public DateTime ModifiedTime { get; set; } // Thêm thuộc tính thời gian cập nhật
        public ICollection<ProductModel> Prodcut { get; } = new List<ProductModel>(); // Collection navigation containing dependents
    }
}
