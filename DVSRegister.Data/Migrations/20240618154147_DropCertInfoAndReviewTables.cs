using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DVSRegister.Data.Migrations
{
    /// <inheritdoc />
    public partial class DropCertInfoAndReviewTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CertificateInfoIdentityProfileMapping");

            migrationBuilder.DropTable(
                name: "CertificateInfoRoleMapping");

            migrationBuilder.DropTable(
                name: "CertificateInfoSupSchemeMappings");

            migrationBuilder.DropTable(
                name: "CertificateReviewRejectionReasonMappings");

            migrationBuilder.DropTable(
                name: "CertificateReview");

            migrationBuilder.DropTable(
                name: "CertificateInformation");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CertificateInformation",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PreRegistrationId = table.Column<int>(type: "integer", nullable: false),
                    Address = table.Column<string>(type: "text", nullable: false),
                    CertificateInfoStatus = table.Column<int>(type: "integer", nullable: false),
                    ConformityExpiryDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ConformityIssueDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    FileLink = table.Column<string>(type: "text", nullable: false),
                    FileName = table.Column<string>(type: "text", nullable: false),
                    HasSupplementarySchemes = table.Column<bool>(type: "boolean", nullable: false),
                    ModifiedBy = table.Column<string>(type: "text", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    PublicContactEmail = table.Column<string>(type: "text", nullable: false),
                    RegisteredName = table.Column<string>(type: "text", nullable: false),
                    ServiceName = table.Column<string>(type: "text", nullable: false),
                    SubmittedCAB = table.Column<string>(type: "text", nullable: true),
                    TelephoneNumber = table.Column<string>(type: "text", nullable: false),
                    TradingName = table.Column<string>(type: "text", nullable: false),
                    WebsiteAddress = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CertificateInformation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CertificateInformation_PreRegistration_PreRegistrationId",
                        column: x => x.PreRegistrationId,
                        principalTable: "PreRegistration",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CertificateInfoIdentityProfileMapping",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CertificateInformationId = table.Column<int>(type: "integer", nullable: false),
                    IdentityProfileId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CertificateInfoIdentityProfileMapping", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CertificateInfoIdentityProfileMapping_CertificateInformatio~",
                        column: x => x.CertificateInformationId,
                        principalTable: "CertificateInformation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CertificateInfoIdentityProfileMapping_IdentityProfile_Ident~",
                        column: x => x.IdentityProfileId,
                        principalTable: "IdentityProfile",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CertificateInfoRoleMapping",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CertificateInformationId = table.Column<int>(type: "integer", nullable: false),
                    RoleId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CertificateInfoRoleMapping", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CertificateInfoRoleMapping_CertificateInformation_Certifica~",
                        column: x => x.CertificateInformationId,
                        principalTable: "CertificateInformation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CertificateInfoRoleMapping_Role_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Role",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CertificateInfoSupSchemeMappings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CertificateInformationId = table.Column<int>(type: "integer", nullable: false),
                    SupplementarySchemeId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CertificateInfoSupSchemeMappings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CertificateInfoSupSchemeMappings_CertificateInformation_Cer~",
                        column: x => x.CertificateInformationId,
                        principalTable: "CertificateInformation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CertificateInfoSupSchemeMappings_SupplementaryScheme_Supple~",
                        column: x => x.SupplementarySchemeId,
                        principalTable: "SupplementaryScheme",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CertificateReview",
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

            migrationBuilder.CreateTable(
                name: "CertificateReviewRejectionReasonMappings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CertificateReviewRejectionReasonId = table.Column<int>(type: "integer", nullable: false),
                    CetificateReviewId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CertificateReviewRejectionReasonMappings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CertificateReviewRejectionReasonMappings_CertificateReviewR~",
                        column: x => x.CertificateReviewRejectionReasonId,
                        principalTable: "CertificateReviewRejectionReason",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CertificateReviewRejectionReasonMappings_CertificateReview_~",
                        column: x => x.CetificateReviewId,
                        principalTable: "CertificateReview",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CertificateInfoIdentityProfileMapping_CertificateInformatio~",
                table: "CertificateInfoIdentityProfileMapping",
                column: "CertificateInformationId");

            migrationBuilder.CreateIndex(
                name: "IX_CertificateInfoIdentityProfileMapping_IdentityProfileId",
                table: "CertificateInfoIdentityProfileMapping",
                column: "IdentityProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_CertificateInformation_PreRegistrationId",
                table: "CertificateInformation",
                column: "PreRegistrationId");

            migrationBuilder.CreateIndex(
                name: "IX_CertificateInfoRoleMapping_CertificateInformationId",
                table: "CertificateInfoRoleMapping",
                column: "CertificateInformationId");

            migrationBuilder.CreateIndex(
                name: "IX_CertificateInfoRoleMapping_RoleId",
                table: "CertificateInfoRoleMapping",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_CertificateInfoSupSchemeMappings_CertificateInformationId",
                table: "CertificateInfoSupSchemeMappings",
                column: "CertificateInformationId");

            migrationBuilder.CreateIndex(
                name: "IX_CertificateInfoSupSchemeMappings_SupplementarySchemeId",
                table: "CertificateInfoSupSchemeMappings",
                column: "SupplementarySchemeId");

            migrationBuilder.CreateIndex(
                name: "IX_CertificateReview_CertificateInformationId",
                table: "CertificateReview",
                column: "CertificateInformationId");

            migrationBuilder.CreateIndex(
                name: "IX_CertificateReview_PreRegistrationId",
                table: "CertificateReview",
                column: "PreRegistrationId");

            migrationBuilder.CreateIndex(
                name: "IX_CertificateReviewRejectionReasonMappings_CertificateReviewR~",
                table: "CertificateReviewRejectionReasonMappings",
                column: "CertificateReviewRejectionReasonId");

            migrationBuilder.CreateIndex(
                name: "IX_CertificateReviewRejectionReasonMappings_CetificateReviewId",
                table: "CertificateReviewRejectionReasonMappings",
                column: "CetificateReviewId");
        }
    }
}
