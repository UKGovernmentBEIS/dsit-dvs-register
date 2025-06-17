using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DVSRegister.Data.Migrations
{
    /// <inheritdoc />
    public partial class TrustFrameworkVersionTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PublicInterestCheck_ServiceId",
                table: "PublicInterestCheck");

            migrationBuilder.AddColumn<int>(
                name: "TrustFrameworkVersionId",
                table: "SupplementaryScheme",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ManualUnderPinningServiceId",
                table: "Service",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ServiceType",
                table: "Service",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TrustFrameworkVersionId",
                table: "Service",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UnderPinningServiceId",
                table: "Service",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TrustFrameworkVersionId",
                table: "Role",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TrustFrameworkVersionId",
                table: "QualityLevel",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TrustFrameworkVersionId",
                table: "IdentityProfile",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ManualUnderPinningService",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ServiceName = table.Column<string>(type: "text", nullable: false),
                    ProviderName = table.Column<string>(type: "text", nullable: false),
                    CABName = table.Column<string>(type: "text", nullable: false),
                    CertificateExpiryDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ManualUnderPinningService", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SchemeGPG44Mapping",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ServiceId = table.Column<int>(type: "integer", nullable: false),
                    QualityLevelId = table.Column<int>(type: "integer", nullable: false),
                    SupplementarySchemeId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SchemeGPG44Mapping", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SchemeGPG44Mapping_QualityLevel_QualityLevelId",
                        column: x => x.QualityLevelId,
                        principalTable: "QualityLevel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SchemeGPG44Mapping_Service_ServiceId",
                        column: x => x.ServiceId,
                        principalTable: "Service",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SchemeGPG44Mapping_SupplementaryScheme_SupplementarySchemeId",
                        column: x => x.SupplementarySchemeId,
                        principalTable: "SupplementaryScheme",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SchemeGPG45Mapping",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ServiceId = table.Column<int>(type: "integer", nullable: false),
                    IdentityProfileId = table.Column<int>(type: "integer", nullable: false),
                    SupplementarySchemeId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SchemeGPG45Mapping", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SchemeGPG45Mapping_IdentityProfile_IdentityProfileId",
                        column: x => x.IdentityProfileId,
                        principalTable: "IdentityProfile",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SchemeGPG45Mapping_Service_ServiceId",
                        column: x => x.ServiceId,
                        principalTable: "Service",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SchemeGPG45Mapping_SupplementaryScheme_SupplementarySchemeId",
                        column: x => x.SupplementarySchemeId,
                        principalTable: "SupplementaryScheme",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TrustFrameworkVersion",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TrustFrameworkName = table.Column<string>(type: "text", nullable: false),
                    Order = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrustFrameworkVersion", x => x.Id);
                });

            migrationBuilder.InsertData(
               table: "TrustFrameworkVersion",
               columns: new[] { "Id", "Order", "TrustFrameworkName" },
               values: new object[,]
               {
                    { 1, 1, "0.4 gamma" },
                    { 2, 2, "0.3 beta" }
               });


            migrationBuilder.InsertData(
                table: "Role",
                columns: new[] { "Id", "Order", "RoleName", "TrustFrameworkVersionId" },
                values: new object[,]
                {
                    { 4, 4, "Holder Service Provider (HSP)", 1 },
                    { 5, 5, "Component Service Provider (CSP)", 1 }
                });

            migrationBuilder.Sql(@"UPDATE ""IdentityProfile"" SET ""TrustFrameworkVersionId"" = 2");
            migrationBuilder.Sql(@"UPDATE ""QualityLevel"" SET ""TrustFrameworkVersionId"" = 2");
            migrationBuilder.Sql(@"UPDATE ""Role"" SET ""TrustFrameworkVersionId"" = 2 WHERE ""Id"" = 1 OR ""Id"" = 2 OR ""Id"" = 3 ");
            migrationBuilder.Sql(@"UPDATE ""SupplementaryScheme"" SET ""TrustFrameworkVersionId"" = 2");
            migrationBuilder.Sql(@"UPDATE ""Service"" SET ""TrustFrameworkVersionId"" = 2");




            migrationBuilder.CreateIndex(
                name: "IX_SupplementaryScheme_TrustFrameworkVersionId",
                table: "SupplementaryScheme",
                column: "TrustFrameworkVersionId");

            migrationBuilder.CreateIndex(
                name: "IX_Service_ManualUnderPinningServiceId",
                table: "Service",
                column: "ManualUnderPinningServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_Service_TrustFrameworkVersionId",
                table: "Service",
                column: "TrustFrameworkVersionId");

            migrationBuilder.CreateIndex(
                name: "IX_Service_UnderPinningServiceId",
                table: "Service",
                column: "UnderPinningServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_Role_TrustFrameworkVersionId",
                table: "Role",
                column: "TrustFrameworkVersionId");

            migrationBuilder.CreateIndex(
                name: "IX_QualityLevel_TrustFrameworkVersionId",
                table: "QualityLevel",
                column: "TrustFrameworkVersionId");

            migrationBuilder.CreateIndex(
                name: "IX_PublicInterestCheck_ServiceId",
                table: "PublicInterestCheck",
                column: "ServiceId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_IdentityProfile_TrustFrameworkVersionId",
                table: "IdentityProfile",
                column: "TrustFrameworkVersionId");

            migrationBuilder.CreateIndex(
                name: "IX_SchemeGPG44Mapping_QualityLevelId",
                table: "SchemeGPG44Mapping",
                column: "QualityLevelId");

            migrationBuilder.CreateIndex(
                name: "IX_SchemeGPG44Mapping_ServiceId",
                table: "SchemeGPG44Mapping",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_SchemeGPG44Mapping_SupplementarySchemeId",
                table: "SchemeGPG44Mapping",
                column: "SupplementarySchemeId");

            migrationBuilder.CreateIndex(
                name: "IX_SchemeGPG45Mapping_IdentityProfileId",
                table: "SchemeGPG45Mapping",
                column: "IdentityProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_SchemeGPG45Mapping_ServiceId",
                table: "SchemeGPG45Mapping",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_SchemeGPG45Mapping_SupplementarySchemeId",
                table: "SchemeGPG45Mapping",
                column: "SupplementarySchemeId");

            migrationBuilder.AddForeignKey(
                name: "FK_IdentityProfile_TrustFrameworkVersion_TrustFrameworkVersion~",
                table: "IdentityProfile",
                column: "TrustFrameworkVersionId",
                principalTable: "TrustFrameworkVersion",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_QualityLevel_TrustFrameworkVersion_TrustFrameworkVersionId",
                table: "QualityLevel",
                column: "TrustFrameworkVersionId",
                principalTable: "TrustFrameworkVersion",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Role_TrustFrameworkVersion_TrustFrameworkVersionId",
                table: "Role",
                column: "TrustFrameworkVersionId",
                principalTable: "TrustFrameworkVersion",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Service_ManualUnderPinningService_ManualUnderPinningService~",
                table: "Service",
                column: "ManualUnderPinningServiceId",
                principalTable: "ManualUnderPinningService",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Service_Service_UnderPinningServiceId",
                table: "Service",
                column: "UnderPinningServiceId",
                principalTable: "Service",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Service_TrustFrameworkVersion_TrustFrameworkVersionId",
                table: "Service",
                column: "TrustFrameworkVersionId",
                principalTable: "TrustFrameworkVersion",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SupplementaryScheme_TrustFrameworkVersion_TrustFrameworkVer~",
                table: "SupplementaryScheme",
                column: "TrustFrameworkVersionId",
                principalTable: "TrustFrameworkVersion",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IdentityProfile_TrustFrameworkVersion_TrustFrameworkVersion~",
                table: "IdentityProfile");

            migrationBuilder.DropForeignKey(
                name: "FK_QualityLevel_TrustFrameworkVersion_TrustFrameworkVersionId",
                table: "QualityLevel");

            migrationBuilder.DropForeignKey(
                name: "FK_Role_TrustFrameworkVersion_TrustFrameworkVersionId",
                table: "Role");

            migrationBuilder.DropForeignKey(
                name: "FK_Service_ManualUnderPinningService_ManualUnderPinningService~",
                table: "Service");

            migrationBuilder.DropForeignKey(
                name: "FK_Service_Service_UnderPinningServiceId",
                table: "Service");

            migrationBuilder.DropForeignKey(
                name: "FK_Service_TrustFrameworkVersion_TrustFrameworkVersionId",
                table: "Service");

            migrationBuilder.DropForeignKey(
                name: "FK_SupplementaryScheme_TrustFrameworkVersion_TrustFrameworkVer~",
                table: "SupplementaryScheme");

            migrationBuilder.DropTable(
                name: "ManualUnderPinningService");

            migrationBuilder.DropTable(
                name: "SchemeGPG44Mapping");

            migrationBuilder.DropTable(
                name: "SchemeGPG45Mapping");

            migrationBuilder.DropTable(
                name: "TrustFrameworkVersion");

            migrationBuilder.DropIndex(
                name: "IX_SupplementaryScheme_TrustFrameworkVersionId",
                table: "SupplementaryScheme");

            migrationBuilder.DropIndex(
                name: "IX_Service_ManualUnderPinningServiceId",
                table: "Service");

            migrationBuilder.DropIndex(
                name: "IX_Service_TrustFrameworkVersionId",
                table: "Service");

            migrationBuilder.DropIndex(
                name: "IX_Service_UnderPinningServiceId",
                table: "Service");

            migrationBuilder.DropIndex(
                name: "IX_Role_TrustFrameworkVersionId",
                table: "Role");

            migrationBuilder.DropIndex(
                name: "IX_QualityLevel_TrustFrameworkVersionId",
                table: "QualityLevel");

            migrationBuilder.DropIndex(
                name: "IX_PublicInterestCheck_ServiceId",
                table: "PublicInterestCheck");

            migrationBuilder.DropIndex(
                name: "IX_IdentityProfile_TrustFrameworkVersionId",
                table: "IdentityProfile");

            migrationBuilder.DeleteData(
                table: "Role",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Role",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DropColumn(
                name: "TrustFrameworkVersionId",
                table: "SupplementaryScheme");

            migrationBuilder.DropColumn(
                name: "ManualUnderPinningServiceId",
                table: "Service");

            migrationBuilder.DropColumn(
                name: "ServiceType",
                table: "Service");

            migrationBuilder.DropColumn(
                name: "TrustFrameworkVersionId",
                table: "Service");

            migrationBuilder.DropColumn(
                name: "UnderPinningServiceId",
                table: "Service");

            migrationBuilder.DropColumn(
                name: "TrustFrameworkVersionId",
                table: "Role");

            migrationBuilder.DropColumn(
                name: "TrustFrameworkVersionId",
                table: "QualityLevel");

            migrationBuilder.DropColumn(
                name: "TrustFrameworkVersionId",
                table: "IdentityProfile");

            migrationBuilder.CreateIndex(
                name: "IX_PublicInterestCheck_ServiceId",
                table: "PublicInterestCheck",
                column: "ServiceId");
        }
    }
}
