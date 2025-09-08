using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DVSRegister.Data.Migrations
{
    /// <inheritdoc />
    public partial class DropServiceDraftTokenTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ServiceDraftToken");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ServiceDraftToken",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    ServiceDraftId = table.Column<int>(type: "integer", nullable: false),
                    CreatedTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ModifiedTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Token = table.Column<string>(type: "text", nullable: false),
                    TokenId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceDraftToken", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiceDraftToken_ServiceDraft_ServiceDraftId",
                        column: x => x.ServiceDraftId,
                        principalTable: "ServiceDraft",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ServiceDraftToken_ServiceDraftId",
                table: "ServiceDraftToken",
                column: "ServiceDraftId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceDraftToken_Token",
                table: "ServiceDraftToken",
                column: "Token");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceDraftToken_TokenId",
                table: "ServiceDraftToken",
                column: "TokenId");
        }
    }
}
