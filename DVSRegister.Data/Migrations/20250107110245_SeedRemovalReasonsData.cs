using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DVSRegister.Data.Migrations
{
    /// <inheritdoc />
    public partial class SeedRemovalReasonsData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "RemovalReasons",
                columns: new[] { "RemovalReasonId", "IsActiveReason", "RemovalReason", "RequiresAdditionalInfo", "TimeCreated", "TimeUpdated" },
                values: new object[,]
                {
                    { 1, true, "The service provider has requested to remove the whole provider record", false, new DateTime(2025, 1, 7, 11, 2, 45, 176, DateTimeKind.Utc).AddTicks(7808), new DateTime(2025, 1, 7, 11, 2, 45, 176, DateTimeKind.Utc).AddTicks(7809) },
                    { 3, true, "The service provider no longer exists", false, new DateTime(2025, 1, 7, 11, 2, 45, 176, DateTimeKind.Utc).AddTicks(7812), new DateTime(2025, 1, 7, 11, 2, 45, 176, DateTimeKind.Utc).AddTicks(7813) },
                    { 4, true, "The service provider has failed to provide the Secretary of State with information requested in accordance with a notice", false, new DateTime(2025, 1, 7, 11, 2, 45, 176, DateTimeKind.Utc).AddTicks(7813), new DateTime(2025, 1, 7, 11, 2, 45, 176, DateTimeKind.Utc).AddTicks(7814) },
                    { 5, true, "The Secretary of State is satisfied that the provider is failing to comply with the trust framework", false, new DateTime(2025, 1, 7, 11, 2, 45, 176, DateTimeKind.Utc).AddTicks(7814), new DateTime(2025, 1, 7, 11, 2, 45, 176, DateTimeKind.Utc).AddTicks(7815) },
                    { 6, true, "The Secretary of State is satisfied that the provider is failing to comply with the supplementary code", false, new DateTime(2025, 1, 7, 11, 2, 45, 176, DateTimeKind.Utc).AddTicks(7815), new DateTime(2025, 1, 7, 11, 2, 45, 176, DateTimeKind.Utc).AddTicks(7815) },
                    { 7, true, "The Secretary of State considers removal necessary is the interests of national security", false, new DateTime(2025, 1, 7, 11, 2, 45, 176, DateTimeKind.Utc).AddTicks(7816), new DateTime(2025, 1, 7, 11, 2, 45, 176, DateTimeKind.Utc).AddTicks(7816) }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "RemovalReasons",
                keyColumn: "RemovalReasonId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "RemovalReasons",
                keyColumn: "RemovalReasonId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "RemovalReasons",
                keyColumn: "RemovalReasonId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "RemovalReasons",
                keyColumn: "RemovalReasonId",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "RemovalReasons",
                keyColumn: "RemovalReasonId",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "RemovalReasons",
                keyColumn: "RemovalReasonId",
                keyValue: 7);
        }
    }
}
