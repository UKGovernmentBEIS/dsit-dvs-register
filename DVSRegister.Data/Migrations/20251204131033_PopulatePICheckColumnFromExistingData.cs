using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DVSRegister.Data.Migrations
{
    /// <inheritdoc />
    public partial class PopulatePICheckColumnFromExistingData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"UPDATE ""CertificateReview"" SET ""CertificateValid"" = TRUE WHERE ""CertificateValid"" IS NULL 
            AND ""IsCabLogoCorrect"" = TRUE AND ""IsCabDetailsCorrect"" = TRUE 
            AND ""IsProviderDetailsCorrect"" = TRUE AND ""IsServiceNameCorrect"" = TRUE 
            AND ""IsRolesCertifiedCorrect"" = TRUE AND ""IsCertificationScopeCorrect"" = TRUE 
            AND ""IsServiceSummaryCorrect"" = TRUE AND ""IsURLLinkToServiceCorrect"" = TRUE 
            AND ""IsGPG44Correct"" = TRUE AND ""IsGPG45Correct"" = TRUE
            AND ""IsServiceProvisionCorrect"" = TRUE AND ""IsLocationCorrect"" = TRUE
            AND ""IsDateOfIssueCorrect"" = TRUE AND ""IsDateOfExpiryCorrect"" = TRUE 
            AND ""IsAuthenticyVerifiedCorrect"" = TRUE;");

            migrationBuilder.Sql(@"UPDATE ""CertificateReview"" SET ""CertificateValid"" = FALSE WHERE ""CertificateValid"" IS NULL
            AND (""IsCabLogoCorrect"" = FALSE OR ""IsCabDetailsCorrect"" = FALSE 
            OR ""IsProviderDetailsCorrect"" = FALSE OR ""IsServiceNameCorrect"" = FALSE 
            OR ""IsRolesCertifiedCorrect"" = FALSE OR ""IsCertificationScopeCorrect"" = FALSE 
            OR ""IsServiceSummaryCorrect"" = FALSE OR ""IsURLLinkToServiceCorrect"" = FALSE 
            OR ""IsGPG44Correct"" = FALSE OR ""IsGPG45Correct"" = FALSE 
            OR ""IsServiceProvisionCorrect"" = FALSE OR ""IsLocationCorrect"" = FALSE 
            OR ""IsDateOfIssueCorrect"" = FALSE OR ""IsDateOfExpiryCorrect"" = FALSE 
            OR ""IsAuthenticyVerifiedCorrect"" = FALSE);");



            migrationBuilder.Sql(@"UPDATE ""PublicInterestCheck"" SET ""PublicInterestChecksMet"" = TRUE WHERE ""PublicInterestChecksMet"" IS NULL
            AND ""IsCompanyHouseNumberApproved"" = TRUE AND ""IsDirectorshipsApproved"" = TRUE
            AND ""IsDirectorshipsAndRelationApproved"" = TRUE AND ""IsTradingAddressApproved"" = TRUE
            AND ""IsSanctionListApproved"" = TRUE AND ""IsUNFCApproved"" = TRUE
            AND ""IsECCheckApproved"" = TRUE AND ""IsTARICApproved"" = TRUE
            AND ""IsBannedPoliticalApproved"" = TRUE AND ""IsProvidersWebpageApproved"" = TRUE;");


            migrationBuilder.Sql(@"UPDATE ""PublicInterestCheck"" SET ""PublicInterestChecksMet"" = FALSE WHERE ""PublicInterestChecksMet"" IS NULL 
            AND (""IsCompanyHouseNumberApproved"" = FALSE OR ""IsDirectorshipsApproved"" = FALSE 
            OR ""IsDirectorshipsAndRelationApproved"" = FALSE OR ""IsTradingAddressApproved"" = FALSE 
            OR ""IsSanctionListApproved"" = FALSE OR ""IsUNFCApproved"" = FALSE 
            OR ""IsECCheckApproved"" = FALSE OR ""IsTARICApproved"" = FALSE 
            OR ""IsBannedPoliticalApproved"" = FALSE OR ""IsProvidersWebpageApproved"" = FALSE);");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {


        }
    }
}
