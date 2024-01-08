using System.ComponentModel.DataAnnotations;

namespace WebsiteBanHang.Areas.Admin.Models
{
    public class Cart_Item
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int CartId { get; set; }

        public CartModel Cart { get; set; }

        [Required]
        public int ProductId { get; set; }

        public ProductModel Product { get; set; }

        public int Quantity { get; set; }

    }

}
