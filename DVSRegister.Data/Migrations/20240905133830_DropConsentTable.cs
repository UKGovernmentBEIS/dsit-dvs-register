using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DVSRegister.Data.Migrations
{
    /// <inheritdoc />
    public partial class DropConsentTable : Migration
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
                value: new DateTime(2024, 9, 5, 13, 38, 29, 309, DateTimeKind.Utc).AddTicks(9536));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedTime",
                value: new DateTime(2024, 9, 5, 13, 38, 29, 309, DateTimeKind.Utc).AddTicks(9538));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedTime",
                value: new DateTime(2024, 9, 5, 13, 38, 29, 309, DateTimeKind.Utc).AddTicks(9540));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedTime",
                value: new DateTime(2024, 9, 5, 13, 38, 29, 309, DateTimeKind.Utc).AddTicks(9541));
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
                value: new DateTime(2024, 9, 4, 14, 50, 48, 676, DateTimeKind.Utc).AddTicks(2173));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedTime",
                value: new DateTime(2024, 9, 4, 14, 50, 48, 676, DateTimeKind.Utc).AddTicks(2180));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedTime",
                value: new DateTime(2024, 9, 4, 14, 50, 48, 676, DateTimeKind.Utc).AddTicks(2181));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedTime",
                value: new DateTime(2024, 9, 4, 14, 50, 48, 676, DateTimeKind.Utc).AddTicks(2182));

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
