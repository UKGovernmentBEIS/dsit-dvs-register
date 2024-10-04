using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DVSRegister.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemovedTrustmarkNumberColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TrustMarkNumber",
                table: "Service");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "ProviderProfile");

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedTime",
                value: new DateTime(2024, 10, 3, 13, 9, 11, 793, DateTimeKind.Utc).AddTicks(2376));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedTime",
                value: new DateTime(2024, 10, 3, 13, 9, 11, 793, DateTimeKind.Utc).AddTicks(2381));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedTime",
                value: new DateTime(2024, 10, 3, 13, 9, 11, 793, DateTimeKind.Utc).AddTicks(2382));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedTime",
                value: new DateTime(2024, 10, 3, 13, 9, 11, 793, DateTimeKind.Utc).AddTicks(2383));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedTime",
                value: new DateTime(2024, 10, 3, 13, 9, 11, 793, DateTimeKind.Utc).AddTicks(2385));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedTime",
                value: new DateTime(2024, 10, 3, 13, 9, 11, 793, DateTimeKind.Utc).AddTicks(2386));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TrustMarkNumber",
                table: "Service",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CompanyId",
                table: "ProviderProfile",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedTime",
                value: new DateTime(2024, 10, 3, 12, 56, 11, 760, DateTimeKind.Utc).AddTicks(2256));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedTime",
                value: new DateTime(2024, 10, 3, 12, 56, 11, 760, DateTimeKind.Utc).AddTicks(2260));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedTime",
                value: new DateTime(2024, 10, 3, 12, 56, 11, 760, DateTimeKind.Utc).AddTicks(2261));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedTime",
                value: new DateTime(2024, 10, 3, 12, 56, 11, 760, DateTimeKind.Utc).AddTicks(2262));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedTime",
                value: new DateTime(2024, 10, 3, 12, 56, 11, 760, DateTimeKind.Utc).AddTicks(2264));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedTime",
                value: new DateTime(2024, 10, 3, 12, 56, 11, 760, DateTimeKind.Utc).AddTicks(2265));
        }
    }
}
