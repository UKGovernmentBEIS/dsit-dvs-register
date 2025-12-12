using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DVSRegister.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePiCheckTableAddSendBackUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PublicInterestCheck_User_PrimaryCheckUserId",
                table: "PublicInterestCheck");

            migrationBuilder.AlterColumn<int>(
                name: "PrimaryCheckUserId",
                table: "PublicInterestCheck",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<int>(
                name: "SentBackByUserId",
                table: "PublicInterestCheck",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PublicInterestCheck_SentBackByUserId",
                table: "PublicInterestCheck",
                column: "SentBackByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_PublicInterestCheck_User_PrimaryCheckUserId",
                table: "PublicInterestCheck",
                column: "PrimaryCheckUserId",
                principalTable: "User",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PublicInterestCheck_User_SentBackByUserId",
                table: "PublicInterestCheck",
                column: "SentBackByUserId",
                principalTable: "User",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PublicInterestCheck_User_PrimaryCheckUserId",
                table: "PublicInterestCheck");

            migrationBuilder.DropForeignKey(
                name: "FK_PublicInterestCheck_User_SentBackByUserId",
                table: "PublicInterestCheck");

            migrationBuilder.DropIndex(
                name: "IX_PublicInterestCheck_SentBackByUserId",
                table: "PublicInterestCheck");

            migrationBuilder.DropColumn(
                name: "SentBackByUserId",
                table: "PublicInterestCheck");

            migrationBuilder.AlterColumn<int>(
                name: "PrimaryCheckUserId",
                table: "PublicInterestCheck",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PublicInterestCheck_User_PrimaryCheckUserId",
                table: "PublicInterestCheck",
                column: "PrimaryCheckUserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
