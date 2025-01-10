using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DVSRegister.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddRemovalRequestTimeColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "RemovalRequestTime",
                table: "ProviderProfile",
                type: "timestamp without time zone",
                nullable: true);

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

            migrationBuilder.UpdateData(
                table: "RemovalReasons",
                keyColumn: "RemovalReasonId",
                keyValue: 1,
                columns: new[] { "TimeCreated", "TimeUpdated" },
                values: new object[] { new DateTime(2025, 1, 9, 11, 16, 20, 850, DateTimeKind.Utc).AddTicks(6203), new DateTime(2025, 1, 9, 11, 16, 20, 850, DateTimeKind.Utc).AddTicks(6203) });

            migrationBuilder.UpdateData(
                table: "RemovalReasons",
                keyColumn: "RemovalReasonId",
                keyValue: 2,
                columns: new[] { "TimeCreated", "TimeUpdated" },
                values: new object[] { new DateTime(2025, 1, 9, 11, 16, 20, 850, DateTimeKind.Utc).AddTicks(6206), new DateTime(2025, 1, 9, 11, 16, 20, 850, DateTimeKind.Utc).AddTicks(6206) });

            migrationBuilder.UpdateData(
                table: "RemovalReasons",
                keyColumn: "RemovalReasonId",
                keyValue: 3,
                columns: new[] { "TimeCreated", "TimeUpdated" },
                values: new object[] { new DateTime(2025, 1, 9, 11, 16, 20, 850, DateTimeKind.Utc).AddTicks(6207), new DateTime(2025, 1, 9, 11, 16, 20, 850, DateTimeKind.Utc).AddTicks(6207) });

            migrationBuilder.UpdateData(
                table: "RemovalReasons",
                keyColumn: "RemovalReasonId",
                keyValue: 4,
                columns: new[] { "TimeCreated", "TimeUpdated" },
                values: new object[] { new DateTime(2025, 1, 9, 11, 16, 20, 850, DateTimeKind.Utc).AddTicks(6208), new DateTime(2025, 1, 9, 11, 16, 20, 850, DateTimeKind.Utc).AddTicks(6208) });

            migrationBuilder.UpdateData(
                table: "RemovalReasons",
                keyColumn: "RemovalReasonId",
                keyValue: 5,
                columns: new[] { "TimeCreated", "TimeUpdated" },
                values: new object[] { new DateTime(2025, 1, 9, 11, 16, 20, 850, DateTimeKind.Utc).AddTicks(6209), new DateTime(2025, 1, 9, 11, 16, 20, 850, DateTimeKind.Utc).AddTicks(6209) });

            migrationBuilder.UpdateData(
                table: "RemovalReasons",
                keyColumn: "RemovalReasonId",
                keyValue: 6,
                columns: new[] { "TimeCreated", "TimeUpdated" },
                values: new object[] { new DateTime(2025, 1, 9, 11, 16, 20, 850, DateTimeKind.Utc).AddTicks(6209), new DateTime(2025, 1, 9, 11, 16, 20, 850, DateTimeKind.Utc).AddTicks(6210) });

            migrationBuilder.UpdateData(
                table: "RemovalReasons",
                keyColumn: "RemovalReasonId",
                keyValue: 7,
                columns: new[] { "TimeCreated", "TimeUpdated" },
                values: new object[] { new DateTime(2025, 1, 9, 11, 16, 20, 850, DateTimeKind.Utc).AddTicks(6210), new DateTime(2025, 1, 9, 11, 16, 20, 850, DateTimeKind.Utc).AddTicks(6210) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RemovalRequestTime",
                table: "ProviderProfile");

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedTime",
                value: new DateTime(2025, 1, 7, 11, 2, 45, 176, DateTimeKind.Utc).AddTicks(7643));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedTime",
                value: new DateTime(2025, 1, 7, 11, 2, 45, 176, DateTimeKind.Utc).AddTicks(7646));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedTime",
                value: new DateTime(2025, 1, 7, 11, 2, 45, 176, DateTimeKind.Utc).AddTicks(7647));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedTime",
                value: new DateTime(2025, 1, 7, 11, 2, 45, 176, DateTimeKind.Utc).AddTicks(7648));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedTime",
                value: new DateTime(2025, 1, 7, 11, 2, 45, 176, DateTimeKind.Utc).AddTicks(7649));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedTime",
                value: new DateTime(2025, 1, 7, 11, 2, 45, 176, DateTimeKind.Utc).AddTicks(7650));

            migrationBuilder.UpdateData(
                table: "RemovalReasons",
                keyColumn: "RemovalReasonId",
                keyValue: 1,
                columns: new[] { "TimeCreated", "TimeUpdated" },
                values: new object[] { new DateTime(2025, 1, 7, 11, 2, 45, 176, DateTimeKind.Utc).AddTicks(7808), new DateTime(2025, 1, 7, 11, 2, 45, 176, DateTimeKind.Utc).AddTicks(7809) });

            migrationBuilder.UpdateData(
                table: "RemovalReasons",
                keyColumn: "RemovalReasonId",
                keyValue: 2,
                columns: new[] { "TimeCreated", "TimeUpdated" },
                values: new object[] { new DateTime(2025, 1, 7, 11, 2, 45, 176, DateTimeKind.Utc).AddTicks(7811), new DateTime(2025, 1, 7, 11, 2, 45, 176, DateTimeKind.Utc).AddTicks(7811) });

            migrationBuilder.UpdateData(
                table: "RemovalReasons",
                keyColumn: "RemovalReasonId",
                keyValue: 3,
                columns: new[] { "TimeCreated", "TimeUpdated" },
                values: new object[] { new DateTime(2025, 1, 7, 11, 2, 45, 176, DateTimeKind.Utc).AddTicks(7812), new DateTime(2025, 1, 7, 11, 2, 45, 176, DateTimeKind.Utc).AddTicks(7813) });

            migrationBuilder.UpdateData(
                table: "RemovalReasons",
                keyColumn: "RemovalReasonId",
                keyValue: 4,
                columns: new[] { "TimeCreated", "TimeUpdated" },
                values: new object[] { new DateTime(2025, 1, 7, 11, 2, 45, 176, DateTimeKind.Utc).AddTicks(7813), new DateTime(2025, 1, 7, 11, 2, 45, 176, DateTimeKind.Utc).AddTicks(7814) });

            migrationBuilder.UpdateData(
                table: "RemovalReasons",
                keyColumn: "RemovalReasonId",
                keyValue: 5,
                columns: new[] { "TimeCreated", "TimeUpdated" },
                values: new object[] { new DateTime(2025, 1, 7, 11, 2, 45, 176, DateTimeKind.Utc).AddTicks(7814), new DateTime(2025, 1, 7, 11, 2, 45, 176, DateTimeKind.Utc).AddTicks(7815) });

            migrationBuilder.UpdateData(
                table: "RemovalReasons",
                keyColumn: "RemovalReasonId",
                keyValue: 6,
                columns: new[] { "TimeCreated", "TimeUpdated" },
                values: new object[] { new DateTime(2025, 1, 7, 11, 2, 45, 176, DateTimeKind.Utc).AddTicks(7815), new DateTime(2025, 1, 7, 11, 2, 45, 176, DateTimeKind.Utc).AddTicks(7815) });

            migrationBuilder.UpdateData(
                table: "RemovalReasons",
                keyColumn: "RemovalReasonId",
                keyValue: 7,
                columns: new[] { "TimeCreated", "TimeUpdated" },
                values: new object[] { new DateTime(2025, 1, 7, 11, 2, 45, 176, DateTimeKind.Utc).AddTicks(7816), new DateTime(2025, 1, 7, 11, 2, 45, 176, DateTimeKind.Utc).AddTicks(7816) });
        }
    }
}
