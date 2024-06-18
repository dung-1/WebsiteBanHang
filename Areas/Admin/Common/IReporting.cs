using WebsiteBanHang.Areas.Admin.Models;
using System.Collections.Generic;
using WebsiteBanHang.Areas.Admin.AdminDTO;

namespace WebsiteBanHang.Areas.Admin.Common
{
    public interface IReporting
    {
        List<Category_exrepoting_Dto> GetCategorywiseReport();
        List<Brand_exrepoting_Dto> GetBrandwiseReport();
        List<Product_exrepoting_Dto> GetProductwiseReport();
    }
}
