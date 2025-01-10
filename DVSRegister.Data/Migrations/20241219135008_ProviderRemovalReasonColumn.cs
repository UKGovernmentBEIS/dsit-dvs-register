using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DVSRegister.Data.Migrations
{
    /// <inheritdoc />
    public partial class ProviderRemovalReasonColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RemovalReason",
                table: "ProviderProfile",
                type: "text",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedTime",
                value: new DateTime(2024, 12, 19, 13, 50, 7, 961, DateTimeKind.Utc).AddTicks(6518));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedTime",
                value: new DateTime(2024, 12, 19, 13, 50, 7, 961, DateTimeKind.Utc).AddTicks(6523));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedTime",
                value: new DateTime(2024, 12, 19, 13, 50, 7, 961, DateTimeKind.Utc).AddTicks(6525));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedTime",
                value: new DateTime(2024, 12, 19, 13, 50, 7, 961, DateTimeKind.Utc).AddTicks(6527));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedTime",
                value: new DateTime(2024, 12, 19, 13, 50, 7, 961, DateTimeKind.Utc).AddTicks(6528));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedTime",
                value: new DateTime(2024, 12, 19, 13, 50, 7, 961, DateTimeKind.Utc).AddTicks(6530));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RemovalReason",
                table: "ProviderProfile");

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedTime",
                value: new DateTime(2024, 12, 16, 11, 27, 16, 904, DateTimeKind.Utc).AddTicks(5909));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedTime",
                value: new DateTime(2024, 12, 16, 11, 27, 16, 904, DateTimeKind.Utc).AddTicks(5911));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedTime",
                value: new DateTime(2024, 12, 16, 11, 27, 16, 904, DateTimeKind.Utc).AddTicks(5912));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedTime",
                value: new DateTime(2024, 12, 16, 11, 27, 16, 904, DateTimeKind.Utc).AddTicks(5913));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedTime",
                value: new DateTime(2024, 12, 16, 11, 27, 16, 904, DateTimeKind.Utc).AddTicks(5914));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedTime",
                value: new DateTime(2024, 12, 16, 11, 27, 16, 904, DateTimeKind.Utc).AddTicks(5915));
        }
    }
}
