using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DVSRegister.Data.Migrations
{
    /// <inheritdoc />
    public partial class ModifyRemovalRequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EditServiceTokenStatus",
                table: "Service");

            migrationBuilder.DropColumn(
                name: "RemovalTokenStatus",
                table: "Service");

            migrationBuilder.AddColumn<int>(
                name: "RemovedByUserId",
                table: "ServiceRemovalRequest",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RemovedByUserId",
                table: "ProviderRemovalRequest",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ServiceRemovalRequest_RemovedByUserId",
                table: "ServiceRemovalRequest",
                column: "RemovedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ProviderRemovalRequest_RemovedByUserId",
                table: "ProviderRemovalRequest",
                column: "RemovedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProviderRemovalRequest_User_RemovedByUserId",
                table: "ProviderRemovalRequest",
                column: "RemovedByUserId",
                principalTable: "User",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceRemovalRequest_User_RemovedByUserId",
                table: "ServiceRemovalRequest",
                column: "RemovedByUserId",
                principalTable: "User",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProviderRemovalRequest_User_RemovedByUserId",
                table: "ProviderRemovalRequest");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceRemovalRequest_User_RemovedByUserId",
                table: "ServiceRemovalRequest");

            migrationBuilder.DropIndex(
                name: "IX_ServiceRemovalRequest_RemovedByUserId",
                table: "ServiceRemovalRequest");

            migrationBuilder.DropIndex(
                name: "IX_ProviderRemovalRequest_RemovedByUserId",
                table: "ProviderRemovalRequest");

            migrationBuilder.DropColumn(
                name: "RemovedByUserId",
                table: "ServiceRemovalRequest");

            migrationBuilder.DropColumn(
                name: "RemovedByUserId",
                table: "ProviderRemovalRequest");

            migrationBuilder.AddColumn<int>(
                name: "EditServiceTokenStatus",
                table: "Service",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RemovalTokenStatus",
                table: "Service",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
