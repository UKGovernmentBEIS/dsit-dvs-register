using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using NpgsqlTypes;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DVSRegister.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCabTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Cab",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CabName = table.Column<string>(type: "text", nullable: false),
                    CreatedTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cab", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "QualityLevel",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Level = table.Column<string>(type: "text", nullable: false),
                    QualityType = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QualityLevel", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CabUser",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CabId = table.Column<int>(type: "integer", nullable: false),
                    CabEmail = table.Column<string>(type: "text", nullable: false),
                    CreatedTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CabUser", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CabUser_Cab_CabId",
                        column: x => x.CabId,
                        principalTable: "Cab",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProviderProfile",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RegisteredName = table.Column<string>(type: "text", nullable: false),
                    TradingName = table.Column<string>(type: "text", nullable: false),
                    HasRegistrationNumber = table.Column<bool>(type: "boolean", nullable: false),
                    CompanyRegistrationNumber = table.Column<string>(type: "text", nullable: true),
                    DUNSNumber = table.Column<string>(type: "text", nullable: true),
                    PrimaryContactFullName = table.Column<string>(type: "text", nullable: false),
                    PrimaryContactJobTitle = table.Column<string>(type: "text", nullable: false),
                    PrimaryContactEmail = table.Column<string>(type: "text", nullable: false),
                    PrimaryContactTelephoneNumber = table.Column<string>(type: "text", nullable: false),
                    SecondaryContactFullName = table.Column<string>(type: "text", nullable: false),
                    SecondaryContactJobTitle = table.Column<string>(type: "text", nullable: false),
                    SecondaryContactEmail = table.Column<string>(type: "text", nullable: false),
                    SecondaryContactTelephoneNumber = table.Column<string>(type: "text", nullable: false),
                    PublicContactEmail = table.Column<string>(type: "text", nullable: false),
                    ProviderTelephoneNumber = table.Column<string>(type: "text", nullable: false),
                    ProviderWebsiteAddress = table.Column<string>(type: "text", nullable: false),
                    CompanyId = table.Column<int>(type: "integer", nullable: false),
                    CabUserId = table.Column<int>(type: "integer", nullable: false),
                    ProviderStatus = table.Column<int>(type: "integer", nullable: false),
                    SearchVector = table.Column<NpgsqlTsVector>(type: "tsvector", nullable: false)
                        .Annotation("Npgsql:TsVectorConfig", "english")
                        .Annotation("Npgsql:TsVectorProperties", new[] { "TradingName", "RegisteredName" }),
                    CreatedTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ModifiedTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    PublishedTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProviderProfile", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProviderProfile_CabUser_CabUserId",
                        column: x => x.CabUserId,
                        principalTable: "CabUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Service",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProviderProfileId = table.Column<int>(type: "integer", nullable: false),
                    ServiceName = table.Column<string>(type: "text", nullable: false),
                    WebsiteAddress = table.Column<string>(type: "text", nullable: false),
                    CompanyAddress = table.Column<string>(type: "text", nullable: false),
                    HasSupplementarySchemes = table.Column<bool>(type: "boolean", nullable: false),
                    FileName = table.Column<string>(type: "text", nullable: false),
                    FileLink = table.Column<string>(type: "text", nullable: false),
                    FileSizeInKb = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    ConformityIssueDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ConformityExpiryDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CabUserId = table.Column<int>(type: "integer", nullable: false),
                    ServiceNumber = table.Column<int>(type: "integer", nullable: false),
                    TrustMarkNumber = table.Column<int>(type: "integer", nullable: false),
                    ServiceStatus = table.Column<int>(type: "integer", nullable: false),
                    CreatedTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ModifiedTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    PublishedTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Service", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Service_CabUser_CabUserId",
                        column: x => x.CabUserId,
                        principalTable: "CabUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Service_ProviderProfile_ProviderProfileId",
                        column: x => x.ProviderProfileId,
                        principalTable: "ProviderProfile",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServiceIdentityProfileMapping",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ServiceId = table.Column<int>(type: "integer", nullable: false),
                    IdentityProfileId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceIdentityProfileMapping", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiceIdentityProfileMapping_IdentityProfile_IdentityProfi~",
                        column: x => x.IdentityProfileId,
                        principalTable: "IdentityProfile",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServiceIdentityProfileMapping_Service_ServiceId",
                        column: x => x.ServiceId,
                        principalTable: "Service",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServiceQualityLevelMapping",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ServiceId = table.Column<int>(type: "integer", nullable: false),
                    QualityLevelId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceQualityLevelMapping", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiceQualityLevelMapping_QualityLevel_QualityLevelId",
                        column: x => x.QualityLevelId,
                        principalTable: "QualityLevel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServiceQualityLevelMapping_Service_ServiceId",
                        column: x => x.ServiceId,
                        principalTable: "Service",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServiceRoleMapping",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ServiceId = table.Column<int>(type: "integer", nullable: false),
                    RoleId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceRoleMapping", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiceRoleMapping_Role_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Role",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServiceRoleMapping_Service_ServiceId",
                        column: x => x.ServiceId,
                        principalTable: "Service",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServiceSupSchemeMapping",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ServiceId = table.Column<int>(type: "integer", nullable: false),
                    SupplementarySchemeId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceSupSchemeMapping", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiceSupSchemeMapping_Service_ServiceId",
                        column: x => x.ServiceId,
                        principalTable: "Service",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServiceSupSchemeMapping_SupplementaryScheme_SupplementarySc~",
                        column: x => x.SupplementarySchemeId,
                        principalTable: "SupplementaryScheme",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Cab",
                columns: new[] { "Id", "CabName", "CreatedTime" },
                values: new object[,]
                {
                    { 1, "EY", new DateTime(2024, 8, 8, 21, 15, 50, 24, DateTimeKind.Utc).AddTicks(6040) },
                    { 2, "DSIT", new DateTime(2024, 8, 8, 21, 15, 50, 24, DateTimeKind.Utc).AddTicks(6046) }
                });

            migrationBuilder.InsertData(
                table: "QualityLevel",
                columns: new[] { "Id", "Level", "QualityType" },
                values: new object[,]
                {
                    { 1, "Low", 1 },
                    { 2, "Medium", 1 },
                    { 3, "High", 1 },
                    { 4, "Low", 2 },
                    { 5, "Medium", 2 },
                    { 6, "High", 2 },
                    { 7, "Very High", 2 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_CabUser_CabId",
                table: "CabUser",
                column: "CabId");

            migrationBuilder.CreateIndex(
                name: "IX_ProviderProfile_CabUserId",
                table: "ProviderProfile",
                column: "CabUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ProviderProfile_SearchVector",
                table: "ProviderProfile",
                column: "SearchVector")
                .Annotation("Npgsql:IndexMethod", "GIN");

            migrationBuilder.CreateIndex(
                name: "IX_Service_CabUserId",
                table: "Service",
                column: "CabUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Service_ProviderProfileId",
                table: "Service",
                column: "ProviderProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceIdentityProfileMapping_IdentityProfileId",
                table: "ServiceIdentityProfileMapping",
                column: "IdentityProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceIdentityProfileMapping_ServiceId",
                table: "ServiceIdentityProfileMapping",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceQualityLevelMapping_QualityLevelId",
                table: "ServiceQualityLevelMapping",
                column: "QualityLevelId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceQualityLevelMapping_ServiceId",
                table: "ServiceQualityLevelMapping",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceRoleMapping_RoleId",
                table: "ServiceRoleMapping",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceRoleMapping_ServiceId",
                table: "ServiceRoleMapping",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceSupSchemeMapping_ServiceId",
                table: "ServiceSupSchemeMapping",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceSupSchemeMapping_SupplementarySchemeId",
                table: "ServiceSupSchemeMapping",
                column: "SupplementarySchemeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ServiceIdentityProfileMapping");

            migrationBuilder.DropTable(
                name: "ServiceQualityLevelMapping");

            migrationBuilder.DropTable(
                name: "ServiceRoleMapping");

            migrationBuilder.DropTable(
                name: "ServiceSupSchemeMapping");

            migrationBuilder.DropTable(
                name: "QualityLevel");

            migrationBuilder.DropTable(
                name: "Service");

            migrationBuilder.DropTable(
                name: "ProviderProfile");

            migrationBuilder.DropTable(
                name: "CabUser");

            migrationBuilder.DropTable(
                name: "Cab");
        }
    }
}
