using System.ComponentModel.DataAnnotations;
using WebsiteBanHang.Areas.Admin.Common;

namespace WebsiteBanHang.Areas.Admin.AdminDTO
{
    public class PostsViewDto

    {
        public int Id { get; set; }
        [DataType(DataType.Date)]
        public DateTime CreatedTime { get; set; }

        [Display(Name = "Tiêu đề")]
        public string Title { get; set; }
        [Display(Name = "Mô tả trích đoạn")]
        public string Excerpt { get; set; }
        [Display(Name = "Nội dung trích đoạn")]
        public string? Content { get; set; }
        [Display(Name = "Hình ảnh trích đoạn")]
        public string? ExcerptImage { get; set; }
        [Display(Name = "Danh mục")]
        public string CategoryName { get; set; }
        [Display(Name = "Lượt truy cập")]
        public int? ViewCount { get; set; }
        [Display(Name = "Thời gian mở")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? FromDate { get; set; }
        [Display(Name = "Thời gian đóng")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? ToDate { get; set; }
        [Display(Name = "Trạng thái")]
        public StatusActivity Status { get; set; }
        [Display(Name = "Trạng thái")]
        public string StatusString { get => Status.GetDescription(); }

    }
}
