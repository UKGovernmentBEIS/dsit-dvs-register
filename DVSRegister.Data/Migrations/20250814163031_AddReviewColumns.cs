using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DVSRegister.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddReviewColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "PublicInterestChecksMet",
                table: "PublicInterestCheck",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "CertificateValid",
                table: "CertificateReview",
                type: "boolean",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PublicInterestChecksMet",
                table: "PublicInterestCheck");

            migrationBuilder.DropColumn(
                name: "CertificateValid",
                table: "CertificateReview");
        }
    }
}
