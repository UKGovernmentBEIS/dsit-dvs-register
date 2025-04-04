using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DVSRegister.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddAmendmentsCoulmn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ServiceDraft_ServiceId",
                table: "ServiceDraft");

            migrationBuilder.DropIndex(
                name: "IX_ProviderProfileDraft_ProviderProfileId",
                table: "ProviderProfileDraft");

            migrationBuilder.AddColumn<string>(
                name: "Amendments",
                table: "CertificateReview",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ServiceDraft_ServiceId",
                table: "ServiceDraft",
                column: "ServiceId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProviderProfileDraft_ProviderProfileId",
                table: "ProviderProfileDraft",
                column: "ProviderProfileId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ServiceDraft_ServiceId",
                table: "ServiceDraft");

            migrationBuilder.DropIndex(
                name: "IX_ProviderProfileDraft_ProviderProfileId",
                table: "ProviderProfileDraft");

            migrationBuilder.DropColumn(
                name: "Amendments",
                table: "CertificateReview");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceDraft_ServiceId",
                table: "ServiceDraft",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_ProviderProfileDraft_ProviderProfileId",
                table: "ProviderProfileDraft",
                column: "ProviderProfileId");
        }
    }
}
