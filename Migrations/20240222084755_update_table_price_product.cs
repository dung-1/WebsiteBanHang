using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebsiteBanHang.Migrations
{
    public partial class update_table_price_product : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Gia",
                table: "Product",
                newName: "GiaNhap");

            migrationBuilder.AddColumn<decimal>(
                name: "GiaBan",
                table: "Product",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GiaBan",
                table: "Product");

            migrationBuilder.RenameColumn(
                name: "GiaNhap",
                table: "Product",
                newName: "Gia");
        }
    }
}
