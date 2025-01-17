using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DVSRegister.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddColumnsUserServiceProvider : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Profile",
                table: "User",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RemovalRequestTime",
                table: "Service",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RemovedTime",
                table: "Service",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ServiceRemovalReason",
                table: "Service",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RemovedTime",
                table: "ProviderProfile",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedTime",
                value: new DateTime(2025, 1, 17, 10, 22, 35, 843, DateTimeKind.Utc).AddTicks(5869));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedTime",
                value: new DateTime(2025, 1, 17, 10, 22, 35, 843, DateTimeKind.Utc).AddTicks(5872));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedTime",
                value: new DateTime(2025, 1, 17, 10, 22, 35, 843, DateTimeKind.Utc).AddTicks(5873));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedTime",
                value: new DateTime(2025, 1, 17, 10, 22, 35, 843, DateTimeKind.Utc).AddTicks(5874));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedTime",
                value: new DateTime(2025, 1, 17, 10, 22, 35, 843, DateTimeKind.Utc).AddTicks(5875));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedTime",
                value: new DateTime(2025, 1, 17, 10, 22, 35, 843, DateTimeKind.Utc).AddTicks(5876));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Profile",
                table: "User");

            migrationBuilder.DropColumn(
                name: "RemovalRequestTime",
                table: "Service");

            migrationBuilder.DropColumn(
                name: "RemovedTime",
                table: "Service");

            migrationBuilder.DropColumn(
                name: "ServiceRemovalReason",
                table: "Service");

            migrationBuilder.DropColumn(
                name: "RemovedTime",
                table: "ProviderProfile");

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
    }
}
