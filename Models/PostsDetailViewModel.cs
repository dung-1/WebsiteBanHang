using WebsiteBanHang.Areas.Admin.AdminDTO;

namespace WebsiteBanHang.Models
{
    public class PostsDetailViewModel
    {
        public PostsViewDto Post { get; set; }
        public List<HighlightedPostDto> TopHighlightedPosts { get; set; }
        public List<PromotionalPostDto> TopPromotionalPosts { get; set; }
        public PromotionalPostDto LatestPromotionalPost { get; set; }
    }

}
