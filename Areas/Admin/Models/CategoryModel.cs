﻿using System.ComponentModel.DataAnnotations;

namespace WebsiteBanHang.Areas.Admin.Models
{
    public class CategoryModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(10)]
        public string MaLoai { get; set; }

        [Required]
        [StringLength(15)]
        public string ?TenLoai { get; set; }

        [DataType(DataType.Date)]
        public DateTime CreatedTime { get; set; } 

        [DataType(DataType.Date)]
        public DateTime ModifiedTime { get; set; } 
        public ICollection<ProductModel> Prodcut { get; } = new List<ProductModel>(); 

    }
}
