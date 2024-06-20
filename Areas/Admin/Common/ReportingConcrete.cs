
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using WebsiteBanHang.Areas.Admin.Models;
using WebsiteBanHang.Areas.Admin.Data;
using WebsiteBanHang.Areas.Admin.AdminDTO;
using System.Text.RegularExpressions;
namespace WebsiteBanHang.Areas.Admin.Common
{
    public class ReportingConcrete : IReporting
    {
        private readonly ApplicationDbContext _databaseContext;
        private readonly IConfiguration _configuration;
        public ReportingConcrete(ApplicationDbContext databaseContext, IConfiguration configuration)
        {
            _databaseContext = databaseContext;
            _configuration = configuration;
        }



        public List<Category_exrepoting_Dto> GetCategorywiseReport()
        {
            try
            {
                var listcategory = (from usermaster in _databaseContext.Category.AsNoTracking()
                                    select new Category_exrepoting_Dto()
                                    {
                                        Id = usermaster.Id,
                                        MaLoai = usermaster.MaLoai,
                                        TenLoai = usermaster.TenLoai,

                                    }).ToList();

                return listcategory;
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        public List<Brand_exrepoting_Dto> GetBrandwiseReport()
        {
            try
            {
                var listbrand = (from usermaster in _databaseContext.Brand.AsNoTracking()
                                 select new Brand_exrepoting_Dto()
                                 {
                                     Id = usermaster.Id,
                                     MaHang = usermaster.MaHang,
                                     TenHang = usermaster.TenHang,
                                     XuatXu = usermaster.XuatXu,
                                     NgaySanXuat = usermaster.NgaySanXuat

                                 }).ToList();

                return listbrand;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public List<Product_exrepoting_Dto> GetProductwiseReport()
        {
            try
            {
                var listproduct = (from usermaster in _databaseContext.Product.AsNoTracking()
                                   select new Product_exrepoting_Dto()
                                   {
                                       Id = usermaster.Id,
                                       MaSanPham = usermaster.MaSanPham,
                                       TenSanPham = usermaster.TenSanPham,
                                       HangTen = usermaster.Brand.TenHang,
                                       LoaiTen = usermaster.Category.TenLoai,
                                       GiaNhap = usermaster.GiaNhap,
                                       GiaBan = usermaster.GiaBan,
                                       Image=usermaster.Image,
                                       GiaGiam = usermaster.GiaGiam,
                                       ThongTinSanPham= Regex.Replace(usermaster.ThongTinSanPham, @"<[^>]*>", string.Empty), // Loại bỏ các thẻ HTML

                                   }).ToList();

                return listproduct;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

    }
}