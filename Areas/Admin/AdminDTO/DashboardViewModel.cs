namespace WebsiteBanHang.Areas.Admin.AdminDTO
{
    public class DashboardViewModel
    {
        public List<string> RevenueLabels { get; set; }
        public List<decimal> RevenueData { get; set; }
        public List<string> Months { get; set; }
        public List<dynamic> ChartData { get; set; }
        public List<BrandInventoryDto> BrandInventories { get; set; }
        public List<string> CategoryLabels { get; set; } // Danh sách tên các category
        public List<int> PostCounts { get; set; } // Số lượng bài viết theo từng category
        // Thêm các thuộc tính để chứa dữ liệu hiển thị bài viết theo category có viewcount cao nhất
        public string SelectedCategory { get; set; } // Tên Category được chọn
        public List<string> PostNames { get; set; } // Danh sách tên các bài viết
        public List<int> PostViewCounts { get; set; } // Số lượt xem của từng bài viết
    }

    public class BrandInventoryDto
    {
        public string BrandName { get; set; }
        public List<ProductDetailDto> ProductDetails { get; set; }
    }

    public class ProductDetailDto
    {
        public string ProductName { get; set; }
        public int Quantity { get; set; }
    }
}
