using Microsoft.AspNetCore.Authentication.Cookies;
using WebsiteBanHang.Areas.Admin.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Options;
using WebsiteBanHang.Models;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace WebsiteBanHang
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
            );

            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(cfg =>
            {
                cfg.Cookie.Name = "dungCTS";
                cfg.IdleTimeout = new TimeSpan(0, 60, 0);
            });
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);

            });

            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.Cookie.Name = "DungCTS";
                    options.LoginPath = "/Account/Login";
                    options.AccessDeniedPath = "/Account/AccessDenied";
                });
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
                options.AddPolicy("EmployeeOnly", policy => policy.RequireRole("Employee"));
            });
            builder.Services.AddControllersWithViews().AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix);
            builder.Services.AddSingleton<LanguageService>();
            builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
            builder.Services.AddMvc()
            .AddViewLocalization()
            .AddDataAnnotationsLocalization(options =>
            {
                options.DataAnnotationLocalizerProvider = (type, factory) =>
                {
                    var assemblyName = new AssemblyName(type.GetTypeInfo().Assembly.FullName);
                    return factory.Create("SharedResource", assemblyName.Name);
                };
            });

         
            builder.Services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = new[] { "vi-VN", "en-US" };
                options.SetDefaultCulture(supportedCultures[1])
                    .AddSupportedCultures(supportedCultures)
                    .AddSupportedUICultures(supportedCultures);
                var questStringCultureProvider = options.RequestCultureProviders[0];
                options.RequestCultureProviders.RemoveAt(0);
                options.RequestCultureProviders.Insert(1, questStringCultureProvider);
                //Add services to the container.
            });
            var app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            app.UseRequestLocalization();

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSession();
            app.UseRouting();
            app.UseAuthentication(); 
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                // Đặt route của Admin area lên trước với tiền tố "admin"
                endpoints.MapAreaControllerRoute(
                    name: "adminArea",
                    areaName: "Admin",
                    pattern: "admin/{controller=Home}/{action=Index}/{id?}"
                );

                // Sau đó đặt route của Login area với tiền tố "login"
                endpoints.MapAreaControllerRoute(
                    name: "loginArea",
                    areaName: "Login",
                    pattern: "login/{controller=Account}/{action=Index}/{id?}"
                );
                endpoints.MapControllerRoute(
                   name: "category",
                   pattern: "Category/Index/{categoryid?}",
                   defaults: new { controller = "Category", action = "Index" });

                // Cuối cùng, đặt route mặc định cho Home controller
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}"
                );

            });

            app.Run();
        }
    }
}
