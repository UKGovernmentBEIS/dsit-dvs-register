using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DVSRegister.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCabEditedTime : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CabEditedTime",
                table: "ProviderProfile",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedTime",
                value: new DateTime(2024, 11, 11, 11, 8, 28, 459, DateTimeKind.Utc).AddTicks(6976));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedTime",
                value: new DateTime(2024, 11, 11, 11, 8, 28, 459, DateTimeKind.Utc).AddTicks(6980));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedTime",
                value: new DateTime(2024, 11, 11, 11, 8, 28, 459, DateTimeKind.Utc).AddTicks(6981));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedTime",
                value: new DateTime(2024, 11, 11, 11, 8, 28, 459, DateTimeKind.Utc).AddTicks(6983));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedTime",
                value: new DateTime(2024, 11, 11, 11, 8, 28, 459, DateTimeKind.Utc).AddTicks(6984));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedTime",
                value: new DateTime(2024, 11, 11, 11, 8, 28, 459, DateTimeKind.Utc).AddTicks(6986));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CabEditedTime",
                table: "ProviderProfile");

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedTime",
                value: new DateTime(2024, 10, 25, 11, 27, 48, 869, DateTimeKind.Utc).AddTicks(1304));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedTime",
                value: new DateTime(2024, 10, 25, 11, 27, 48, 869, DateTimeKind.Utc).AddTicks(1307));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedTime",
                value: new DateTime(2024, 10, 25, 11, 27, 48, 869, DateTimeKind.Utc).AddTicks(1308));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedTime",
                value: new DateTime(2024, 10, 25, 11, 27, 48, 869, DateTimeKind.Utc).AddTicks(1309));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedTime",
                value: new DateTime(2024, 10, 25, 11, 27, 48, 869, DateTimeKind.Utc).AddTicks(1310));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedTime",
                value: new DateTime(2024, 10, 25, 11, 27, 48, 869, DateTimeKind.Utc).AddTicks(1311));
        }
    }
}
