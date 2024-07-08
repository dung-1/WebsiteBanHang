using Microsoft.ML.Data;

namespace WebsiteBanHang.Areas.Admin.AdminDTO
{
    public class ModelInput
    {
        [ColumnName("input")]
        public string Comment { get; set; }
    }
}
