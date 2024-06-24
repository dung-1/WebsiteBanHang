﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using WebsiteBanHang.Areas.Admin.Data;

#nullable disable

namespace WebsiteBanHang.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20240621085159_update_message")]
    partial class update_message
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("WebsiteBanHang.Areas.Admin.Models.BrandModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("MaHang")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar(10)");

                    b.Property<DateTime>("NgaySanXuat")
                        .HasColumnType("datetime2");

                    b.Property<string>("TenHang")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar(10)");

                    b.Property<string>("XuatXu")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.HasKey("Id");

                    b.ToTable("Brand");
                });

            modelBuilder.Entity("WebsiteBanHang.Areas.Admin.Models.Cart_Item", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<int>("CartId")
                        .HasColumnType("int");

                    b.Property<int>("ProductId")
                        .HasColumnType("int");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("CartId");

                    b.HasIndex("ProductId");

                    b.ToTable("Cart_Item");
                });

            modelBuilder.Entity("WebsiteBanHang.Areas.Admin.Models.CartModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<int>("CustomerId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("CustomerId");

                    b.ToTable("CartModel");
                });

            modelBuilder.Entity("WebsiteBanHang.Areas.Admin.Models.CategoryModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("MaLoai")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar(10)");

                    b.Property<string>("TenLoai")
                        .IsRequired()
                        .HasMaxLength(15)
                        .HasColumnType("nvarchar(15)");

                    b.HasKey("Id");

                    b.ToTable("Category");
                });

            modelBuilder.Entity("WebsiteBanHang.Areas.Admin.Models.ChatConnection", b =>
                {
                    b.Property<string>("ConnectionId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<bool>("Connected")
                        .HasColumnType("bit");

                    b.Property<DateTime>("LastActive")
                        .HasColumnType("datetime2");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("ConnectionId");

                    b.HasIndex("UserId");

                    b.ToTable("ChatConnection");
                });

            modelBuilder.Entity("WebsiteBanHang.Areas.Admin.Models.ChatMessage", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("ChatConnectionConnectionId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ConnectionIdFrom")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ConnectionIdTo")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Content")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("SentAt")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("ChatConnectionConnectionId");

                    b.HasIndex("ConnectionIdFrom");

                    b.HasIndex("ConnectionIdTo");

                    b.ToTable("ChatMessage");
                });

            modelBuilder.Entity("WebsiteBanHang.Areas.Admin.Models.Customer_Details", b =>
                {
                    b.Property<int>("CustomerId")
                        .HasColumnType("int");

                    b.Property<string>("DiaChi")
                        .HasMaxLength(64)
                        .HasColumnType("nvarchar(64)");

                    b.Property<string>("HoTen")
                        .HasMaxLength(64)
                        .HasColumnType("nvarchar(64)");

                    b.Property<string>("SoDienThoai")
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar(10)");

                    b.HasKey("CustomerId");

                    b.ToTable("Customer_Details");
                });

            modelBuilder.Entity("WebsiteBanHang.Areas.Admin.Models.CustomerRoleModel", b =>
                {
                    b.Property<int>("Customer_ID")
                        .HasColumnType("int")
                        .HasColumnOrder(0);

                    b.Property<int>("Role_ID")
                        .HasColumnType("int")
                        .HasColumnOrder(1);

                    b.HasKey("Customer_ID", "Role_ID");

                    b.HasIndex("Role_ID");

                    b.ToTable("CustomerRole");
                });

            modelBuilder.Entity("WebsiteBanHang.Areas.Admin.Models.InventoriesModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("MaKho")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar(10)");

                    b.Property<DateTime?>("NgayNhap")
                        .HasColumnType("datetime2");

                    b.Property<int>("ProductId")
                        .HasColumnType("int");

                    b.Property<int>("SoLuong")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ProductId");

                    b.ToTable("Inventory");
                });

            modelBuilder.Entity("WebsiteBanHang.Areas.Admin.Models.OrderDetaiModel", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ID"), 1L, 1);

                    b.Property<int>("OrderId")
                        .HasColumnType("int");

                    b.Property<int>("ProductId")
                        .HasColumnType("int");

                    b.Property<float>("gia")
                        .HasColumnType("real");

                    b.Property<int>("soLuong")
                        .HasColumnType("int");

                    b.HasKey("ID");

                    b.HasIndex("OrderId");

                    b.HasIndex("ProductId");

                    b.ToTable("Order_Detai");
                });

            modelBuilder.Entity("WebsiteBanHang.Areas.Admin.Models.OrdersModel", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("id"), 1L, 1);

                    b.Property<int?>("CustomerID")
                        .HasColumnType("int");

                    b.Property<string>("LoaiHoaDon")
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<string>("MaHoaDon")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar(10)");

                    b.Property<int?>("UserID")
                        .HasColumnType("int");

                    b.Property<DateTime>("ngayBan")
                        .HasColumnType("datetime2");

                    b.Property<float>("tongTien")
                        .HasColumnType("real");

                    b.Property<string>("trangThai")
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.HasKey("id");

                    b.HasIndex("CustomerID");

                    b.HasIndex("UserID");

                    b.ToTable("Order");
                });

            modelBuilder.Entity("WebsiteBanHang.Areas.Admin.Models.PermissionRoleModel", b =>
                {
                    b.Property<int>("Permission_ID")
                        .HasColumnType("int")
                        .HasColumnOrder(0);

                    b.Property<int>("Role_ID")
                        .HasColumnType("int")
                        .HasColumnOrder(1);

                    b.HasKey("Permission_ID", "Role_ID");

                    b.HasIndex("Role_ID");

                    b.ToTable("PermissionRole");
                });

            modelBuilder.Entity("WebsiteBanHang.Areas.Admin.Models.PermissionsModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<bool>("Access")
                        .HasColumnType("bit");

                    b.Property<bool>("Add")
                        .HasColumnType("bit");

                    b.Property<bool>("Edit")
                        .HasColumnType("bit");

                    b.Property<string>("FunctionName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("Remote")
                        .HasColumnType("bit");

                    b.Property<bool>("Show")
                        .HasColumnType("bit");

                    b.HasKey("Id");

                    b.ToTable("Permissions");
                });

            modelBuilder.Entity("WebsiteBanHang.Areas.Admin.Models.ProductModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<decimal>("GiaBan")
                        .HasColumnType("decimal(18,2)");

                    b.Property<decimal>("GiaGiam")
                        .HasColumnType("decimal(18,2)");

                    b.Property<decimal>("GiaNhap")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int?>("HangId")
                        .IsRequired()
                        .HasColumnType("int");

                    b.Property<string>("Image")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<int?>("LoaiId")
                        .IsRequired()
                        .HasColumnType("int");

                    b.Property<string>("MaSanPham")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar(10)");

                    b.Property<string>("TenSanPham")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<string>("ThongTinSanPham")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("HangId");

                    b.HasIndex("LoaiId");

                    b.ToTable("Product");
                });

            modelBuilder.Entity("WebsiteBanHang.Areas.Admin.Models.RoleModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("Id");

                    b.ToTable("Role");
                });

            modelBuilder.Entity("WebsiteBanHang.Areas.Admin.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("Discriminator")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("MaNguoiDung")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar(10)");

                    b.Property<string>("MatKhau")
                        .HasMaxLength(32)
                        .HasColumnType("nvarchar(32)");

                    b.Property<DateTime>("NgayTao")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.ToTable("User");

                    b.HasDiscriminator<string>("Discriminator").HasValue("User");
                });

            modelBuilder.Entity("WebsiteBanHang.Areas.Admin.Models.UserRoleModel", b =>
                {
                    b.Property<int>("User_ID")
                        .HasColumnType("int")
                        .HasColumnOrder(0);

                    b.Property<int>("Role_ID")
                        .HasColumnType("int")
                        .HasColumnOrder(1);

                    b.HasKey("User_ID", "Role_ID");

                    b.HasIndex("Role_ID");

                    b.ToTable("UserRole");
                });

            modelBuilder.Entity("WebsiteBanHang.Areas.Admin.Models.Users_Details", b =>
                {
                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<string>("DiaChi")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("nvarchar(64)");

                    b.Property<string>("HoTen")
                        .HasMaxLength(64)
                        .HasColumnType("nvarchar(64)");

                    b.Property<string>("SoDienThoai")
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar(10)");

                    b.HasKey("UserId");

                    b.ToTable("Users_Details");
                });

            modelBuilder.Entity("WebsiteBanHang.Areas.Admin.Models.CustomerModel", b =>
                {
                    b.HasBaseType("WebsiteBanHang.Areas.Admin.Models.User");

                    b.Property<string>("ChatConnectionId")
                        .HasColumnType("nvarchar(450)");

                    b.HasIndex("ChatConnectionId");

                    b.HasDiscriminator().HasValue("CustomerModel");
                });

            modelBuilder.Entity("WebsiteBanHang.Areas.Admin.Models.UserModel", b =>
                {
                    b.HasBaseType("WebsiteBanHang.Areas.Admin.Models.User");

                    b.Property<string>("ChatConnectionId")
                        .HasColumnType("nvarchar(450)")
                        .HasColumnName("UserModel_ChatConnectionId");

                    b.HasIndex("ChatConnectionId");

                    b.HasDiscriminator().HasValue("UserModel");
                });

            modelBuilder.Entity("WebsiteBanHang.Areas.Admin.Models.Cart_Item", b =>
                {
                    b.HasOne("WebsiteBanHang.Areas.Admin.Models.CartModel", "Cart")
                        .WithMany("CartItems")
                        .HasForeignKey("CartId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("WebsiteBanHang.Areas.Admin.Models.ProductModel", "Product")
                        .WithMany("CartItems")
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Cart");

                    b.Navigation("Product");
                });

            modelBuilder.Entity("WebsiteBanHang.Areas.Admin.Models.CartModel", b =>
                {
                    b.HasOne("WebsiteBanHang.Areas.Admin.Models.CustomerModel", "Customer")
                        .WithMany("Carts")
                        .HasForeignKey("CustomerId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Customer");
                });

            modelBuilder.Entity("WebsiteBanHang.Areas.Admin.Models.ChatConnection", b =>
                {
                    b.HasOne("WebsiteBanHang.Areas.Admin.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("WebsiteBanHang.Areas.Admin.Models.ChatMessage", b =>
                {
                    b.HasOne("WebsiteBanHang.Areas.Admin.Models.ChatConnection", null)
                        .WithMany("ChatMessages")
                        .HasForeignKey("ChatConnectionConnectionId");

                    b.HasOne("WebsiteBanHang.Areas.Admin.Models.ChatConnection", "FromConnection")
                        .WithMany()
                        .HasForeignKey("ConnectionIdFrom")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("WebsiteBanHang.Areas.Admin.Models.ChatConnection", "ToConnection")
                        .WithMany()
                        .HasForeignKey("ConnectionIdTo")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("FromConnection");

                    b.Navigation("ToConnection");
                });

            modelBuilder.Entity("WebsiteBanHang.Areas.Admin.Models.Customer_Details", b =>
                {
                    b.HasOne("WebsiteBanHang.Areas.Admin.Models.CustomerModel", "Customer")
                        .WithOne("CustomerDetail")
                        .HasForeignKey("WebsiteBanHang.Areas.Admin.Models.Customer_Details", "CustomerId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Customer");
                });

            modelBuilder.Entity("WebsiteBanHang.Areas.Admin.Models.CustomerRoleModel", b =>
                {
                    b.HasOne("WebsiteBanHang.Areas.Admin.Models.CustomerModel", "Customer")
                        .WithMany("CustomerRole")
                        .HasForeignKey("Customer_ID")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("WebsiteBanHang.Areas.Admin.Models.RoleModel", "Role")
                        .WithMany("CustomerRole")
                        .HasForeignKey("Role_ID")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Customer");

                    b.Navigation("Role");
                });

            modelBuilder.Entity("WebsiteBanHang.Areas.Admin.Models.InventoriesModel", b =>
                {
                    b.HasOne("WebsiteBanHang.Areas.Admin.Models.ProductModel", "product")
                        .WithMany("Inventory")
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("product");
                });

            modelBuilder.Entity("WebsiteBanHang.Areas.Admin.Models.OrderDetaiModel", b =>
                {
                    b.HasOne("WebsiteBanHang.Areas.Admin.Models.OrdersModel", "order")
                        .WithMany("ctdh")
                        .HasForeignKey("OrderId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("WebsiteBanHang.Areas.Admin.Models.ProductModel", "product")
                        .WithMany("Order_Detai")
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("order");

                    b.Navigation("product");
                });

            modelBuilder.Entity("WebsiteBanHang.Areas.Admin.Models.OrdersModel", b =>
                {
                    b.HasOne("WebsiteBanHang.Areas.Admin.Models.CustomerModel", "Customer")
                        .WithMany("Order")
                        .HasForeignKey("CustomerID")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("WebsiteBanHang.Areas.Admin.Models.UserModel", "user")
                        .WithMany("Order")
                        .HasForeignKey("UserID")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.Navigation("Customer");

                    b.Navigation("user");
                });

            modelBuilder.Entity("WebsiteBanHang.Areas.Admin.Models.PermissionRoleModel", b =>
                {
                    b.HasOne("WebsiteBanHang.Areas.Admin.Models.PermissionsModel", "Permission")
                        .WithMany("PermissionRole")
                        .HasForeignKey("Permission_ID")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("WebsiteBanHang.Areas.Admin.Models.RoleModel", "Role")
                        .WithMany("PermissionRole")
                        .HasForeignKey("Role_ID")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Permission");

                    b.Navigation("Role");
                });

            modelBuilder.Entity("WebsiteBanHang.Areas.Admin.Models.ProductModel", b =>
                {
                    b.HasOne("WebsiteBanHang.Areas.Admin.Models.BrandModel", "Brand")
                        .WithMany("Prodcut")
                        .HasForeignKey("HangId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("WebsiteBanHang.Areas.Admin.Models.CategoryModel", "Category")
                        .WithMany("Prodcut")
                        .HasForeignKey("LoaiId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Brand");

                    b.Navigation("Category");
                });

            modelBuilder.Entity("WebsiteBanHang.Areas.Admin.Models.UserRoleModel", b =>
                {
                    b.HasOne("WebsiteBanHang.Areas.Admin.Models.RoleModel", "Role")
                        .WithMany("UserRole")
                        .HasForeignKey("Role_ID")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("WebsiteBanHang.Areas.Admin.Models.UserModel", "User")
                        .WithMany("UserRole")
                        .HasForeignKey("User_ID")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Role");

                    b.Navigation("User");
                });

            modelBuilder.Entity("WebsiteBanHang.Areas.Admin.Models.Users_Details", b =>
                {
                    b.HasOne("WebsiteBanHang.Areas.Admin.Models.UserModel", "User")
                        .WithOne("userDetail")
                        .HasForeignKey("WebsiteBanHang.Areas.Admin.Models.Users_Details", "UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("WebsiteBanHang.Areas.Admin.Models.CustomerModel", b =>
                {
                    b.HasOne("WebsiteBanHang.Areas.Admin.Models.ChatConnection", "ChatConnection")
                        .WithMany()
                        .HasForeignKey("ChatConnectionId");

                    b.Navigation("ChatConnection");
                });

            modelBuilder.Entity("WebsiteBanHang.Areas.Admin.Models.UserModel", b =>
                {
                    b.HasOne("WebsiteBanHang.Areas.Admin.Models.ChatConnection", "ChatConnection")
                        .WithMany()
                        .HasForeignKey("ChatConnectionId");

                    b.Navigation("ChatConnection");
                });

            modelBuilder.Entity("WebsiteBanHang.Areas.Admin.Models.BrandModel", b =>
                {
                    b.Navigation("Prodcut");
                });

            modelBuilder.Entity("WebsiteBanHang.Areas.Admin.Models.CartModel", b =>
                {
                    b.Navigation("CartItems");
                });

            modelBuilder.Entity("WebsiteBanHang.Areas.Admin.Models.CategoryModel", b =>
                {
                    b.Navigation("Prodcut");
                });

            modelBuilder.Entity("WebsiteBanHang.Areas.Admin.Models.ChatConnection", b =>
                {
                    b.Navigation("ChatMessages");
                });

            modelBuilder.Entity("WebsiteBanHang.Areas.Admin.Models.OrdersModel", b =>
                {
                    b.Navigation("ctdh");
                });

            modelBuilder.Entity("WebsiteBanHang.Areas.Admin.Models.PermissionsModel", b =>
                {
                    b.Navigation("PermissionRole");
                });

            modelBuilder.Entity("WebsiteBanHang.Areas.Admin.Models.ProductModel", b =>
                {
                    b.Navigation("CartItems");

                    b.Navigation("Inventory");

                    b.Navigation("Order_Detai");
                });

            modelBuilder.Entity("WebsiteBanHang.Areas.Admin.Models.RoleModel", b =>
                {
                    b.Navigation("CustomerRole");

                    b.Navigation("PermissionRole");

                    b.Navigation("UserRole");
                });

            modelBuilder.Entity("WebsiteBanHang.Areas.Admin.Models.CustomerModel", b =>
                {
                    b.Navigation("Carts");

                    b.Navigation("CustomerDetail");

                    b.Navigation("CustomerRole");

                    b.Navigation("Order");
                });

            modelBuilder.Entity("WebsiteBanHang.Areas.Admin.Models.UserModel", b =>
                {
                    b.Navigation("Order");

                    b.Navigation("UserRole");

                    b.Navigation("userDetail");
                });
#pragma warning restore 612, 618
        }
    }
}
