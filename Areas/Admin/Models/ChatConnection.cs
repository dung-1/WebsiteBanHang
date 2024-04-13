using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebsiteBanHang.Areas.Admin.Models
{
    public class ChatConnection
    {

        [Key]
        public string ConnectionId { get; set; } = Guid.NewGuid().ToString();

        public int UserId { get; set; } // Can be either customer or admin ID

        public bool Connected { get; set; } = true;

        public DateTime LastActive { get; set; } = DateTime.Now;

        // Relationship (consider using polymorphism)
        public User? User { get; set; } // Base class for CustomerModel and UserModel
    }
}
