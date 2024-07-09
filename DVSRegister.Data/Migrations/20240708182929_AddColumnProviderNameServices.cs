using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DVSRegister.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddColumnProviderNameServices : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CertificateReview_CertificateInformationId",
                table: "CertificateReview");

            migrationBuilder.AddColumn<string>(
                name: "ProviderName",
                table: "RegisterPublishLog",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Services",
                table: "RegisterPublishLog",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CertificateReview_CertificateInformationId",
                table: "CertificateReview",
                column: "CertificateInformationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CertificateReview_CertificateInformationId",
                table: "CertificateReview");

            migrationBuilder.DropColumn(
                name: "ProviderName",
                table: "RegisterPublishLog");

            migrationBuilder.DropColumn(
                name: "Services",
                table: "RegisterPublishLog");

            migrationBuilder.CreateIndex(
                name: "IX_CertificateReview_CertificateInformationId",
                table: "CertificateReview",
                column: "CertificateInformationId",
                unique: true);
        }
    }
}
