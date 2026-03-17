using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DVSRegister.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDownloadLogoTokenMapping : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_DownloadLogoToken_ServiceId",
                table: "DownloadLogoToken");

            migrationBuilder.CreateIndex(
                name: "IX_DownloadLogoToken_ServiceId",
                table: "DownloadLogoToken",
                column: "ServiceId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_DownloadLogoToken_ServiceId",
                table: "DownloadLogoToken");

            migrationBuilder.CreateIndex(
                name: "IX_DownloadLogoToken_ServiceId",
                table: "DownloadLogoToken",
                column: "ServiceId");
        }
    }
}
