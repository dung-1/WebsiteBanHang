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
        public List<int> ViewCounts { get; set; } // Tổng lượt xem theo từng category
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
