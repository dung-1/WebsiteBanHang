namespace WebsiteBanHang.Areas.Admin.AdminDTO
{
    public class DashboardViewModel
    {
        public StatisticsViewDto Statistics { get; set; }
        public List<string> RevenueLabels { get; set; }
        public List<decimal> RevenueData { get; set; }
        public List<string> Months { get; set; }
        public List<dynamic> ChartData { get; set; }

        // New properties for inventory statistics
        public List<InventoryCategoryPercentageDto> InventoryCategoryPercentages { get; set; }
        public List<ProductInventoryDto> ProductInventories { get; set; }
    }

    public class InventoryCategoryPercentageDto
    {
        public string CategoryName { get; set; }
        public decimal Percentage { get; set; }
    }

    public class ProductInventoryDto
    {
        public string ProductName { get; set; }
        public int Quantity { get; set; }
    }



}
