using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebsiteBanHang.Migrations
{
    public partial class update_cart_item : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cart_Item_CartModel_CartId",
                table: "Cart_Item");

            migrationBuilder.AddForeignKey(
                name: "FK_Cart_Item_CartModel_CartId",
                table: "Cart_Item",
                column: "CartId",
                principalTable: "CartModel",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cart_Item_CartModel_CartId",
                table: "Cart_Item");

            migrationBuilder.AddForeignKey(
                name: "FK_Cart_Item_CartModel_CartId",
                table: "Cart_Item",
                column: "CartId",
                principalTable: "CartModel",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
