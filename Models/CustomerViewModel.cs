namespace WebsiteBanHang.Models
{
    public class CustomerViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string LastMessage { get; set; }
        public DateTime LastMessageTimeAgo { get; set; }
        public string LastMessageTimeAgoFormatted { get; internal set; }
        public DateTime LastActive { get; set; }
        public bool IsRead { get; set; } // Trạng thái đọc tin nhắn
        public bool IsActive { get; set; } // Trạng thái hoạt động của người gửi
    }

}
