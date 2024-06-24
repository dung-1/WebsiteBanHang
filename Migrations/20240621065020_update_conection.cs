using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebsiteBanHang.Migrations
{
    public partial class update_conection : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_User_ChatConnection_ChatConnectionConnectionId",
                table: "User");

            migrationBuilder.DropForeignKey(
                name: "FK_User_ChatConnection_UserModel_ChatConnectionConnectionId",
                table: "User");

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
                name: "UserModel_ChatConnectionConnectionId",
                table: "User");

            migrationBuilder.AlterColumn<string>(
                name: "UserModel_ChatConnectionId",
                table: "User",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ChatConnectionId",
                table: "User",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_User_ChatConnectionId",
                table: "User",
                column: "ChatConnectionId");

            migrationBuilder.CreateIndex(
                name: "IX_User_UserModel_ChatConnectionId",
                table: "User",
                column: "UserModel_ChatConnectionId");

            migrationBuilder.AddForeignKey(
                name: "FK_User_ChatConnection_ChatConnectionId",
                table: "User",
                column: "ChatConnectionId",
                principalTable: "ChatConnection",
                principalColumn: "ConnectionId");

            migrationBuilder.AddForeignKey(
                name: "FK_User_ChatConnection_UserModel_ChatConnectionId",
                table: "User",
                column: "UserModel_ChatConnectionId",
                principalTable: "ChatConnection",
                principalColumn: "ConnectionId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_User_ChatConnection_ChatConnectionId",
                table: "User");

            migrationBuilder.DropForeignKey(
                name: "FK_User_ChatConnection_UserModel_ChatConnectionId",
                table: "User");

            migrationBuilder.DropIndex(
                name: "IX_User_ChatConnectionId",
                table: "User");

            migrationBuilder.DropIndex(
                name: "IX_User_UserModel_ChatConnectionId",
                table: "User");

            migrationBuilder.AlterColumn<int>(
                name: "UserModel_ChatConnectionId",
                table: "User",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ChatConnectionId",
                table: "User",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ChatConnectionConnectionId",
                table: "User",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserModel_ChatConnectionConnectionId",
                table: "User",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_User_ChatConnectionConnectionId",
                table: "User",
                column: "ChatConnectionConnectionId");

            migrationBuilder.CreateIndex(
                name: "IX_User_UserModel_ChatConnectionConnectionId",
                table: "User",
                column: "UserModel_ChatConnectionConnectionId");

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
    }
}
