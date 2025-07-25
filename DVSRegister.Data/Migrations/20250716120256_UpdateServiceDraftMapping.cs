using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DVSRegister.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateServiceDraftMapping : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ServiceDraft_ServiceId",
                table: "ServiceDraft");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceDraft_ServiceId",
                table: "ServiceDraft",
                column: "ServiceId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ServiceDraft_ServiceId",
                table: "ServiceDraft");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceDraft_ServiceId",
                table: "ServiceDraft",
                column: "ServiceId");
        }
    }
}
