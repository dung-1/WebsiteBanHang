using WebsiteBanHang.Areas.Admin.Models;
using Microsoft.EntityFrameworkCore;
using WebsiteBanHang.Models;
using Microsoft.Extensions.Hosting;

namespace WebsiteBanHang.Areas.Admin.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext>? options) : base(options)
        {
        }

        public DbSet<BrandModel> Brand { get; set; }
        public DbSet<CategoryModel> Category { get; set; }
        public DbSet<ProductModel> Product { get; set; }
        public DbSet<UserModel> User { get; set; }
        public DbSet<UserRoleModel> UserRole { get; set; }
        public DbSet<CustomerRoleModel> CustomerRole { get; set; }
        public DbSet<RoleModel> Role { get; set; }
        public DbSet<Users_Details> Users_Details { get; set; }
        public DbSet<OrderDetaiModel> Order_Detai { get; set; }
        public DbSet<CustomerModel> Customer { get; set; }
        public DbSet<Customer_Details> Customer_Details { get; set; }
        public DbSet<OrdersModel> Order { get; set; }
        public DbSet<InventoriesModel> Inventory { get; set; }
        public DbSet<PermissionRoleModel> PermissionRole { get; set; }
        public DbSet<PermissionsModel> Permissions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BrandModel>()
                .HasMany(e => e.Prodcut)
                .WithOne(e => e.Brand)
                .HasForeignKey(e => e.HangId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ProductModel>()
                  .HasOne(e => e.Brand)
                  .WithMany(e => e.Prodcut)
                  .HasForeignKey(e => e.HangId)
                  .IsRequired()
                  .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<CategoryModel>()
                .HasMany(e => e.Prodcut)
                .WithOne(e => e.Category)
                .HasForeignKey(e => e.LoaiId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<ProductModel>()
                 .HasOne(e => e.Category)
                 .WithMany(e => e.Prodcut)
                 .HasForeignKey(e => e.LoaiId)
                 .IsRequired()
                 .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserModel>()
               .HasMany(e => e.UserRole)
               .WithOne(e => e.User)
               .HasForeignKey(e => e.User_ID)
               .IsRequired()
               .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<CustomerModel>()
              .HasMany(e => e.CustomerRole)
              .WithOne(e => e.Customer)
              .HasForeignKey(e => e.Customer_ID)
              .IsRequired()
              .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<RoleModel>()
                .HasMany(e => e.UserRole)
                .WithOne(e => e.Role)
                .HasForeignKey(e => e.Role_ID)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<RoleModel>()
                .HasMany(e => e.CustomerRole)
                .WithOne(e => e.Role)
                .HasForeignKey(e => e.Role_ID)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ProductModel>()
                .HasMany(e => e.Order_Detai)
                .WithOne(e => e.product)
                .HasForeignKey(e => e.ProductId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<OrdersModel>()
               .HasMany(e => e.ctdh)
               .WithOne(e => e.order)
               .HasForeignKey(e => e.OrderId)
               .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ProductModel>()
                  .HasMany(e => e.Inventory)
                  .WithOne(e => e.product)
                  .HasForeignKey(e => e.ProductId)
                  .IsRequired()
                 .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<OrdersModel>()
                 .HasOne(o => o.user)
                 .WithMany(u => u.Order)
                 .HasForeignKey(o => o.UserID)
                 .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<OrdersModel>()
                .HasOne(o => o.Customer)
                .WithMany(c => c.Order)
                .HasForeignKey(o => o.CustomerID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<RoleModel>()
                .HasMany(e => e.PermissionRole)
                .WithOne(e => e.Role)
                .HasForeignKey(e => e.Role_ID)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PermissionsModel>()
                .HasMany(e => e.PermissionRole)
                .WithOne(e => e.Permission)
                .HasForeignKey(e => e.Permission_ID)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserModel>()
                 .HasOne(e => e.userDetail)
                 .WithOne(e => e.User)
                 .HasForeignKey<Users_Details>(e => e.UserId)
                 .IsRequired()
                 .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CustomerModel>()
                .HasOne(e => e.CustomerDetail)
                .WithOne(e => e.Customer)
                .HasForeignKey<Customer_Details>(e => e.CustomerId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserRoleModel>()
                .HasKey(ur => new { ur.User_ID, ur.Role_ID });

            modelBuilder.Entity<CustomerRoleModel>()
                .HasKey(ur => new { ur.Customer_ID, ur.Role_ID });

            modelBuilder.Entity<PermissionRoleModel>()
                .HasKey(ur => new { ur.Permission_ID, ur.Role_ID });

            modelBuilder.Entity<OrdersModel>()
                .Property(e => e.UserID)
                .IsRequired(false);
            modelBuilder.Entity<OrdersModel>()
                .Property(e => e.CustomerID)
                .IsRequired(false);

            modelBuilder.Entity<ProductModel>()
                .Property(p => p.GiaGiam)
                .HasColumnType("decimal(18,2)");
            modelBuilder.Entity<ProductModel>()
                .Property(p => p.Gia)
                .HasColumnType("decimal(18,2)");
        }
    }
}
