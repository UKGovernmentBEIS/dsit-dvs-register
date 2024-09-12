using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DVSRegister.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddParentCompanyColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "HasParentCompany",
                table: "ProviderProfile",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ParentCompanyLocation",
                table: "ProviderProfile",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ParentCompanyRegisteredName",
                table: "ProviderProfile",
                type: "text",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedTime",
                value: new DateTime(2024, 9, 11, 11, 23, 42, 109, DateTimeKind.Utc).AddTicks(3674));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedTime",
                value: new DateTime(2024, 9, 11, 11, 23, 42, 109, DateTimeKind.Utc).AddTicks(3678));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedTime",
                value: new DateTime(2024, 9, 11, 11, 23, 42, 109, DateTimeKind.Utc).AddTicks(3680));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CabName", "CreatedTime" },
                values: new object[] { "Kantara", new DateTime(2024, 9, 11, 11, 23, 42, 109, DateTimeKind.Utc).AddTicks(3681) });

            migrationBuilder.InsertData(
                table: "Cab",
                columns: new[] { "Id", "CabName", "CreatedTime" },
                values: new object[,]
                {
                    { 6, "NQA", new DateTime(2024, 9, 11, 11, 23, 42, 109, DateTimeKind.Utc).AddTicks(3682) },
                    { 7, "BSI", new DateTime(2024, 9, 11, 11, 23, 42, 109, DateTimeKind.Utc).AddTicks(3683) }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DropColumn(
                name: "HasParentCompany",
                table: "ProviderProfile");

            migrationBuilder.DropColumn(
                name: "ParentCompanyLocation",
                table: "ProviderProfile");

            migrationBuilder.DropColumn(
                name: "ParentCompanyRegisteredName",
                table: "ProviderProfile");

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedTime",
                value: new DateTime(2024, 9, 9, 9, 52, 7, 272, DateTimeKind.Utc).AddTicks(8741));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedTime",
                value: new DateTime(2024, 9, 9, 9, 52, 7, 272, DateTimeKind.Utc).AddTicks(8744));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedTime",
                value: new DateTime(2024, 9, 9, 9, 52, 7, 272, DateTimeKind.Utc).AddTicks(8746));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CabName", "CreatedTime" },
                values: new object[] { "Kantara Initiative", new DateTime(2024, 9, 9, 9, 52, 7, 272, DateTimeKind.Utc).AddTicks(8747) });
        }
    }
}
