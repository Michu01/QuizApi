using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuizApi.Migrations
{
    /// <inheritdoc />
    public partial class PasswordLengthUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Users_UserDTOId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_UserDTOId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "UserDTOId",
                table: "Users");

            migrationBuilder.AlterColumn<string>(
                name: "Password",
                table: "Users",
                type: "nvarchar(84)",
                maxLength: 84,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(32)",
                oldMaxLength: 32);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Password",
                table: "Users",
                type: "nvarchar(32)",
                maxLength: 32,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(84)",
                oldMaxLength: 84);

            migrationBuilder.AddColumn<int>(
                name: "UserDTOId",
                table: "Users",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_UserDTOId",
                table: "Users",
                column: "UserDTOId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Users_UserDTOId",
                table: "Users",
                column: "UserDTOId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
