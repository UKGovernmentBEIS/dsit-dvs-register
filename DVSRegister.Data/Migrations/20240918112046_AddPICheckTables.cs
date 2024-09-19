using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DVSRegister.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPICheckTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PICheckLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ReviewType = table.Column<int>(type: "integer", nullable: false),
                    Comment = table.Column<string>(type: "text", nullable: false),
                    PublicInterestCheckId = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    LogTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PICheckLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PICheckLogs_PublicInterestCheck_PublicInterestCheckId",
                        column: x => x.PublicInterestCheckId,
                        principalTable: "PublicInterestCheck",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PICheckLogs_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProceedPublishConsentToken",
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
                    table.PrimaryKey("PK_ProceedPublishConsentToken", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProceedPublishConsentToken_Service_ServiceId",
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
                value: new DateTime(2024, 9, 18, 11, 20, 45, 306, DateTimeKind.Utc).AddTicks(4761));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedTime",
                value: new DateTime(2024, 9, 18, 11, 20, 45, 306, DateTimeKind.Utc).AddTicks(4764));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedTime",
                value: new DateTime(2024, 9, 18, 11, 20, 45, 306, DateTimeKind.Utc).AddTicks(4765));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedTime",
                value: new DateTime(2024, 9, 18, 11, 20, 45, 306, DateTimeKind.Utc).AddTicks(4767));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedTime",
                value: new DateTime(2024, 9, 18, 11, 20, 45, 306, DateTimeKind.Utc).AddTicks(4768));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedTime",
                value: new DateTime(2024, 9, 18, 11, 20, 45, 306, DateTimeKind.Utc).AddTicks(4769));

            migrationBuilder.CreateIndex(
                name: "IX_PICheckLogs_PublicInterestCheckId",
                table: "PICheckLogs",
                column: "PublicInterestCheckId");

            migrationBuilder.CreateIndex(
                name: "IX_PICheckLogs_UserId",
                table: "PICheckLogs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ProceedPublishConsentToken_ServiceId",
                table: "ProceedPublishConsentToken",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_ProceedPublishConsentToken_Token",
                table: "ProceedPublishConsentToken",
                column: "Token");

            migrationBuilder.CreateIndex(
                name: "IX_ProceedPublishConsentToken_TokenId",
                table: "ProceedPublishConsentToken",
                column: "TokenId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PICheckLogs");

            migrationBuilder.DropTable(
                name: "ProceedPublishConsentToken");

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedTime",
                value: new DateTime(2024, 9, 16, 16, 11, 59, 427, DateTimeKind.Utc).AddTicks(2094));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedTime",
                value: new DateTime(2024, 9, 16, 16, 11, 59, 427, DateTimeKind.Utc).AddTicks(2099));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedTime",
                value: new DateTime(2024, 9, 16, 16, 11, 59, 427, DateTimeKind.Utc).AddTicks(2101));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedTime",
                value: new DateTime(2024, 9, 16, 16, 11, 59, 427, DateTimeKind.Utc).AddTicks(2102));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedTime",
                value: new DateTime(2024, 9, 16, 16, 11, 59, 427, DateTimeKind.Utc).AddTicks(2103));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedTime",
                value: new DateTime(2024, 9, 16, 16, 11, 59, 427, DateTimeKind.Utc).AddTicks(2105));
        }
    }
}
