using System.ComponentModel.DataAnnotations;

namespace WebsiteBanHang.Areas.Admin.Models
{
    public class CategoryModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(10)]
        public string maLoai { get; set; }

        [Required]
        [StringLength(15)]
        public string tenLoai { get; set; }
    }
}
