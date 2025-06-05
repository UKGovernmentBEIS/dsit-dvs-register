using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DVSRegister.Data.Migrations
{
    /// <inheritdoc />
    public partial class ProviderProfileCabMapping : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                table: "CabUser",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: true);

            migrationBuilder.CreateTable(
                name: "ProviderProfileCabMapping",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProviderId = table.Column<int>(type: "integer", nullable: false),
                    CabId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProviderProfileCabMapping", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProviderProfileCabMapping_Cab_CabId",
                        column: x => x.CabId,
                        principalTable: "Cab",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProviderProfileCabMapping_ProviderProfile_ProviderId",
                        column: x => x.ProviderId,
                        principalTable: "ProviderProfile",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProviderProfileCabMapping_CabId",
                table: "ProviderProfileCabMapping",
                column: "CabId");

            migrationBuilder.CreateIndex(
                name: "IX_ProviderProfileCabMapping_ProviderId",
                table: "ProviderProfileCabMapping",
                column: "ProviderId");

            // Populate the new mapping table with existing data
            migrationBuilder.Sql(@" INSERT INTO ""ProviderProfileCabMapping"" (""ProviderId"", ""CabId"")
            SELECT  p.""Id"", cu.""CabId""  FROM  ""ProviderProfile""  p  INNER JOIN  ""CabUser""  cu
            ON p.""CabUserId"" = cu.""Id"";
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProviderProfileCabMapping");

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                table: "CabUser",
                type: "boolean",
                nullable: false,
                defaultValue: true,
                oldClrType: typeof(bool),
                oldType: "boolean");
        }
    }
}
