using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuizApi.Migrations
{
    /// <inheritdoc />
    public partial class NameRefactor1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Friendships_Users_MeId",
                table: "Friendships");

            migrationBuilder.DropForeignKey(
                name: "FK_Friendships_Users_TheyId",
                table: "Friendships");

            migrationBuilder.DropForeignKey(
                name: "FK_Questions_Quizes_QuestionSetId",
                table: "Questions");

            migrationBuilder.RenameColumn(
                name: "QuestionSetId",
                table: "Questions",
                newName: "QuizId");

            migrationBuilder.RenameIndex(
                name: "IX_Questions_QuestionSetId",
                table: "Questions",
                newName: "IX_Questions_QuizId");

            migrationBuilder.RenameColumn(
                name: "TheyId",
                table: "Friendships",
                newName: "SecondUserId");

            migrationBuilder.RenameColumn(
                name: "MeId",
                table: "Friendships",
                newName: "FirstUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Friendships_TheyId",
                table: "Friendships",
                newName: "IX_Friendships_SecondUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Friendships_Users_FirstUserId",
                table: "Friendships",
                column: "FirstUserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Friendships_Users_SecondUserId",
                table: "Friendships",
                column: "SecondUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Questions_Quizes_QuizId",
                table: "Questions",
                column: "QuizId",
                principalTable: "Quizes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Friendships_Users_FirstUserId",
                table: "Friendships");

            migrationBuilder.DropForeignKey(
                name: "FK_Friendships_Users_SecondUserId",
                table: "Friendships");

            migrationBuilder.DropForeignKey(
                name: "FK_Questions_Quizes_QuizId",
                table: "Questions");

            migrationBuilder.RenameColumn(
                name: "QuizId",
                table: "Questions",
                newName: "QuestionSetId");

            migrationBuilder.RenameIndex(
                name: "IX_Questions_QuizId",
                table: "Questions",
                newName: "IX_Questions_QuestionSetId");

            migrationBuilder.RenameColumn(
                name: "SecondUserId",
                table: "Friendships",
                newName: "TheyId");

            migrationBuilder.RenameColumn(
                name: "FirstUserId",
                table: "Friendships",
                newName: "MeId");

            migrationBuilder.RenameIndex(
                name: "IX_Friendships_SecondUserId",
                table: "Friendships",
                newName: "IX_Friendships_TheyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Friendships_Users_MeId",
                table: "Friendships",
                column: "MeId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Friendships_Users_TheyId",
                table: "Friendships",
                column: "TheyId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Questions_Quizes_QuestionSetId",
                table: "Questions",
                column: "QuestionSetId",
                principalTable: "Quizes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
