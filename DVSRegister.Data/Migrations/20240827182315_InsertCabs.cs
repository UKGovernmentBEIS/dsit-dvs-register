using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DVSRegister.Data.Migrations
{
    /// <inheritdoc />
    public partial class InsertCabs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedTime",
                value: new DateTime(2024, 8, 27, 18, 23, 14, 542, DateTimeKind.Utc).AddTicks(2042));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedTime",
                value: new DateTime(2024, 8, 27, 18, 23, 14, 542, DateTimeKind.Utc).AddTicks(2046));

            migrationBuilder.InsertData(
                table: "Cab",
                columns: new[] { "Id", "CabName", "CreatedTime" },
                values: new object[,]
                {
                    { 3, "ACCS", new DateTime(2024, 8, 27, 18, 23, 14, 542, DateTimeKind.Utc).AddTicks(2048) },
                    { 4, "Kantara Initiative", new DateTime(2024, 8, 27, 18, 23, 14, 542, DateTimeKind.Utc).AddTicks(2049) }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedTime",
                value: new DateTime(2024, 8, 27, 9, 48, 52, 800, DateTimeKind.Utc).AddTicks(8205));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedTime",
                value: new DateTime(2024, 8, 27, 9, 48, 52, 800, DateTimeKind.Utc).AddTicks(8207));
        }
    }
}
