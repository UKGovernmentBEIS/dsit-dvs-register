using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DVSRegister.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateReviewTableMappings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PublicInterestCheck_ServiceId",
                table: "PublicInterestCheck");

            migrationBuilder.DropIndex(
                name: "IX_CertificateReview_ServiceId",
                table: "CertificateReview");

            migrationBuilder.AddColumn<bool>(
                name: "IsLatestReviewVersion",
                table: "PublicInterestCheck",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "ReviewVersion",
                table: "PublicInterestCheck",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsLatestReviewVersion",
                table: "CertificateReview",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "ReviewVersion",
                table: "CertificateReview",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_PublicInterestCheck_ServiceId",
                table: "PublicInterestCheck",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_CertificateReview_ServiceId",
                table: "CertificateReview",
                column: "ServiceId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PublicInterestCheck_ServiceId",
                table: "PublicInterestCheck");

            migrationBuilder.DropIndex(
                name: "IX_CertificateReview_ServiceId",
                table: "CertificateReview");

            migrationBuilder.DropColumn(
                name: "IsLatestReviewVersion",
                table: "PublicInterestCheck");

            migrationBuilder.DropColumn(
                name: "ReviewVersion",
                table: "PublicInterestCheck");

            migrationBuilder.DropColumn(
                name: "IsLatestReviewVersion",
                table: "CertificateReview");

            migrationBuilder.DropColumn(
                name: "ReviewVersion",
                table: "CertificateReview");

            migrationBuilder.CreateIndex(
                name: "IX_PublicInterestCheck_ServiceId",
                table: "PublicInterestCheck",
                column: "ServiceId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CertificateReview_ServiceId",
                table: "CertificateReview",
                column: "ServiceId",
                unique: true);
        }
    }
}
