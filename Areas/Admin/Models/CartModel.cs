using System.ComponentModel.DataAnnotations;
using WebsiteBanHang.Models;

namespace WebsiteBanHang.Areas.Admin.Models
{
    public class CartModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int CustomerId { get; set; }

        public CustomerModel Customer { get; set; }

        public ICollection<Cart_Item> CartItems { get; } = new List<Cart_Item>();
    }

}
