using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebsiteBanHang.Areas.Admin.Models
{
    public class CommentModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ProductId { get; set; }
        public ProductModel Product { get; set; } = null!;

        [Required]
        public int UserId { get; set; }
        public User User { get; set; } = null!;

        [Required]
        [Column(TypeName = "nvarchar(max)")]
        public string Content { get; set; } = null!;

        [Required]
        [Range(1, 5)]
        public int Rating { get; set; }

        [Required]
        public DateTime CommentDate { get; set; }
    }
}
