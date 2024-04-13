using System.ComponentModel.DataAnnotations;

namespace WebsiteBanHang.Areas.Admin.Models
{
    public class ChatMessage
    {

        [Key]
        public int Id { get; set; }

        public string? Content { get; set; } // Message content
        public DateTime SentAt { get; set; } = DateTime.Now;

        public string? ConnectionIdFrom { get; set; } // Foreign key for sender's ChatConnection

        public string? ConnectionIdTo { get; set; } // Foreign key for recipient's ChatConnection

        // Navigation properties (optional)
        public ChatConnection? FromConnection { get; set; }
        public ChatConnection? ToConnection { get; set; }
    }
}
