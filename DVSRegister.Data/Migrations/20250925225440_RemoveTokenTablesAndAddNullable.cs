using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DVSRegister.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveTokenTablesAndAddNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RemoveTokenServiceMapping");

            migrationBuilder.DropTable(
                name: "RemoveProviderToken");

            migrationBuilder.AlterColumn<DateTime>(
                name: "RemovalRequestTime",
                table: "ProviderRemovalRequest",
                type: "timestamp without time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "RemovalRequestTime",
                table: "ProviderRemovalRequest",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "RemoveProviderToken",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProviderProfileId = table.Column<int>(type: "integer", nullable: false),
                    CreatedTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ModifiedTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Token = table.Column<string>(type: "text", nullable: false),
                    TokenId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RemoveProviderToken", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RemoveProviderToken_ProviderProfile_ProviderProfileId",
                        column: x => x.ProviderProfileId,
                        principalTable: "ProviderProfile",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RemoveTokenServiceMapping",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RemoveProviderTokenId = table.Column<int>(type: "integer", nullable: false),
                    ServiceId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RemoveTokenServiceMapping", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RemoveTokenServiceMapping_RemoveProviderToken_RemoveProvide~",
                        column: x => x.RemoveProviderTokenId,
                        principalTable: "RemoveProviderToken",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RemoveTokenServiceMapping_Service_ServiceId",
                        column: x => x.ServiceId,
                        principalTable: "Service",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RemoveProviderToken_ProviderProfileId",
                table: "RemoveProviderToken",
                column: "ProviderProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_RemoveTokenServiceMapping_RemoveProviderTokenId",
                table: "RemoveTokenServiceMapping",
                column: "RemoveProviderTokenId");

            migrationBuilder.CreateIndex(
                name: "IX_RemoveTokenServiceMapping_ServiceId",
                table: "RemoveTokenServiceMapping",
                column: "ServiceId");
        }
    }
}
