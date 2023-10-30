using Microsoft.EntityFrameworkCore;
using WebsiteBanHang.Areas.Admin.Data;
using static WebsiteBanHang.Areas.Admin.Data.ApplicationDbContext;
using X.PagedList.Mvc.Core;
using Microsoft.AspNetCore.Identity;
using System.Security.Principal;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace WebsiteBanHang
{
    public class Program
    {
        public void ConfigureServices(IServiceCollection services)
        {
            // Các dịch vụ khác

            services.AddMvc();
        }
        public static void Main(string[] args)
        {

            var builder = WebApplication.CreateBuilder(args);
            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            // add
            builder.Services.AddDbContext<ApplicationDbContext>(
                options => options.UseSqlServer(
                    builder.Configuration.GetConnectionString("DefaultConnection")
                    ));

            builder.Services.AddDistributedMemoryCache();     // Đăng ký dịch vụ lưu cache trong bộ nhớ (Session sẽ sử dụng nó)
            builder.Services.AddSession(cfg =>
            {             // Đăng ký dịch vụ Session
                cfg.Cookie.Name = "dungCTS";             // Đặt tên Session - tên này sử dụng ở Browser (Cookie)
                cfg.IdleTimeout = new TimeSpan(0, 60, 0);  // Thời gian tồn tại của Session
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSession();
            app.UseRouting();
            app.UseAuthorization();
            app.UseAuthentication();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                  name: "areas",
                  pattern: "{area:exists}/{controller=AdminHome}/{action=Index}/{id?}"
                );

                app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Account}/{action=Index}/{id?}");
            });
            IdentityUser user;
            IdentityDbContext identityDbContext;

            app.Run();
        }
    }
}