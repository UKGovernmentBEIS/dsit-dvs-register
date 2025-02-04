using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DVSRegister.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddRemovalReasonCabColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RemovalReasonByCab",
                table: "Service",
                type: "text",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedTime",
                value: new DateTime(2025, 1, 21, 10, 41, 8, 288, DateTimeKind.Utc).AddTicks(8647));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedTime",
                value: new DateTime(2025, 1, 21, 10, 41, 8, 288, DateTimeKind.Utc).AddTicks(8650));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedTime",
                value: new DateTime(2025, 1, 21, 10, 41, 8, 288, DateTimeKind.Utc).AddTicks(8652));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedTime",
                value: new DateTime(2025, 1, 21, 10, 41, 8, 288, DateTimeKind.Utc).AddTicks(8654));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedTime",
                value: new DateTime(2025, 1, 21, 10, 41, 8, 288, DateTimeKind.Utc).AddTicks(8655));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedTime",
                value: new DateTime(2025, 1, 21, 10, 41, 8, 288, DateTimeKind.Utc).AddTicks(8657));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RemovalReasonByCab",
                table: "Service");

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
    }
}
