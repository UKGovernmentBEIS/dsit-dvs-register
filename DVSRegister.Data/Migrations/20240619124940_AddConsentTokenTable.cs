using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DVSRegister.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddConsentTokenTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ConsentToken",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    CertificateReviewId = table.Column<int>(type: "integer", nullable: false),
                    TokenId = table.Column<string>(type: "text", nullable: false),
                    Token = table.Column<string>(type: "text", nullable: false),
                    CreatedTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConsentToken", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConsentToken_CertificateReview_CertificateReviewId",
                        column: x => x.CertificateReviewId,
                        principalTable: "CertificateReview",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ConsentToken_CertificateReviewId",
                table: "ConsentToken",
                column: "CertificateReviewId");

            migrationBuilder.CreateIndex(
                name: "IX_ConsentToken_Token",
                table: "ConsentToken",
                column: "Token");

            migrationBuilder.CreateIndex(
                name: "IX_ConsentToken_TokenId",
                table: "ConsentToken",
                column: "TokenId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ConsentToken");
        }
    }
}
