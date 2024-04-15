using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebsiteBanHang.Migrations
{
    public partial class InitialSchoolDB : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CartModel_Customer_CustomerId",
                table: "CartModel");

            migrationBuilder.DropForeignKey(
                name: "FK_Customer_Details_Customer_CustomerId",
                table: "Customer_Details");

            migrationBuilder.DropForeignKey(
                name: "FK_CustomerRole_Customer_Customer_ID",
                table: "CustomerRole");

            migrationBuilder.DropForeignKey(
                name: "FK_Order_Customer_CustomerID",
                table: "Order");

            migrationBuilder.DropTable(
                name: "Customer");

            migrationBuilder.AddColumn<string>(
                name: "ChatConnectionConnectionId",
                table: "User",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ChatConnectionId",
                table: "User",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "User",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UserModel_ChatConnectionConnectionId",
                table: "User",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UserModel_ChatConnectionId",
                table: "User",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ChatConnection",
                columns: table => new
                {
                    ConnectionId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Connected = table.Column<bool>(type: "bit", nullable: false),
                    LastActive = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatConnection", x => x.ConnectionId);
                    table.ForeignKey(
                        name: "FK_ChatConnection_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ChatMessage",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SentAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ConnectionIdFrom = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ConnectionIdTo = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatMessage", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChatMessage_ChatConnection_ConnectionIdFrom",
                        column: x => x.ConnectionIdFrom,
                        principalTable: "ChatConnection",
                        principalColumn: "ConnectionId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ChatMessage_ChatConnection_ConnectionIdTo",
                        column: x => x.ConnectionIdTo,
                        principalTable: "ChatConnection",
                        principalColumn: "ConnectionId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_User_ChatConnectionConnectionId",
                table: "User",
                column: "ChatConnectionConnectionId");

            migrationBuilder.CreateIndex(
                name: "IX_User_UserModel_ChatConnectionConnectionId",
                table: "User",
                column: "UserModel_ChatConnectionConnectionId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatConnection_UserId",
                table: "ChatConnection",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatMessage_ConnectionIdFrom",
                table: "ChatMessage",
                column: "ConnectionIdFrom");

            migrationBuilder.CreateIndex(
                name: "IX_ChatMessage_ConnectionIdTo",
                table: "ChatMessage",
                column: "ConnectionIdTo");

            migrationBuilder.AddForeignKey(
                name: "FK_CartModel_User_CustomerId",
                table: "CartModel",
                column: "CustomerId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Customer_Details_User_CustomerId",
                table: "Customer_Details",
                column: "CustomerId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerRole_User_Customer_ID",
                table: "CustomerRole",
                column: "Customer_ID",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Order_User_CustomerID",
                table: "Order",
                column: "CustomerID",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_User_ChatConnection_ChatConnectionConnectionId",
                table: "User",
                column: "ChatConnectionConnectionId",
                principalTable: "ChatConnection",
                principalColumn: "ConnectionId");

            migrationBuilder.AddForeignKey(
                name: "FK_User_ChatConnection_UserModel_ChatConnectionConnectionId",
                table: "User",
                column: "UserModel_ChatConnectionConnectionId",
                principalTable: "ChatConnection",
                principalColumn: "ConnectionId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CartModel_User_CustomerId",
                table: "CartModel");

            migrationBuilder.DropForeignKey(
                name: "FK_Customer_Details_User_CustomerId",
                table: "Customer_Details");

            migrationBuilder.DropForeignKey(
                name: "FK_CustomerRole_User_Customer_ID",
                table: "CustomerRole");

            migrationBuilder.DropForeignKey(
                name: "FK_Order_User_CustomerID",
                table: "Order");

            migrationBuilder.DropForeignKey(
                name: "FK_User_ChatConnection_ChatConnectionConnectionId",
                table: "User");

            migrationBuilder.DropForeignKey(
                name: "FK_User_ChatConnection_UserModel_ChatConnectionConnectionId",
                table: "User");

            migrationBuilder.DropTable(
                name: "ChatMessage");

            migrationBuilder.DropTable(
                name: "ChatConnection");

            migrationBuilder.DropIndex(
                name: "IX_User_ChatConnectionConnectionId",
                table: "User");

            migrationBuilder.DropIndex(
                name: "IX_User_UserModel_ChatConnectionConnectionId",
                table: "User");

            migrationBuilder.DropColumn(
                name: "ChatConnectionConnectionId",
                table: "User");

            migrationBuilder.DropColumn(
                name: "ChatConnectionId",
                table: "User");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "User");

            migrationBuilder.DropColumn(
                name: "UserModel_ChatConnectionConnectionId",
                table: "User");

            migrationBuilder.DropColumn(
                name: "UserModel_ChatConnectionId",
                table: "User");

            migrationBuilder.CreateTable(
                name: "Customer",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    MaNguoiDung = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    MatKhau = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customer", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_CartModel_Customer_CustomerId",
                table: "CartModel",
                column: "CustomerId",
                principalTable: "Customer",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Customer_Details_Customer_CustomerId",
                table: "Customer_Details",
                column: "CustomerId",
                principalTable: "Customer",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerRole_Customer_Customer_ID",
                table: "CustomerRole",
                column: "Customer_ID",
                principalTable: "Customer",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Order_Customer_CustomerID",
                table: "Order",
                column: "CustomerID",
                principalTable: "Customer",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
