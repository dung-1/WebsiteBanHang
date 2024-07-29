namespace WebsiteBanHang.Models
{
    public class ChatMessageViewModel
    {
        public string Content { get; set; }
        public DateTime SentAt { get; set; }
        public DateTime LastActive { get; set; }
        public int SenderId { get; set; }
        public string SenderName { get; set; } // Tên của người gửi
        public string SenderAvatar { get; set; } // URL ảnh đại diện của người gửi
        public bool IsRead { get; set; } // Trạng thái đọc tin nhắn
        public bool IsActive { get; set; } // Trạng thái hoạt động của người gửi
        public string MessageType { get; set; } // Loại tin nhắn (text, image, video, etc.)
        public bool IsAdminMessage { get; internal set; }
    }


}
