using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DVSRegister.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTrustMarkNumberColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_CompanyId",
                table: "TrustmarkNumber");

            migrationBuilder.AlterColumn<string>(
                name: "TrustMarkNumber",
                table: "TrustmarkNumber",
                type: "text",
                nullable: false,
                computedColumnSql: "LPAD(\"CompanyId\"::VARCHAR(4), 4, '0') || LPAD(\"ServiceNumber\"::VARCHAR(2), 2, '0')",
                stored: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldComputedColumnSql: "\"CompanyId\"::VARCHAR(4) || LPAD(\"ServiceNumber\"::VARCHAR(2), 2, '0')",
                oldStored: true);

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedTime",
                value: new DateTime(2024, 10, 10, 10, 20, 27, 462, DateTimeKind.Utc).AddTicks(9955));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedTime",
                value: new DateTime(2024, 10, 10, 10, 20, 27, 462, DateTimeKind.Utc).AddTicks(9958));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedTime",
                value: new DateTime(2024, 10, 10, 10, 20, 27, 462, DateTimeKind.Utc).AddTicks(9960));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedTime",
                value: new DateTime(2024, 10, 10, 10, 20, 27, 462, DateTimeKind.Utc).AddTicks(9961));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedTime",
                value: new DateTime(2024, 10, 10, 10, 20, 27, 462, DateTimeKind.Utc).AddTicks(9962));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedTime",
                value: new DateTime(2024, 10, 10, 10, 20, 27, 462, DateTimeKind.Utc).AddTicks(9964));

            migrationBuilder.AddCheckConstraint(
                name: "CK_CompanyId",
                table: "TrustmarkNumber",
                sql: "\"CompanyId\" BETWEEN 200 AND 9999");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_CompanyId",
                table: "TrustmarkNumber");

            migrationBuilder.AlterColumn<string>(
                name: "TrustMarkNumber",
                table: "TrustmarkNumber",
                type: "text",
                nullable: false,
                computedColumnSql: "\"CompanyId\"::VARCHAR(4) || LPAD(\"ServiceNumber\"::VARCHAR(2), 2, '0')",
                stored: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldComputedColumnSql: "LPAD(\"CompanyId\"::VARCHAR(4), 4, '0') || LPAD(\"ServiceNumber\"::VARCHAR(2), 2, '0')",
                oldStored: true);

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedTime",
                value: new DateTime(2024, 10, 9, 15, 22, 48, 471, DateTimeKind.Utc).AddTicks(929));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedTime",
                value: new DateTime(2024, 10, 9, 15, 22, 48, 471, DateTimeKind.Utc).AddTicks(933));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedTime",
                value: new DateTime(2024, 10, 9, 15, 22, 48, 471, DateTimeKind.Utc).AddTicks(935));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedTime",
                value: new DateTime(2024, 10, 9, 15, 22, 48, 471, DateTimeKind.Utc).AddTicks(936));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedTime",
                value: new DateTime(2024, 10, 9, 15, 22, 48, 471, DateTimeKind.Utc).AddTicks(937));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedTime",
                value: new DateTime(2024, 10, 9, 15, 22, 48, 471, DateTimeKind.Utc).AddTicks(938));

            migrationBuilder.AddCheckConstraint(
                name: "CK_CompanyId",
                table: "TrustmarkNumber",
                sql: "\"CompanyId\" BETWEEN 2000 AND 9999");
        }
    }
}
