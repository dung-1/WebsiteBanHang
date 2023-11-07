using WebsiteBanHang.Areas.Admin.Models;
using Microsoft.EntityFrameworkCore;
using WebsiteBanHang.Models;

namespace WebsiteBanHang.Areas.Admin.Data
{
    public class ApplicationDbContext : DbContext
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {

        }

        public DbSet<BrandModel> Brand { get; set; }
        public DbSet<CategoryModel> Category { get; set; }
        public DbSet<ProductModel> Product { get; set; }
        public DbSet<UserModel> User { get; set; }
        public DbSet<UserRoleModel> UserRole { get; set; }
        public DbSet<RoleModel> Role { get; set; }
        public DbSet<Users_Details> Users_Details { get; set; }
        public DbSet<OrderDetaiModel> Order_Detai { get; set; }
        public DbSet<OrderModel> Order { get; set; }
        public DbSet<InventoriesModel> Inventory { get; set; }
        public DbSet<PermissionRoleModel> PermissionRole { get; set; }
        public DbSet<PermissionsModel> Permissions { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BrandModel>()
                .HasMany(e => e.Prodcut)
                .WithOne(e => e.Brand)
                .HasForeignKey(e => e.HangId)
                .IsRequired();

            modelBuilder.Entity<CategoryModel>()
                .HasMany(e => e.Prodcut)
                .WithOne(e => e.Category)
                .HasForeignKey(e => e.LoaiId)
                .IsRequired();

            modelBuilder.Entity<UserModel>()
               .HasMany(e => e.UserRole)
               .WithOne(e => e.User)
               .HasForeignKey(e => e.User_ID)
               .IsRequired();

            modelBuilder.Entity<RoleModel>()
                .HasMany(e => e.UserRole)
                .WithOne(e => e.Role)
                .HasForeignKey(e => e.Role_ID)
                .IsRequired();

            modelBuilder.Entity<ProductModel>()
                .HasMany(e => e.Order_Detai)
                .WithOne(e => e.product)
                .HasForeignKey(e => e.ProductId)
                .IsRequired();

            modelBuilder.Entity<OrderModel>()
               .HasMany(e => e.ctdh)
               .WithOne(e => e.order)
               .HasForeignKey(e => e.OrderId)
               .IsRequired();

            modelBuilder.Entity<ProductModel>()
                  .HasMany(e => e.Inventory)
                  .WithOne(e => e.product)
                  .HasForeignKey(e => e.ProductId)
                  .IsRequired();

                modelBuilder.Entity<UserModel>()
               .HasMany(e => e.Order)
               .WithOne(e => e.user)
               .HasForeignKey(e => e.UserID)
               .IsRequired();

            modelBuilder.Entity<RoleModel>()
                .HasMany(e => e.PermissionRole)
                .WithOne(e => e.Role)
                .HasForeignKey(e => e.Role_ID)
                .IsRequired();

            modelBuilder.Entity<PermissionsModel>()
                .HasMany(e => e.PermissionRole)
                .WithOne(e => e.Permission)
                .HasForeignKey(e => e.Permission_ID)
                .IsRequired();

            modelBuilder.Entity<UserModel>()
               .HasOne(e => e.userDetail)
               .WithOne(e => e.User)
               .HasForeignKey<Users_Details>(e => e.UserId)
               .IsRequired();
            modelBuilder.Entity<UserRoleModel>()
              .HasKey(ur => new { ur.User_ID, ur.Role_ID });
            modelBuilder.Entity<PermissionRoleModel>()
             .HasKey(ur => new { ur.Permission_ID, ur.Role_ID });
            modelBuilder.Entity<OrderModel>().Property(e => e.UserID).IsRequired(false);

        }
    }
}