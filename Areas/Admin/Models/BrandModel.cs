using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;

namespace WebsiteBanHang.Areas.Admin.Models
{
    public class BrandModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(10)]
        public string maHang { get; set; }

        [Required]
        [StringLength(10)]
        public string tenHang { get; set; } // Thêm thuộc tính Tên Hãng

        [Required]
        [StringLength(20)]
        public string xuatXu { get; set; } // Thêm thuộc tính Xuất Xứ

        [Display(Name = "Ngày Xuất Xứ")]
        [DataType(DataType.Date)]
        public DateTime ngaySanXuat { get; set; } // Thêm thuộc tính Ngày Xuất Xứ
    }
}
