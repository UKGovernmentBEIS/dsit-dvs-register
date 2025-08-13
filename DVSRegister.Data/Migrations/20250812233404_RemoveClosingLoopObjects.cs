using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DVSRegister.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveClosingLoopObjects : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProceedPublishConsentToken");

            migrationBuilder.DropColumn(
                name: "ClosingLoopTokenStatus",
                table: "Service");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ClosingLoopTokenStatus",
                table: "Service",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ProceedPublishConsentToken",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    ServiceId = table.Column<int>(type: "integer", nullable: false),
                    CreatedTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ModifiedTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Token = table.Column<string>(type: "text", nullable: false),
                    TokenId = table.Column<string>(type: "text", nullable: false)
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
    }
}
