using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DVSRegister.Data.Migrations
{
    /// <inheritdoc />
    public partial class CreateTablesForCab : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CertificateInformation",
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
                    ServiceName = table.Column<string>(type: "text", nullable: false),
                    HasSupplementarySchemes = table.Column<bool>(type: "boolean", nullable: false),
                    FileName = table.Column<string>(type: "text", nullable: false),
                    FileLink = table.Column<string>(type: "text", nullable: false),
                    ConformityIssueDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ConformityExpiryDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CertificateInfoStatus = table.Column<int>(type: "integer", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ModifiedBy = table.Column<string>(type: "text", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
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
                name: "IdentityProfile",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdentityProfileName = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentityProfile", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Role",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RoleName = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Role", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SupplementaryScheme",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SchemeName = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupplementaryScheme", x => x.Id);
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

            migrationBuilder.InsertData(
                table: "IdentityProfile",
                columns: new[] { "Id", "IdentityProfileName" },
                values: new object[,]
                {
                    { 1, "L1A " },
                    { 2, "L1B " },
                    { 3, "L1C " },
                    { 4, "L2A " },
                    { 5, "L2B " },
                    { 6, "L3A " },
                    { 7, "M1A " },
                    { 8, "M1B " },
                    { 9, "M1C " },
                    { 10, "M1D " },
                    { 11, "M2A " },
                    { 12, "M2B " },
                    { 13, "M2C " },
                    { 14, "M3A " },
                    { 15, "H1A " },
                    { 16, "H1B " },
                    { 17, "H1C " },
                    { 18, "H2A " },
                    { 19, "H2B " },
                    { 20, "H2C " },
                    { 21, "H2D " },
                    { 22, "H2E " },
                    { 23, "H3A " },
                    { 24, "V1A " },
                    { 25, "V1B " },
                    { 26, "V1C " },
                    { 27, "V1D " },
                    { 28, "V2A " },
                    { 29, "V2B " },
                    { 30, "V2C " },
                    { 31, "V2D " },
                    { 32, "V3A " }
                });

            migrationBuilder.InsertData(
                table: "Role",
                columns: new[] { "Id", "RoleName" },
                values: new object[,]
                {
                    { 1, "Identity Service Provider (IDSP)" },
                    { 2, "Attribute Service Provider (ASP)" },
                    { 3, "Orchestration Service Provider (OSP)" }
                });

            migrationBuilder.InsertData(
                table: "SupplementaryScheme",
                columns: new[] { "Id", "SchemeName" },
                values: new object[,]
                {
                    { 1, "Right to Work" },
                    { 2, "Right to Rent" },
                    { 3, "Disclosure and Barring Service" }
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
                name: "IdentityProfile");

            migrationBuilder.DropTable(
                name: "Role");

            migrationBuilder.DropTable(
                name: "CertificateInformation");

            migrationBuilder.DropTable(
                name: "SupplementaryScheme");
        }
    }
}
