using System.ComponentModel.DataAnnotations;

namespace WebsiteBanHang.Areas.Admin.Models
{
    public class UserModel : User
    {
        public ICollection<UserRoleModel> UserRole { get; } = new List<UserRoleModel>();
        public ICollection<OrdersModel> Order { get; } = new List<OrdersModel>();
        public Users_Details? userDetail { get; set; } = new Users_Details();
        public String? ChatConnectionId { get; set; }
        public ChatConnection? ChatConnection { get; set; }
        public ICollection<OrderCancellationModel> OrderCancellations { get; } = new List<OrderCancellationModel>();

    }
}
