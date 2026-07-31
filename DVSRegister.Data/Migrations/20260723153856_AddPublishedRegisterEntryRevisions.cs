using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DVSRegister.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPublishedRegisterEntryRevisions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PublishedRegisterEntryRevisions",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ServiceKey = table.Column<int>(type: "integer", nullable: false),
                    ProviderProfileId = table.Column<int>(type: "integer", nullable: false),
                    ServiceId = table.Column<int>(type: "integer", nullable: false),
                    ServiceVersion = table.Column<int>(type: "integer", nullable: false),
                    EffectiveAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    IsInRegister = table.Column<bool>(type: "boolean", nullable: false),
                    ActivityKind = table.Column<int>(type: "integer", nullable: false),
                    SourceType = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    SourceId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    AdditionalInformation = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: false),
                    FormatVersion = table.Column<int>(type: "integer", nullable: false),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Provider = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    Cab = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    ServiceName = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    PublicationType = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    CompanyAddress = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    PublicEmailAddress = table.Column<string>(type: "character varying(320)", maxLength: 320, nullable: true),
                    PublicTelephoneNumber = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    WebsiteAddress = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true),
                    SubmittedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    PublishedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    RemovedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    TrustFrameworkVersion = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    RolesCertifiedAgainst = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: false),
                    IsUnderpinningOrWhiteLabelledService = table.Column<bool>(type: "boolean", nullable: true),
                    IsCertifiedAgainstGpg45Profiles = table.Column<bool>(type: "boolean", nullable: true),
                    IdentityProfiles = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: false),
                    IsCertifiedAgainstGpg44 = table.Column<bool>(type: "boolean", nullable: true),
                    Gpg44AuthenticationQualities = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: false),
                    Gpg44ProtectionQualities = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: false),
                    IsCertifiedAgainstSupplementaryCodes = table.Column<bool>(type: "boolean", nullable: true),
                    SupplementaryCodes = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: false),
                    CertificateIssueDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CertificateExpiryDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    RightToWorkIdentityProfiles = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: false),
                    IsRightToWorkCertifiedAgainstGpg44 = table.Column<bool>(type: "boolean", nullable: true),
                    RightToWorkAuthenticationQualities = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: false),
                    RightToWorkProtectionQualities = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: false),
                    RightToRentIdentityProfiles = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: false),
                    IsRightToRentCertifiedAgainstGpg44 = table.Column<bool>(type: "boolean", nullable: true),
                    RightToRentAuthenticationQualities = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: false),
                    RightToRentProtectionQualities = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: false),
                    DbsIdentityProfiles = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: false),
                    IsDbsCertifiedAgainstGpg44 = table.Column<bool>(type: "boolean", nullable: true),
                    DbsAuthenticationQualities = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: false),
                    DbsProtectionQualities = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: false),
                    UnderpinningServiceName = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    UnderpinningProviderName = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    UnderpinningCab = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    UnderpinningCertificateExpiryDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PublishedRegisterEntryRevisions", x => x.Id);
                    table.CheckConstraint("CK_PublishedRegisterEntryRevisions_PositiveKeys", "\"ServiceKey\" > 0 AND \"ServiceVersion\" > 0 AND \"FormatVersion\" > 0");
                });

            migrationBuilder.CreateIndex(
                name: "IX_PublishedRegisterEntryRevisions_EffectiveAtUtc_ActivityKind~",
                table: "PublishedRegisterEntryRevisions",
                columns: new[] { "EffectiveAtUtc", "ActivityKind", "ServiceKey" });

            migrationBuilder.CreateIndex(
                name: "IX_PublishedRegisterEntryRevisions_ServiceKey_EffectiveAtUtc_Id",
                table: "PublishedRegisterEntryRevisions",
                columns: new[] { "ServiceKey", "EffectiveAtUtc", "Id" });

            migrationBuilder.CreateIndex(
                name: "IX_PublishedRegisterEntryRevisions_SourceType_SourceId_Service~",
                table: "PublishedRegisterEntryRevisions",
                columns: new[] { "SourceType", "SourceId", "ServiceKey", "ActivityKind" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PublishedRegisterEntryRevisions");
        }
    }
}
