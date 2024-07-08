using Microsoft.ML.Data;

namespace WebsiteBanHang.Areas.Admin.AdminDTO
{
    public class ModelOutput
    {
        [ColumnName("output_label")]
        public bool PredictedLabel { get; set; }
    }
}
