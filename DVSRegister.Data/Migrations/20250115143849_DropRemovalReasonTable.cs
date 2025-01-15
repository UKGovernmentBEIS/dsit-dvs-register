using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DVSRegister.Data.Migrations
{
    /// <inheritdoc />
    public partial class DropRemovalReasonTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RemovalReasons");

            migrationBuilder.DropColumn(
                name: "RemovalReason",
                table: "ProviderProfile"
            );

            migrationBuilder.AddColumn<int>(
                name: "RemovalReason",
                table: "ProviderProfile",
                type: "integer",
                nullable: false,
                defaultValue: 0
            );

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedTime",
                value: new DateTime(2025, 1, 15, 14, 38, 48, 915, DateTimeKind.Utc).AddTicks(2089));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedTime",
                value: new DateTime(2025, 1, 15, 14, 38, 48, 915, DateTimeKind.Utc).AddTicks(2093));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedTime",
                value: new DateTime(2025, 1, 15, 14, 38, 48, 915, DateTimeKind.Utc).AddTicks(2093));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedTime",
                value: new DateTime(2025, 1, 15, 14, 38, 48, 915, DateTimeKind.Utc).AddTicks(2094));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedTime",
                value: new DateTime(2025, 1, 15, 14, 38, 48, 915, DateTimeKind.Utc).AddTicks(2095));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedTime",
                value: new DateTime(2025, 1, 15, 14, 38, 48, 915, DateTimeKind.Utc).AddTicks(2096));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RemovalReason",
                table: "ProviderProfile"
            );

            migrationBuilder.AddColumn<string>(
                name: "RemovalReason",
                table: "ProviderProfile",
                type: "text",
                nullable: true
            );

            migrationBuilder.CreateTable(
                name: "RemovalReasons",
                columns: table => new
                {
                    RemovalReasonId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IsActiveReason = table.Column<bool>(type: "boolean", nullable: false),
                    RemovalReason = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    RequiresAdditionalInfo = table.Column<bool>(type: "boolean", nullable: false),
                    TimeCreated = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    TimeUpdated = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RemovalReasons", x => x.RemovalReasonId);
                });

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedTime",
                value: new DateTime(2025, 1, 9, 11, 16, 20, 850, DateTimeKind.Utc).AddTicks(5984));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedTime",
                value: new DateTime(2025, 1, 9, 11, 16, 20, 850, DateTimeKind.Utc).AddTicks(5988));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedTime",
                value: new DateTime(2025, 1, 9, 11, 16, 20, 850, DateTimeKind.Utc).AddTicks(5989));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedTime",
                value: new DateTime(2025, 1, 9, 11, 16, 20, 850, DateTimeKind.Utc).AddTicks(5990));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedTime",
                value: new DateTime(2025, 1, 9, 11, 16, 20, 850, DateTimeKind.Utc).AddTicks(5991));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedTime",
                value: new DateTime(2025, 1, 9, 11, 16, 20, 850, DateTimeKind.Utc).AddTicks(5992));

            migrationBuilder.InsertData(
                table: "RemovalReasons",
                columns: new[] { "RemovalReasonId", "IsActiveReason", "RemovalReason", "RequiresAdditionalInfo", "TimeCreated", "TimeUpdated" },
                values: new object[,]
                {
                    { 1, true, "The service provider has requested to remove the whole provider record", false, new DateTime(2025, 1, 9, 11, 16, 20, 850, DateTimeKind.Utc).AddTicks(6203), new DateTime(2025, 1, 9, 11, 16, 20, 850, DateTimeKind.Utc).AddTicks(6203) },
                    { 2, true, "The Conformity Assessment Body has withdrawn the certificate for the service and there are no other services published for this provider", true, new DateTime(2025, 1, 9, 11, 16, 20, 850, DateTimeKind.Utc).AddTicks(6206), new DateTime(2025, 1, 9, 11, 16, 20, 850, DateTimeKind.Utc).AddTicks(6206) },
                    { 3, true, "The service provider no longer exists", false, new DateTime(2025, 1, 9, 11, 16, 20, 850, DateTimeKind.Utc).AddTicks(6207), new DateTime(2025, 1, 9, 11, 16, 20, 850, DateTimeKind.Utc).AddTicks(6207) },
                    { 4, true, "The service provider has failed to provide the Secretary of State with information requested in accordance with a notice", false, new DateTime(2025, 1, 9, 11, 16, 20, 850, DateTimeKind.Utc).AddTicks(6208), new DateTime(2025, 1, 9, 11, 16, 20, 850, DateTimeKind.Utc).AddTicks(6208) },
                    { 5, true, "The Secretary of State is satisfied that the provider is failing to comply with the trust framework", false, new DateTime(2025, 1, 9, 11, 16, 20, 850, DateTimeKind.Utc).AddTicks(6209), new DateTime(2025, 1, 9, 11, 16, 20, 850, DateTimeKind.Utc).AddTicks(6209) },
                    { 6, true, "The Secretary of State is satisfied that the provider is failing to comply with the supplementary code", false, new DateTime(2025, 1, 9, 11, 16, 20, 850, DateTimeKind.Utc).AddTicks(6209), new DateTime(2025, 1, 9, 11, 16, 20, 850, DateTimeKind.Utc).AddTicks(6210) },
                    { 7, true, "The Secretary of State considers removal necessary is the interests of national security", false, new DateTime(2025, 1, 9, 11, 16, 20, 850, DateTimeKind.Utc).AddTicks(6210), new DateTime(2025, 1, 9, 11, 16, 20, 850, DateTimeKind.Utc).AddTicks(6210) }
                });
        }
    }
}
