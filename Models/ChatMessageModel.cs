namespace WebsiteBanHang.Models
{
    public class ChatMessageModel
    {
        public string? SenderId { get; internal set; }
        public string? Message { get; internal set; }
        public DateTime SentAt { get; set; }
    }
}
