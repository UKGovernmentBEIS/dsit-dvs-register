using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DVSRegister.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateActionLogsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CertificateReviewId",
                table: "ActionLogs",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PublicInterestCheckId",
                table: "ActionLogs",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ActionLogs_CertificateReviewId",
                table: "ActionLogs",
                column: "CertificateReviewId");

            migrationBuilder.CreateIndex(
                name: "IX_ActionLogs_PublicInterestCheckId",
                table: "ActionLogs",
                column: "PublicInterestCheckId");

            migrationBuilder.AddForeignKey(
                name: "FK_ActionLogs_CertificateReview_CertificateReviewId",
                table: "ActionLogs",
                column: "CertificateReviewId",
                principalTable: "CertificateReview",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ActionLogs_PublicInterestCheck_PublicInterestCheckId",
                table: "ActionLogs",
                column: "PublicInterestCheckId",
                principalTable: "PublicInterestCheck",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ActionLogs_CertificateReview_CertificateReviewId",
                table: "ActionLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_ActionLogs_PublicInterestCheck_PublicInterestCheckId",
                table: "ActionLogs");

            migrationBuilder.DropIndex(
                name: "IX_ActionLogs_CertificateReviewId",
                table: "ActionLogs");

            migrationBuilder.DropIndex(
                name: "IX_ActionLogs_PublicInterestCheckId",
                table: "ActionLogs");

            migrationBuilder.DropColumn(
                name: "CertificateReviewId",
                table: "ActionLogs");

            migrationBuilder.DropColumn(
                name: "PublicInterestCheckId",
                table: "ActionLogs");
        }
    }
}
