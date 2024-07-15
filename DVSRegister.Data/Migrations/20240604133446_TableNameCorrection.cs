using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DVSRegister.Data.Migrations
{
    /// <inheritdoc />
    public partial class TableNameCorrection : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CertificateReviewRejectionReasonMappings_CetificateReview_C~",
                table: "CertificateReviewRejectionReasonMappings");

            migrationBuilder.DropTable(
                name: "CetificateReview");

            migrationBuilder.CreateTable(
                name: "CertificateReview",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PreRegistrationId = table.Column<int>(type: "integer", nullable: false),
                    CertificateInformationId = table.Column<int>(type: "integer", nullable: false),
                    IsCabLogoCorrect = table.Column<bool>(type: "boolean", nullable: false),
                    IsCabDetailsCorrect = table.Column<bool>(type: "boolean", nullable: false),
                    IsProviderDetailsCorrect = table.Column<bool>(type: "boolean", nullable: false),
                    IsServiceNameCorrect = table.Column<bool>(type: "boolean", nullable: false),
                    IsRolesCertifiedCorrect = table.Column<bool>(type: "boolean", nullable: false),
                    IsCertificationScopeCorrect = table.Column<bool>(type: "boolean", nullable: false),
                    IsServiceSummaryCorrect = table.Column<bool>(type: "boolean", nullable: false),
                    IsURLLinkToServiceCorrect = table.Column<bool>(type: "boolean", nullable: false),
                    IsIdentityProfilesCorrect = table.Column<bool>(type: "boolean", nullable: false),
                    IsQualityAssessmentCorrect = table.Column<bool>(type: "boolean", nullable: false),
                    IsServiceProvisionCorrect = table.Column<bool>(type: "boolean", nullable: false),
                    IsLocationCorrect = table.Column<bool>(type: "boolean", nullable: false),
                    IsDateOfIssueCorrect = table.Column<bool>(type: "boolean", nullable: false),
                    IsDateOfExpiryCorrect = table.Column<bool>(type: "boolean", nullable: false),
                    IsAuthenticyVerifiedCorrect = table.Column<bool>(type: "boolean", nullable: false),
                    CommentsForIncorrect = table.Column<string>(type: "text", nullable: false),
                    InformationMatched = table.Column<bool>(type: "boolean", nullable: true),
                    Comments = table.Column<string>(type: "text", nullable: true),
                    RejectionComments = table.Column<string>(type: "text", nullable: true),
                    VerifiedUser = table.Column<int>(type: "integer", nullable: false),
                    CertificateInfoStatus = table.Column<int>(type: "integer", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ModifiedBy = table.Column<string>(type: "text", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CertificateReview", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CertificateReview_CertificateInformation_CertificateInforma~",
                        column: x => x.CertificateInformationId,
                        principalTable: "CertificateInformation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CertificateReview_PreRegistration_PreRegistrationId",
                        column: x => x.PreRegistrationId,
                        principalTable: "PreRegistration",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CertificateReview_CertificateInformationId",
                table: "CertificateReview",
                column: "CertificateInformationId");

            migrationBuilder.CreateIndex(
                name: "IX_CertificateReview_PreRegistrationId",
                table: "CertificateReview",
                column: "PreRegistrationId");

            migrationBuilder.AddForeignKey(
                name: "FK_CertificateReviewRejectionReasonMappings_CertificateReview_~",
                table: "CertificateReviewRejectionReasonMappings",
                column: "CetificateReviewId",
                principalTable: "CertificateReview",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CertificateReviewRejectionReasonMappings_CertificateReview_~",
                table: "CertificateReviewRejectionReasonMappings");

            migrationBuilder.DropTable(
                name: "CertificateReview");

            migrationBuilder.CreateTable(
                name: "CetificateReview",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CertificateInformationId = table.Column<int>(type: "integer", nullable: false),
                    PreRegistrationId = table.Column<int>(type: "integer", nullable: false),
                    CertificateInfoStatus = table.Column<int>(type: "integer", nullable: false),
                    Comments = table.Column<string>(type: "text", nullable: true),
                    CommentsForIncorrect = table.Column<string>(type: "text", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    InformationMatched = table.Column<bool>(type: "boolean", nullable: true),
                    IsAuthenticyVerifiedCorrect = table.Column<bool>(type: "boolean", nullable: false),
                    IsCabDetailsCorrect = table.Column<bool>(type: "boolean", nullable: false),
                    IsCabLogoCorrect = table.Column<bool>(type: "boolean", nullable: false),
                    IsCertificationScopeCorrect = table.Column<bool>(type: "boolean", nullable: false),
                    IsDateOfExpiryCorrect = table.Column<bool>(type: "boolean", nullable: false),
                    IsDateOfIssueCorrect = table.Column<bool>(type: "boolean", nullable: false),
                    IsIdentityProfilesCorrect = table.Column<bool>(type: "boolean", nullable: false),
                    IsLocationCorrect = table.Column<bool>(type: "boolean", nullable: false),
                    IsProviderDetailsCorrect = table.Column<bool>(type: "boolean", nullable: false),
                    IsQualityAssessmentCorrect = table.Column<bool>(type: "boolean", nullable: false),
                    IsRolesCertifiedCorrect = table.Column<bool>(type: "boolean", nullable: false),
                    IsServiceNameCorrect = table.Column<bool>(type: "boolean", nullable: false),
                    IsServiceProvisionCorrect = table.Column<bool>(type: "boolean", nullable: false),
                    IsServiceSummaryCorrect = table.Column<bool>(type: "boolean", nullable: false),
                    IsURLLinkToServiceCorrect = table.Column<bool>(type: "boolean", nullable: false),
                    ModifiedBy = table.Column<string>(type: "text", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    RejectionComments = table.Column<string>(type: "text", nullable: true),
                    VerifiedUser = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CetificateReview", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CetificateReview_CertificateInformation_CertificateInformat~",
                        column: x => x.CertificateInformationId,
                        principalTable: "CertificateInformation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CetificateReview_PreRegistration_PreRegistrationId",
                        column: x => x.PreRegistrationId,
                        principalTable: "PreRegistration",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CetificateReview_CertificateInformationId",
                table: "CetificateReview",
                column: "CertificateInformationId");

            migrationBuilder.CreateIndex(
                name: "IX_CetificateReview_PreRegistrationId",
                table: "CetificateReview",
                column: "PreRegistrationId");

            migrationBuilder.AddForeignKey(
                name: "FK_CertificateReviewRejectionReasonMappings_CetificateReview_C~",
                table: "CertificateReviewRejectionReasonMappings",
                column: "CetificateReviewId",
                principalTable: "CetificateReview",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
