namespace WebsiteBanHang.Models
{
    public class ChatMessageModel
    {
        public string? SenderId { get; internal set; }
        public string? Message { get; internal set; }
        public DateTime SentAt { get; set; }
        public string? ReceiverId { get; internal set; }
        public bool IsFromAdmin { get; internal set; }
    }
}
