using System.ComponentModel.DataAnnotations;
using WebsiteBanHang.Areas.Admin.Common;

namespace WebsiteBanHang.Areas.Admin.Models
{
    public class CategoryPostModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(10)]
        public string MaTheLoai { get; set; }
        [Required]
        [StringLength(10)]
        public string Name { get; set; }
        public StatusActivity Status { get; set; }

        [DataType(DataType.Date)]
        public DateTime CreatedTime { get; set; } 

        [DataType(DataType.Date)]
        public DateTime ModifiedTime { get; set; }

        public ICollection<PostsModel> Posts { get; set; } = new List<PostsModel>();

    }
}
