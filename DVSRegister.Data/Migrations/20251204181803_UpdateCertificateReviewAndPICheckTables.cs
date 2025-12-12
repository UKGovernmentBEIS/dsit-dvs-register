using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DVSRegister.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCertificateReviewAndPICheckTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsBannedPoliticalApproved",
                table: "PublicInterestCheck");

            migrationBuilder.DropColumn(
                name: "IsCompanyHouseNumberApproved",
                table: "PublicInterestCheck");

            migrationBuilder.DropColumn(
                name: "IsDirectorshipsAndRelationApproved",
                table: "PublicInterestCheck");

            migrationBuilder.DropColumn(
                name: "IsDirectorshipsApproved",
                table: "PublicInterestCheck");

            migrationBuilder.DropColumn(
                name: "IsECCheckApproved",
                table: "PublicInterestCheck");

            migrationBuilder.DropColumn(
                name: "IsProvidersWebpageApproved",
                table: "PublicInterestCheck");

            migrationBuilder.DropColumn(
                name: "IsSanctionListApproved",
                table: "PublicInterestCheck");

            migrationBuilder.DropColumn(
                name: "IsTARICApproved",
                table: "PublicInterestCheck");

            migrationBuilder.DropColumn(
                name: "IsTradingAddressApproved",
                table: "PublicInterestCheck");

            migrationBuilder.DropColumn(
                name: "IsUNFCApproved",
                table: "PublicInterestCheck");

            migrationBuilder.DropColumn(
                name: "IsAuthenticyVerifiedCorrect",
                table: "CertificateReview");

            migrationBuilder.DropColumn(
                name: "IsCabDetailsCorrect",
                table: "CertificateReview");

            migrationBuilder.DropColumn(
                name: "IsCabLogoCorrect",
                table: "CertificateReview");

            migrationBuilder.DropColumn(
                name: "IsCertificationScopeCorrect",
                table: "CertificateReview");

            migrationBuilder.DropColumn(
                name: "IsDateOfExpiryCorrect",
                table: "CertificateReview");

            migrationBuilder.DropColumn(
                name: "IsDateOfIssueCorrect",
                table: "CertificateReview");

            migrationBuilder.DropColumn(
                name: "IsGPG44Correct",
                table: "CertificateReview");

            migrationBuilder.DropColumn(
                name: "IsGPG45Correct",
                table: "CertificateReview");

            migrationBuilder.DropColumn(
                name: "IsLocationCorrect",
                table: "CertificateReview");

            migrationBuilder.DropColumn(
                name: "IsProviderDetailsCorrect",
                table: "CertificateReview");

            migrationBuilder.DropColumn(
                name: "IsRolesCertifiedCorrect",
                table: "CertificateReview");

            migrationBuilder.DropColumn(
                name: "IsServiceNameCorrect",
                table: "CertificateReview");

            migrationBuilder.DropColumn(
                name: "IsServiceProvisionCorrect",
                table: "CertificateReview");

            migrationBuilder.DropColumn(
                name: "IsServiceSummaryCorrect",
                table: "CertificateReview");

            migrationBuilder.DropColumn(
                name: "IsURLLinkToServiceCorrect",
                table: "CertificateReview");

            migrationBuilder.AddColumn<string>(
                name: "ReturningSubmissionComments",
                table: "PublicInterestCheck",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReturningSubmissionComments",
                table: "CertificateReview",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReturningSubmissionComments",
                table: "PublicInterestCheck");

            migrationBuilder.DropColumn(
                name: "ReturningSubmissionComments",
                table: "CertificateReview");

            migrationBuilder.AddColumn<bool>(
                name: "IsBannedPoliticalApproved",
                table: "PublicInterestCheck",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsCompanyHouseNumberApproved",
                table: "PublicInterestCheck",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDirectorshipsAndRelationApproved",
                table: "PublicInterestCheck",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDirectorshipsApproved",
                table: "PublicInterestCheck",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsECCheckApproved",
                table: "PublicInterestCheck",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsProvidersWebpageApproved",
                table: "PublicInterestCheck",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsSanctionListApproved",
                table: "PublicInterestCheck",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsTARICApproved",
                table: "PublicInterestCheck",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsTradingAddressApproved",
                table: "PublicInterestCheck",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsUNFCApproved",
                table: "PublicInterestCheck",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsAuthenticyVerifiedCorrect",
                table: "CertificateReview",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsCabDetailsCorrect",
                table: "CertificateReview",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsCabLogoCorrect",
                table: "CertificateReview",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsCertificationScopeCorrect",
                table: "CertificateReview",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDateOfExpiryCorrect",
                table: "CertificateReview",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDateOfIssueCorrect",
                table: "CertificateReview",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsGPG44Correct",
                table: "CertificateReview",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsGPG45Correct",
                table: "CertificateReview",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsLocationCorrect",
                table: "CertificateReview",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsProviderDetailsCorrect",
                table: "CertificateReview",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsRolesCertifiedCorrect",
                table: "CertificateReview",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsServiceNameCorrect",
                table: "CertificateReview",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsServiceProvisionCorrect",
                table: "CertificateReview",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsServiceSummaryCorrect",
                table: "CertificateReview",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsURLLinkToServiceCorrect",
                table: "CertificateReview",
                type: "boolean",
                nullable: true);
        }
    }
}
