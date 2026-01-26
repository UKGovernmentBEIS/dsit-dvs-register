using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DVSRegister.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateActionLogAddProviderRemovalRequestId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ProviderRemovalRequestServiceMapping_ServiceId",
                table: "ProviderRemovalRequestServiceMapping");

            migrationBuilder.AddColumn<int>(
                name: "ProviderRemovalRequestId",
                table: "ActionLogs",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProviderRemovalRequestServiceMapping_ServiceId",
                table: "ProviderRemovalRequestServiceMapping",
                column: "ServiceId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ActionLogs_ProviderRemovalRequestId",
                table: "ActionLogs",
                column: "ProviderRemovalRequestId");

            migrationBuilder.AddForeignKey(
                name: "FK_ActionLogs_ProviderRemovalRequest_ProviderRemovalRequestId",
                table: "ActionLogs",
                column: "ProviderRemovalRequestId",
                principalTable: "ProviderRemovalRequest",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ActionLogs_ProviderRemovalRequest_ProviderRemovalRequestId",
                table: "ActionLogs");

            migrationBuilder.DropIndex(
                name: "IX_ProviderRemovalRequestServiceMapping_ServiceId",
                table: "ProviderRemovalRequestServiceMapping");

            migrationBuilder.DropIndex(
                name: "IX_ActionLogs_ProviderRemovalRequestId",
                table: "ActionLogs");

            migrationBuilder.DropColumn(
                name: "ProviderRemovalRequestId",
                table: "ActionLogs");

            migrationBuilder.CreateIndex(
                name: "IX_ProviderRemovalRequestServiceMapping_ServiceId",
                table: "ProviderRemovalRequestServiceMapping",
                column: "ServiceId");
        }
    }
}
