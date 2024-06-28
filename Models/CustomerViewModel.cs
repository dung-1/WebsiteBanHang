namespace WebsiteBanHang.Models
{
    public class CustomerViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string LastMessage { get; set; }
        public DateTime LastMessageTimeAgo { get; set; }
        public string LastMessageTimeAgoFormatted { get; internal set; }
    }

}
