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
        public DbSet<Cart_Item> Cart_Item { get; set; }
        public DbSet<CartModel> CartModel { get; set; }
        public DbSet<ChatConnection> ChatConnection { get; set; }
        public DbSet<ChatMessage> ChatMessage { get; set; }
        public DbSet<Session> Sessions { get; set; }
        public DbSet<CommentModel> Comments { get; set; }
        public DbSet<PostsModel> Posts { get; set; }
        public DbSet<CategoryPostModel> CategoryPost { get; set; }
        public DbSet<OrderCancellationModel> OrderCancel { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Session>(entity =>
            {
                entity.ToTable("Sessions");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Value).IsRequired();
                entity.Property(e => e.ExpiresAtTime).IsRequired();
            });

            modelBuilder.Entity<CategoryPostModel>()
               .HasMany(e => e.Posts)
               .WithOne(e => e.Category)
               .HasForeignKey(e => e.CategoryId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<PostsModel>()
               .HasOne(p => p.Category)
               .WithMany(c => c.Posts)
               .HasForeignKey(p => p.CategoryId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CustomerModel>()
               .HasMany(e => e.Carts)
               .WithOne(e => e.Customer)
               .HasForeignKey(e => e.CustomerId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CartModel>()
                  .HasMany(c => c.CartItems)
                  .WithOne(ci => ci.Cart)
                  .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ProductModel>()
                .HasMany(e => e.CartItems)
                .WithOne(e => e.Product)
                .HasForeignKey(e => e.ProductId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

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

            modelBuilder.Entity<ChatConnection>()
              .HasOne(cc => cc.User)
              .WithMany()
              .HasForeignKey(cc => cc.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ChatMessage>()
                .HasOne(cm => cm.FromConnection)
                .WithMany()
                .HasForeignKey(cm => cm.ConnectionIdFrom)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ChatMessage>()
                .HasOne(cm => cm.ToConnection)
                .WithMany()
                .HasForeignKey(cm => cm.ConnectionIdTo)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ProductModel>()
                .HasMany(p => p.Comments)
                .WithOne(c => c.Product)
                .HasForeignKey(c => c.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasMany(u => u.Comments)
                .WithOne(c => c.User)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<OrdersModel>()
                .HasMany(o => o.OrderCancellations)
                .WithOne(oc => oc.Order)
                .HasForeignKey(oc => oc.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserModel>()
                .HasMany(u => u.OrderCancellations)
                .WithOne(oc => oc.Admin)
                .HasForeignKey(oc => oc.AdminId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CustomerModel>()
                .HasMany(c => c.OrderCancellations)
                .WithOne(oc => oc.Customer)
                .HasForeignKey(oc => oc.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<UserRoleModel>()
                    .HasKey(ur => new
                    {
                        ur.User_ID,
                        ur.Role_ID
                    });

            modelBuilder.Entity<CustomerRoleModel>()
                .HasKey(ur => new
                {
                    ur.Customer_ID,
                    ur.Role_ID
                });

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
                .Property(p => p.GiaBan)
                .HasColumnType("decimal(18,2)");
            modelBuilder.Entity<ProductModel>()
                .Property(p => p.GiaNhap)
                .HasColumnType("decimal(18,2)");
            base.OnModelCreating(modelBuilder);

        }
    }
}
