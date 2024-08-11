using System.ComponentModel.DataAnnotations;

namespace WebsiteBanHang.Areas.Admin.Models
{
    public class OrderCancellationModel
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        public int OrderId { get; set; }
        public OrdersModel? Order { get; set; }

        public int? AdminId { get; set; } 
        public UserModel? Admin { get; set; }

        public int? CustomerId { get; set; }
        public CustomerModel? Customer { get; set; }

        [Required]
        [StringLength(200)]
        public string? Reason { get; set; }

        [Required]
        public DateTime CancelledAt { get; set; }
    }

}
