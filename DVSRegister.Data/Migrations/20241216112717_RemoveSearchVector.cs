using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NpgsqlTypes;

#nullable disable

namespace DVSRegister.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveSearchVector : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Service_SearchVector",
                table: "Service");

            migrationBuilder.DropIndex(
                name: "IX_ProviderProfile_SearchVector",
                table: "ProviderProfile");

            migrationBuilder.DropColumn(
                name: "SearchVector",
                table: "Service");

            migrationBuilder.DropColumn(
                name: "SearchVector",
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<NpgsqlTsVector>(
                name: "SearchVector",
                table: "Service",
                type: "tsvector",
                nullable: false)
                .Annotation("Npgsql:TsVectorConfig", "english")
                .Annotation("Npgsql:TsVectorProperties", new[] { "ServiceName" });

            migrationBuilder.AddColumn<NpgsqlTsVector>(
                name: "SearchVector",
                table: "ProviderProfile",
                type: "tsvector",
                nullable: false)
                .Annotation("Npgsql:TsVectorConfig", "english")
                .Annotation("Npgsql:TsVectorProperties", new[] { "RegisteredName", "TradingName" });

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedTime",
                value: new DateTime(2024, 11, 22, 15, 55, 39, 258, DateTimeKind.Utc).AddTicks(2100));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedTime",
                value: new DateTime(2024, 11, 22, 15, 55, 39, 258, DateTimeKind.Utc).AddTicks(2110));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedTime",
                value: new DateTime(2024, 11, 22, 15, 55, 39, 258, DateTimeKind.Utc).AddTicks(2110));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedTime",
                value: new DateTime(2024, 11, 22, 15, 55, 39, 258, DateTimeKind.Utc).AddTicks(2110));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedTime",
                value: new DateTime(2024, 11, 22, 15, 55, 39, 258, DateTimeKind.Utc).AddTicks(2110));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedTime",
                value: new DateTime(2024, 11, 22, 15, 55, 39, 258, DateTimeKind.Utc).AddTicks(2110));

            migrationBuilder.CreateIndex(
                name: "IX_Service_SearchVector",
                table: "Service",
                column: "SearchVector")
                .Annotation("Npgsql:IndexMethod", "GIN");

            migrationBuilder.CreateIndex(
                name: "IX_ProviderProfile_SearchVector",
                table: "ProviderProfile",
                column: "SearchVector")
                .Annotation("Npgsql:IndexMethod", "GIN");
        }
    }
}
