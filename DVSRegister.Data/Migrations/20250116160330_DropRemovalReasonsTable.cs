using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DVSRegister.Data.Migrations
{
    /// <inheritdoc />
    public partial class DropRemovalReasonsTable : Migration
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
                nullable: true
            );

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedTime",
                value: new DateTime(2025, 1, 16, 16, 3, 29, 320, DateTimeKind.Utc).AddTicks(167));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedTime",
                value: new DateTime(2025, 1, 16, 16, 3, 29, 320, DateTimeKind.Utc).AddTicks(172));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedTime",
                value: new DateTime(2025, 1, 16, 16, 3, 29, 320, DateTimeKind.Utc).AddTicks(173));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedTime",
                value: new DateTime(2025, 1, 16, 16, 3, 29, 320, DateTimeKind.Utc).AddTicks(174));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedTime",
                value: new DateTime(2025, 1, 16, 16, 3, 29, 320, DateTimeKind.Utc).AddTicks(175));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedTime",
                value: new DateTime(2025, 1, 16, 16, 3, 29, 320, DateTimeKind.Utc).AddTicks(176));
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
                value: new DateTime(2025, 1, 14, 11, 23, 49, 142, DateTimeKind.Utc).AddTicks(3348));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedTime",
                value: new DateTime(2025, 1, 14, 11, 23, 49, 142, DateTimeKind.Utc).AddTicks(3353));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedTime",
                value: new DateTime(2025, 1, 14, 11, 23, 49, 142, DateTimeKind.Utc).AddTicks(3354));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedTime",
                value: new DateTime(2025, 1, 14, 11, 23, 49, 142, DateTimeKind.Utc).AddTicks(3355));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedTime",
                value: new DateTime(2025, 1, 14, 11, 23, 49, 142, DateTimeKind.Utc).AddTicks(3356));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedTime",
                value: new DateTime(2025, 1, 14, 11, 23, 49, 142, DateTimeKind.Utc).AddTicks(3357));

            migrationBuilder.InsertData(
                table: "RemovalReasons",
                columns: new[] { "RemovalReasonId", "IsActiveReason", "RemovalReason", "RequiresAdditionalInfo", "TimeCreated", "TimeUpdated" },
                values: new object[,]
                {
                    { 1, true, "The service provider has requested to remove the whole provider record", false, new DateTime(2025, 1, 14, 11, 23, 49, 142, DateTimeKind.Utc).AddTicks(3511), new DateTime(2025, 1, 14, 11, 23, 49, 142, DateTimeKind.Utc).AddTicks(3511) },
                    { 3, true, "The service provider no longer exists", false, new DateTime(2025, 1, 14, 11, 23, 49, 142, DateTimeKind.Utc).AddTicks(3514), new DateTime(2025, 1, 14, 11, 23, 49, 142, DateTimeKind.Utc).AddTicks(3514) },
                    { 4, true, "The service provider has failed to provide the Secretary of State with information requested in accordance with a notice", false, new DateTime(2025, 1, 14, 11, 23, 49, 142, DateTimeKind.Utc).AddTicks(3515), new DateTime(2025, 1, 14, 11, 23, 49, 142, DateTimeKind.Utc).AddTicks(3515) },
                    { 5, true, "The Secretary of State is satisfied that the provider is failing to comply with the trust framework", false, new DateTime(2025, 1, 14, 11, 23, 49, 142, DateTimeKind.Utc).AddTicks(3516), new DateTime(2025, 1, 14, 11, 23, 49, 142, DateTimeKind.Utc).AddTicks(3516) },
                    { 6, true, "The Secretary of State is satisfied that the provider is failing to comply with the supplementary code", false, new DateTime(2025, 1, 14, 11, 23, 49, 142, DateTimeKind.Utc).AddTicks(3517), new DateTime(2025, 1, 14, 11, 23, 49, 142, DateTimeKind.Utc).AddTicks(3517) },
                    { 7, true, "The Secretary of State considers removal necessary is the interests of national security", false, new DateTime(2025, 1, 14, 11, 23, 49, 142, DateTimeKind.Utc).AddTicks(3518), new DateTime(2025, 1, 14, 11, 23, 49, 142, DateTimeKind.Utc).AddTicks(3518) }
                });
        }
    }
}
