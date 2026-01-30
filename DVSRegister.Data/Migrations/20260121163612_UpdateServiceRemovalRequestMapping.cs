using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DVSRegister.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateServiceRemovalRequestMapping : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ServiceRemovalRequest_ServiceId",
                table: "ServiceRemovalRequest");

            migrationBuilder.AddColumn<int>(
                name: "ServiceRemovalRequestId",
                table: "ActionLogs",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ServiceRemovalRequest_ServiceId",
                table: "ServiceRemovalRequest",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_ActionLogs_ServiceRemovalRequestId",
                table: "ActionLogs",
                column: "ServiceRemovalRequestId");

            migrationBuilder.AddForeignKey(
                name: "FK_ActionLogs_ServiceRemovalRequest_ServiceRemovalRequestId",
                table: "ActionLogs",
                column: "ServiceRemovalRequestId",
                principalTable: "ServiceRemovalRequest",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ActionLogs_ServiceRemovalRequest_ServiceRemovalRequestId",
                table: "ActionLogs");

            migrationBuilder.DropIndex(
                name: "IX_ServiceRemovalRequest_ServiceId",
                table: "ServiceRemovalRequest");

            migrationBuilder.DropIndex(
                name: "IX_ActionLogs_ServiceRemovalRequestId",
                table: "ActionLogs");

            migrationBuilder.DropColumn(
                name: "ServiceRemovalRequestId",
                table: "ActionLogs");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceRemovalRequest_ServiceId",
                table: "ServiceRemovalRequest",
                column: "ServiceId",
                unique: true);
        }
    }
}
