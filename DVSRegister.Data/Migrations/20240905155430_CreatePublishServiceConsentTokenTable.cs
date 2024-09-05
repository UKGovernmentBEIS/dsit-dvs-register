using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DVSRegister.Data.Migrations
{
    /// <inheritdoc />
    public partial class CreatePublishServiceConsentTokenTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PublishServiceConsentToken",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    TokenId = table.Column<string>(type: "text", nullable: false),
                    Token = table.Column<string>(type: "text", nullable: false),
                    ServiceId = table.Column<int>(type: "integer", nullable: false),
                    CreatedTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PublishServiceConsentToken", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PublishServiceConsentToken_Service_ServiceId",
                        column: x => x.ServiceId,
                        principalTable: "Service",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedTime",
                value: new DateTime(2024, 9, 5, 15, 54, 29, 568, DateTimeKind.Utc).AddTicks(7456));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedTime",
                value: new DateTime(2024, 9, 5, 15, 54, 29, 568, DateTimeKind.Utc).AddTicks(7459));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedTime",
                value: new DateTime(2024, 9, 5, 15, 54, 29, 568, DateTimeKind.Utc).AddTicks(7461));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedTime",
                value: new DateTime(2024, 9, 5, 15, 54, 29, 568, DateTimeKind.Utc).AddTicks(7462));

            migrationBuilder.CreateIndex(
                name: "IX_PublishServiceConsentToken_ServiceId",
                table: "PublishServiceConsentToken",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_PublishServiceConsentToken_Token",
                table: "PublishServiceConsentToken",
                column: "Token");

            migrationBuilder.CreateIndex(
                name: "IX_PublishServiceConsentToken_TokenId",
                table: "PublishServiceConsentToken",
                column: "TokenId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PublishServiceConsentToken");

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
    }
}
