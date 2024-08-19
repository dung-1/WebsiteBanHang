using Microsoft.AspNetCore.Authentication.Cookies;
using WebsiteBanHang.Areas.Admin.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Razor;
using WebsiteBanHang.Models;
using System.Reflection;
using WebsiteBanHang.HubSignalR;
using OfficeOpenXml;
using LicenseContext = OfficeOpenXml.LicenseContext;
using WebsiteBanHang.Areas.Admin.Controllers;
using WebsiteBanHang.Areas.Admin.Common;
using WebsiteBanHang.Service;
using Google.Cloud.Dialogflow.V2;
using WebsiteBanHang.Middleware;

namespace WebsiteBanHang
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews()
                .AddRazorOptions(options =>
                {
                    options.ViewLocationFormats.Add("/{0}.cshtml");
                });

            builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            // Cấu hình DbContext và session SQL Server
            var connectionString = builder.Configuration.GetConnectionString("SqlServerSession");

            builder.Services.AddDbContext<ApplicationDbContext>(o =>
                o.UseSqlServer(connectionString, sqlServerOptionsAction: sqlOption =>
                {
                    sqlOption.EnableRetryOnFailure();
                }));

            builder.Services.AddDistributedSqlServerCache(options =>
            {
                options.ConnectionString = builder.Configuration.GetConnectionString("SqlServerSession");
                options.SchemaName = "dbo";
                options.TableName = "Sessions";
            });

            // Cấu hình session với tên khác biệt
            builder.Services.AddSession(options =>
            {
                options.Cookie.Name = "SessionCTS";
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            // Cấu hình Authentication với tên khác biệt
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.Cookie.Name = "AuthCTS";
                    options.LoginPath = "/Account/Login";
                    options.AccessDeniedPath = "/Account/AccessDenied";
                });

            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
                options.AddPolicy("EmployeeOnly", policy => policy.RequireRole("Employee"));
            });

            // Cấu hình localization
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

            // Cấu hình EPPlus
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            builder.Services.AddTransient<IReporting, ReportingConcrete>();
            builder.Services.AddTransient<AdminHomeController>();

            // Tích hợp đa ngôn ngữ
            builder.Services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = new[] { "vi-VN", "en-US" };
                options.SetDefaultCulture(supportedCultures[1])
                    .AddSupportedCultures(supportedCultures)
                    .AddSupportedUICultures(supportedCultures);

                var questStringCultureProvider = options.RequestCultureProviders[0];
                options.RequestCultureProviders.RemoveAt(0);
                options.RequestCultureProviders.Insert(1, questStringCultureProvider);
            });

            // Đăng ký SignalR
            builder.Services.AddSignalR();

            // Cấu hình cho Stripe
            builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("StripeSettings"));

            // Cấu hình Dialogflow
            var jsonPath = builder.Configuration["Google:JsonPath"];
            builder.Services.AddSingleton<DialogflowService>(sp =>
            {
                var projectId = builder.Configuration["Google:ProjectId"];
                return new DialogflowService(projectId, jsonPath);
            });

            // Add NewtonsoftJson
            builder.Services.AddControllers().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            });

            // Add Razor runtime compilation
            builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();

            var app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseStatusCodePagesWithReExecute("/StatusCode/{0}");
            app.UseRequestLocalization();
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseCors(builder => builder
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseSession();
            app.UseMiddleware<SessionMiddleware>();

            // Map routes
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapAreaControllerRoute(
                    name: "adminArea",
                    areaName: "Admin",
                    pattern: "admin/{controller=Home}/{action=Index}/{id?}");

                endpoints.MapAreaControllerRoute(
                    name: "loginArea",
                    areaName: "Login",
                    pattern: "login/{controller=Account}/{action=Index}/{id?}");

                endpoints.MapControllerRoute(
                    name: "category",
                    pattern: "Category/Index/{categoryid?}",
                    defaults: new { controller = "Category", action = "Index" });

                endpoints.MapHub<ChatHub>("/chathub");
                endpoints.MapHub<NotificationHub>("/notificationHub");

                // Default route
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });

            app.Run();
        }
    }
}
