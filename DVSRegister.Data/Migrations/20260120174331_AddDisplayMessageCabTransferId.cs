using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DVSRegister.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddDisplayMessageCabTransferId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ActionDetails",
                keyColumn: "Id",
                keyValue: 32);

            migrationBuilder.AddColumn<int>(
                name: "CabTransferRequestId",
                table: "ActionLogs",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DisplayMessageAdmin",
                table: "ActionLogs",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ActionLogs_CabTransferRequestId",
                table: "ActionLogs",
                column: "CabTransferRequestId");

            migrationBuilder.AddForeignKey(
                name: "FK_ActionLogs_CabTransferRequest_CabTransferRequestId",
                table: "ActionLogs",
                column: "CabTransferRequestId",
                principalTable: "CabTransferRequest",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ActionLogs_CabTransferRequest_CabTransferRequestId",
                table: "ActionLogs");

            migrationBuilder.DropIndex(
                name: "IX_ActionLogs_CabTransferRequestId",
                table: "ActionLogs");

            migrationBuilder.DropColumn(
                name: "CabTransferRequestId",
                table: "ActionLogs");

            migrationBuilder.DropColumn(
                name: "DisplayMessageAdmin",
                table: "ActionLogs");

            migrationBuilder.InsertData(
                table: "ActionDetails",
                columns: new[] { "Id", "ActionCategoryId", "ActionDescription", "ActionDetailsKey" },
                values: new object[] { 32, 2, "Published", "PI_Pass" });
        }
    }
}
