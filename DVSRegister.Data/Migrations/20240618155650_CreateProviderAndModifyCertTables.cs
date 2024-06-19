using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DVSRegister.Data.Migrations
{
    /// <inheritdoc />
    public partial class CreateProviderAndModifyCertTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Provider",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PreRegistrationId = table.Column<int>(type: "integer", nullable: false),
                    RegisteredName = table.Column<string>(type: "text", nullable: false),
                    TradingName = table.Column<string>(type: "text", nullable: false),
                    PublicContactEmail = table.Column<string>(type: "text", nullable: false),
                    TelephoneNumber = table.Column<string>(type: "text", nullable: false),
                    WebsiteAddress = table.Column<string>(type: "text", nullable: false),
                    Address = table.Column<string>(type: "text", nullable: false),
                    ProviderStatus = table.Column<int>(type: "integer", nullable: false),
                    CreatedTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ModifiedTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    PublishedTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Provider", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Provider_PreRegistration_PreRegistrationId",
                        column: x => x.PreRegistrationId,
                        principalTable: "PreRegistration",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CertificateInformation",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProviderId = table.Column<int>(type: "integer", nullable: false),
                    ServiceName = table.Column<string>(type: "text", nullable: false),
                    HasSupplementarySchemes = table.Column<bool>(type: "boolean", nullable: false),
                    FileName = table.Column<string>(type: "text", nullable: false),
                    FileLink = table.Column<string>(type: "text", nullable: false),
                    ConformityIssueDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ConformityExpiryDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CertificateInfoStatus = table.Column<int>(type: "integer", nullable: false),
                    SubmittedCAB = table.Column<string>(type: "text", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ModifiedBy = table.Column<string>(type: "text", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CertificateInformation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CertificateInformation_Provider_ProviderId",
                        column: x => x.ProviderId,
                        principalTable: "Provider",
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
                    PreRegistrationId = table.Column<int>(type: "integer", nullable: false),
                    CertificateInformationId = table.Column<int>(type: "integer", nullable: false),
                    ProviderId = table.Column<int>(type: "integer", nullable: false),
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
                    table.ForeignKey(
                        name: "FK_CertificateReview_Provider_ProviderId",
                        column: x => x.ProviderId,
                        principalTable: "Provider",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CertificateReviewRejectionReasonMappings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CetificateReviewId = table.Column<int>(type: "integer", nullable: false),
                    CertificateReviewRejectionReasonId = table.Column<int>(type: "integer", nullable: false)
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
                name: "IX_CertificateInformation_ProviderId",
                table: "CertificateInformation",
                column: "ProviderId");

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
                name: "IX_CertificateReview_ProviderId",
                table: "CertificateReview",
                column: "ProviderId");

            migrationBuilder.CreateIndex(
                name: "IX_CertificateReviewRejectionReasonMappings_CertificateReviewR~",
                table: "CertificateReviewRejectionReasonMappings",
                column: "CertificateReviewRejectionReasonId");

            migrationBuilder.CreateIndex(
                name: "IX_CertificateReviewRejectionReasonMappings_CetificateReviewId",
                table: "CertificateReviewRejectionReasonMappings",
                column: "CetificateReviewId");

            migrationBuilder.CreateIndex(
                name: "IX_Provider_PreRegistrationId",
                table: "Provider",
                column: "PreRegistrationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.DropTable(
                name: "Provider");
        }
    }
}
