using WebsiteBanHang.Areas.Admin.Models;

namespace WebsiteBanHang.Models
{
    public class CartItemModel
    {
        public int ProductId { get; set; }

        public int Soluong { set; get; }
        public ProductModel Product { set; get; }
    }
}
