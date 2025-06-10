using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DVSRegister.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveCabUserCoulmn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProviderProfile_CabUser_CabUserId",
                table: "ProviderProfile");

            migrationBuilder.DropIndex(
                name: "IX_ProviderProfile_CabUserId",
                table: "ProviderProfile");

            migrationBuilder.DropColumn(
                name: "CabUserId",
                table: "ProviderProfile");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CabUserId",
                table: "ProviderProfile",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ProviderProfile_CabUserId",
                table: "ProviderProfile",
                column: "CabUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProviderProfile_CabUser_CabUserId",
                table: "ProviderProfile",
                column: "CabUserId",
                principalTable: "CabUser",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
