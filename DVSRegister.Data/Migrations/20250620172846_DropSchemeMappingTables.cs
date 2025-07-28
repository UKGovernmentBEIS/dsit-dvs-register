using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DVSRegister.Data.Migrations
{
    /// <inheritdoc />
    public partial class DropSchemeMappingTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SchemeGPG44Mapping");

            migrationBuilder.DropTable(
                name: "SchemeGPG45Mapping");

            migrationBuilder.DropColumn(
                name: "HasGpg44Mapping",
                table: "ServiceSupSchemeMapping");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "HasGpg44Mapping",
                table: "ServiceSupSchemeMapping",
                type: "boolean",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "SchemeGPG44Mapping",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    QualityLevelId = table.Column<int>(type: "integer", nullable: false),
                    ServiceSupSchemeMappingId = table.Column<int>(type: "integer", nullable: false),
                    SupplementarySchemeId = table.Column<int>(type: "integer", nullable: true)
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
                        name: "FK_SchemeGPG44Mapping_ServiceSupSchemeMapping_ServiceSupScheme~",
                        column: x => x.ServiceSupSchemeMappingId,
                        principalTable: "ServiceSupSchemeMapping",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SchemeGPG44Mapping_SupplementaryScheme_SupplementarySchemeId",
                        column: x => x.SupplementarySchemeId,
                        principalTable: "SupplementaryScheme",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SchemeGPG45Mapping",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdentityProfileId = table.Column<int>(type: "integer", nullable: false),
                    ServiceSupSchemeMappingId = table.Column<int>(type: "integer", nullable: false),
                    SupplementarySchemeId = table.Column<int>(type: "integer", nullable: true)
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
                        name: "FK_SchemeGPG45Mapping_ServiceSupSchemeMapping_ServiceSupScheme~",
                        column: x => x.ServiceSupSchemeMappingId,
                        principalTable: "ServiceSupSchemeMapping",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SchemeGPG45Mapping_SupplementaryScheme_SupplementarySchemeId",
                        column: x => x.SupplementarySchemeId,
                        principalTable: "SupplementaryScheme",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_SchemeGPG44Mapping_QualityLevelId",
                table: "SchemeGPG44Mapping",
                column: "QualityLevelId");

            migrationBuilder.CreateIndex(
                name: "IX_SchemeGPG44Mapping_ServiceSupSchemeMappingId",
                table: "SchemeGPG44Mapping",
                column: "ServiceSupSchemeMappingId");

            migrationBuilder.CreateIndex(
                name: "IX_SchemeGPG44Mapping_SupplementarySchemeId",
                table: "SchemeGPG44Mapping",
                column: "SupplementarySchemeId");

            migrationBuilder.CreateIndex(
                name: "IX_SchemeGPG45Mapping_IdentityProfileId",
                table: "SchemeGPG45Mapping",
                column: "IdentityProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_SchemeGPG45Mapping_ServiceSupSchemeMappingId",
                table: "SchemeGPG45Mapping",
                column: "ServiceSupSchemeMappingId");

            migrationBuilder.CreateIndex(
                name: "IX_SchemeGPG45Mapping_SupplementarySchemeId",
                table: "SchemeGPG45Mapping",
                column: "SupplementarySchemeId");
        }
    }
}
