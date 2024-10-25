using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DVSRegister.Data.Migrations
{
    /// <inheritdoc />
    public partial class SchemeOrderColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Order",
                table: "SupplementaryScheme",
                type: "integer",
                nullable: false,
                defaultValue: 0);

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

            migrationBuilder.UpdateData(
                table: "SupplementaryScheme",
                keyColumn: "Id",
                keyValue: 1,
                column: "Order",
                value: 2);

            migrationBuilder.UpdateData(
                table: "SupplementaryScheme",
                keyColumn: "Id",
                keyValue: 2,
                column: "Order",
                value: 1);

            migrationBuilder.UpdateData(
                table: "SupplementaryScheme",
                keyColumn: "Id",
                keyValue: 3,
                column: "Order",
                value: 3);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Order",
                table: "SupplementaryScheme");

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedTime",
                value: new DateTime(2024, 10, 25, 10, 14, 39, 690, DateTimeKind.Utc).AddTicks(8112));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedTime",
                value: new DateTime(2024, 10, 25, 10, 14, 39, 690, DateTimeKind.Utc).AddTicks(8115));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedTime",
                value: new DateTime(2024, 10, 25, 10, 14, 39, 690, DateTimeKind.Utc).AddTicks(8116));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedTime",
                value: new DateTime(2024, 10, 25, 10, 14, 39, 690, DateTimeKind.Utc).AddTicks(8117));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedTime",
                value: new DateTime(2024, 10, 25, 10, 14, 39, 690, DateTimeKind.Utc).AddTicks(8118));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedTime",
                value: new DateTime(2024, 10, 25, 10, 14, 39, 690, DateTimeKind.Utc).AddTicks(8119));
        }
    }
}
