using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DVSRegister.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveOldConsentTokenTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ConsentToken");

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedTime",
                value: new DateTime(2024, 10, 3, 13, 11, 59, 551, DateTimeKind.Utc).AddTicks(913));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedTime",
                value: new DateTime(2024, 10, 3, 13, 11, 59, 551, DateTimeKind.Utc).AddTicks(917));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedTime",
                value: new DateTime(2024, 10, 3, 13, 11, 59, 551, DateTimeKind.Utc).AddTicks(918));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedTime",
                value: new DateTime(2024, 10, 3, 13, 11, 59, 551, DateTimeKind.Utc).AddTicks(920));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedTime",
                value: new DateTime(2024, 10, 3, 13, 11, 59, 551, DateTimeKind.Utc).AddTicks(921));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedTime",
                value: new DateTime(2024, 10, 3, 13, 11, 59, 551, DateTimeKind.Utc).AddTicks(922));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ConsentToken",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    CreatedTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Token = table.Column<string>(type: "text", nullable: false),
                    TokenId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConsentToken", x => x.Id);
                });

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

            migrationBuilder.CreateIndex(
                name: "IX_ConsentToken_Token",
                table: "ConsentToken",
                column: "Token");

            migrationBuilder.CreateIndex(
                name: "IX_ConsentToken_TokenId",
                table: "ConsentToken",
                column: "TokenId");
        }
    }
}
