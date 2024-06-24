using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebsiteBanHang.Migrations
{
    public partial class update_message : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ChatConnectionConnectionId",
                table: "ChatMessage",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ChatMessage_ChatConnectionConnectionId",
                table: "ChatMessage",
                column: "ChatConnectionConnectionId");

            migrationBuilder.AddForeignKey(
                name: "FK_ChatMessage_ChatConnection_ChatConnectionConnectionId",
                table: "ChatMessage",
                column: "ChatConnectionConnectionId",
                principalTable: "ChatConnection",
                principalColumn: "ConnectionId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChatMessage_ChatConnection_ChatConnectionConnectionId",
                table: "ChatMessage");

            migrationBuilder.DropIndex(
                name: "IX_ChatMessage_ChatConnectionConnectionId",
                table: "ChatMessage");

            migrationBuilder.DropColumn(
                name: "ChatConnectionConnectionId",
                table: "ChatMessage");
        }
    }
}
