﻿using System.ComponentModel.DataAnnotations;
using WebsiteBanHang.Areas.Admin.Common;

namespace WebsiteBanHang.Areas.Admin.AdminDTO
{
    public class ProductViewDTO
    {
        public int Id { get; set; }
        public string? MaSanPham { get; set; }
        public string? TenSanPham { get; set; }
        public string? HangTen { get; set; } // Tên của Brand
        public string? LoaiTen { get; set; }
        public decimal GiaNhap { get; set; }
        public decimal GiaBan { get; set; }
        public decimal GiaGiam { get; set; }
        public string? Image { get; set; }
        public string? ThongTinSanPham { get; set; }
        public StatusActivity Status { get; set; }
        [Display(Name = "Trạng thái")]
        public string StatusString { get => Status.GetDescription(); }
        public List<CommentDTO> Comments { get; set; }


        // Checkboxes for filtering
        public bool CheckboxAll { get; set; }
        public bool CheckboxPrice1 { get; set; }
        public bool CheckboxPrice2 { get; set; }
        public bool CheckboxPrice3 { get; set; }
        public bool CheckboxPrice4 { get; set; }
        public bool CheckboxPrice5 { get; set; }
        public List<ProductDTO> RelatedProducts { get; set; }
        public List<InfoNumberProductDTO> RelatedInfoNumberProduct { get; set; }


        public class InfoNumberProductDTO
        {
            public int Rating { get; set; }
            public int Comment { get; set; }
            public int ProductBuyNumber { get; set; }

        }
        public class ProductDTO
        {
            public int Id { get; set; }
            public string? MaSanPham { get; set; }
            public string? TenSanPham { get; set; }
            public int SoLuong { get; set; }
            public decimal GiaNhap { get; set; }
            public decimal GiaBan { get; set; }
            public decimal GiaGiam { get; set; }
            public string? Image { get; set; }
        }
        public class CommentDTO
        {
            public string UserName { get; set; }
            public string Content { get; set; }
            public int Rating { get; set; }
            public DateTime CommentDate { get; set; }
        }

    }
}
