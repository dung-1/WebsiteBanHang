using System.ComponentModel.DataAnnotations;

namespace WebsiteBanHang.Areas.Admin.Models
{
    public class CustomerModel : User
    {
        public ICollection<CustomerRoleModel> CustomerRole { get; } = new List<CustomerRoleModel>(); // Collection navigation containing dependents
        public ICollection<OrdersModel> Order { get; } = new List<OrdersModel>(); 
        public Customer_Details? CustomerDetail { get; set; } = new Customer_Details(); 
        public ICollection<CartModel> Carts { get; } = new List<CartModel>();
        public string? ChatConnectionId { get; set; }
        public ChatConnection? ChatConnection { get; set; }
        public ICollection<OrderCancellationModel> OrderCancellations { get; } = new List<OrderCancellationModel>();


    }
}
