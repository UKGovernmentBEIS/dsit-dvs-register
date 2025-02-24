using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DVSRegister.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddDraftTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProviderProfileDraft",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RegisteredName = table.Column<string>(type: "text", nullable: true),
                    TradingName = table.Column<string>(type: "text", nullable: true),
                    HasRegistrationNumber = table.Column<bool>(type: "boolean", nullable: true),
                    CompanyRegistrationNumber = table.Column<string>(type: "text", nullable: true),
                    DUNSNumber = table.Column<string>(type: "text", nullable: true),
                    HasParentCompany = table.Column<bool>(type: "boolean", nullable: true),
                    ParentCompanyRegisteredName = table.Column<string>(type: "text", nullable: true),
                    ParentCompanyLocation = table.Column<string>(type: "text", nullable: true),
                    PrimaryContactFullName = table.Column<string>(type: "text", nullable: true),
                    PrimaryContactJobTitle = table.Column<string>(type: "text", nullable: true),
                    PrimaryContactEmail = table.Column<string>(type: "text", nullable: true),
                    PrimaryContactTelephoneNumber = table.Column<string>(type: "text", nullable: true),
                    SecondaryContactFullName = table.Column<string>(type: "text", nullable: true),
                    SecondaryContactJobTitle = table.Column<string>(type: "text", nullable: true),
                    SecondaryContactEmail = table.Column<string>(type: "text", nullable: true),
                    SecondaryContactTelephoneNumber = table.Column<string>(type: "text", nullable: true),
                    PublicContactEmail = table.Column<string>(type: "text", nullable: true),
                    ProviderTelephoneNumber = table.Column<string>(type: "text", nullable: true),
                    ProviderWebsiteAddress = table.Column<string>(type: "text", nullable: true),
                    ProviderProfileId = table.Column<int>(type: "integer", nullable: false),
                    RequestedUserId = table.Column<int>(type: "integer", nullable: false),
                    PreviousProviderStatus = table.Column<int>(type: "integer", nullable: false),
                    ModifiedTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProviderProfileDraft", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProviderProfileDraft_ProviderProfile_ProviderProfileId",
                        column: x => x.ProviderProfileId,
                        principalTable: "ProviderProfile",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProviderProfileDraft_User_RequestedUserId",
                        column: x => x.RequestedUserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServiceDraft",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ServiceName = table.Column<string>(type: "text", nullable: true),
                    WebSiteAddress = table.Column<string>(type: "text", nullable: true),
                    CompanyAddress = table.Column<string>(type: "text", nullable: true),
                    HasGPG44 = table.Column<bool>(type: "boolean", nullable: true),
                    HasGPG45 = table.Column<bool>(type: "boolean", nullable: true),
                    HasSupplementarySchemes = table.Column<bool>(type: "boolean", nullable: true),
                    ConformityIssueDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ConformityExpiryDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ServiceId = table.Column<int>(type: "integer", nullable: false),
                    RequestedUserId = table.Column<int>(type: "integer", nullable: false),
                    PreviousServiceStatus = table.Column<int>(type: "integer", nullable: false),
                    ModifiedTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceDraft", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiceDraft_Service_ServiceId",
                        column: x => x.ServiceId,
                        principalTable: "Service",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServiceDraft_User_RequestedUserId",
                        column: x => x.RequestedUserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProviderDraftToken",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    TokenId = table.Column<string>(type: "text", nullable: false),
                    Token = table.Column<string>(type: "text", nullable: false),
                    ProviderProfileDraftId = table.Column<int>(type: "integer", nullable: false),
                    CreatedTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProviderDraftToken", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProviderDraftToken_ProviderProfileDraft_ProviderProfileDraf~",
                        column: x => x.ProviderProfileDraftId,
                        principalTable: "ProviderProfileDraft",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServiceDraftToken",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    TokenId = table.Column<string>(type: "text", nullable: false),
                    Token = table.Column<string>(type: "text", nullable: false),
                    ServiceDraftId = table.Column<int>(type: "integer", nullable: false),
                    CreatedTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceDraftToken", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiceDraftToken_ServiceDraft_ServiceDraftId",
                        column: x => x.ServiceDraftId,
                        principalTable: "ServiceDraft",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServiceIdentityProfileMappingDraft",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ServiceDraftId = table.Column<int>(type: "integer", nullable: false),
                    IdentityProfileId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceIdentityProfileMappingDraft", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiceIdentityProfileMappingDraft_IdentityProfile_Identity~",
                        column: x => x.IdentityProfileId,
                        principalTable: "IdentityProfile",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServiceIdentityProfileMappingDraft_ServiceDraft_ServiceDraf~",
                        column: x => x.ServiceDraftId,
                        principalTable: "ServiceDraft",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServiceQualityLevelMappingDraft",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ServiceDraftId = table.Column<int>(type: "integer", nullable: false),
                    QualityLevelId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceQualityLevelMappingDraft", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiceQualityLevelMappingDraft_QualityLevel_QualityLevelId",
                        column: x => x.QualityLevelId,
                        principalTable: "QualityLevel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServiceQualityLevelMappingDraft_ServiceDraft_ServiceDraftId",
                        column: x => x.ServiceDraftId,
                        principalTable: "ServiceDraft",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServiceRoleMappingDraft",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ServiceDraftId = table.Column<int>(type: "integer", nullable: false),
                    RoleId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceRoleMappingDraft", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiceRoleMappingDraft_Role_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Role",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServiceRoleMappingDraft_ServiceDraft_ServiceDraftId",
                        column: x => x.ServiceDraftId,
                        principalTable: "ServiceDraft",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServiceSupSchemeMappingDraft",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ServiceDraftId = table.Column<int>(type: "integer", nullable: false),
                    SupplementarySchemeId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceSupSchemeMappingDraft", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiceSupSchemeMappingDraft_ServiceDraft_ServiceDraftId",
                        column: x => x.ServiceDraftId,
                        principalTable: "ServiceDraft",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServiceSupSchemeMappingDraft_SupplementaryScheme_Supplement~",
                        column: x => x.SupplementarySchemeId,
                        principalTable: "SupplementaryScheme",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProviderDraftToken_ProviderProfileDraftId",
                table: "ProviderDraftToken",
                column: "ProviderProfileDraftId");

            migrationBuilder.CreateIndex(
                name: "IX_ProviderDraftToken_Token",
                table: "ProviderDraftToken",
                column: "Token");

            migrationBuilder.CreateIndex(
                name: "IX_ProviderDraftToken_TokenId",
                table: "ProviderDraftToken",
                column: "TokenId");

            migrationBuilder.CreateIndex(
                name: "IX_ProviderProfileDraft_ProviderProfileId",
                table: "ProviderProfileDraft",
                column: "ProviderProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_ProviderProfileDraft_RequestedUserId",
                table: "ProviderProfileDraft",
                column: "RequestedUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceDraft_RequestedUserId",
                table: "ServiceDraft",
                column: "RequestedUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceDraft_ServiceId",
                table: "ServiceDraft",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceDraftToken_ServiceDraftId",
                table: "ServiceDraftToken",
                column: "ServiceDraftId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceDraftToken_Token",
                table: "ServiceDraftToken",
                column: "Token");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceDraftToken_TokenId",
                table: "ServiceDraftToken",
                column: "TokenId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceIdentityProfileMappingDraft_IdentityProfileId",
                table: "ServiceIdentityProfileMappingDraft",
                column: "IdentityProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceIdentityProfileMappingDraft_ServiceDraftId",
                table: "ServiceIdentityProfileMappingDraft",
                column: "ServiceDraftId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceQualityLevelMappingDraft_QualityLevelId",
                table: "ServiceQualityLevelMappingDraft",
                column: "QualityLevelId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceQualityLevelMappingDraft_ServiceDraftId",
                table: "ServiceQualityLevelMappingDraft",
                column: "ServiceDraftId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceRoleMappingDraft_RoleId",
                table: "ServiceRoleMappingDraft",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceRoleMappingDraft_ServiceDraftId",
                table: "ServiceRoleMappingDraft",
                column: "ServiceDraftId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceSupSchemeMappingDraft_ServiceDraftId",
                table: "ServiceSupSchemeMappingDraft",
                column: "ServiceDraftId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceSupSchemeMappingDraft_SupplementarySchemeId",
                table: "ServiceSupSchemeMappingDraft",
                column: "SupplementarySchemeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProviderDraftToken");

            migrationBuilder.DropTable(
                name: "ServiceDraftToken");

            migrationBuilder.DropTable(
                name: "ServiceIdentityProfileMappingDraft");

            migrationBuilder.DropTable(
                name: "ServiceQualityLevelMappingDraft");

            migrationBuilder.DropTable(
                name: "ServiceRoleMappingDraft");

            migrationBuilder.DropTable(
                name: "ServiceSupSchemeMappingDraft");

            migrationBuilder.DropTable(
                name: "ProviderProfileDraft");

            migrationBuilder.DropTable(
                name: "ServiceDraft");
        }
    }
}
