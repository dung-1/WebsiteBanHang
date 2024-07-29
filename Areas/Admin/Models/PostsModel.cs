using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using WebsiteBanHang.Areas.Admin.Common;

namespace WebsiteBanHang.Areas.Admin.Models
{
    public class PostsModel
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(10)]
        public string MaBaiViet { get; set; }
        public string? CreatedBy { get; set; }

        [DataType(DataType.Date)]
        public DateTime CreatedTime { get; set; } 

        [DataType(DataType.Date)]
        public DateTime ModifiedTime { get; set; } 
        
        public string Title { get; set; }

        public int CategoryId { get; set; }

        public string? ExcerptImage { get; set; }

        public string? Content { get; set; }

        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }

        public StatusActivity Status { get; set; }

        [Display(Name = "Lượt xem")]
        public int ViewCount { get; set; }

        [ForeignKey("CategoryId")]
        public CategoryPostModel Category { get; set; }
    }
}
