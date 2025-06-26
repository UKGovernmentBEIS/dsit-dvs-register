using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DVSRegister.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUnderPinningServiceTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CABName",
                table: "ManualUnderPinningService");

            migrationBuilder.AddColumn<int>(
                name: "CabId",
                table: "ManualUnderPinningService",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ManualUnderPinningService_CabId",
                table: "ManualUnderPinningService",
                column: "CabId");

            migrationBuilder.AddForeignKey(
                name: "FK_ManualUnderPinningService_Cab_CabId",
                table: "ManualUnderPinningService",
                column: "CabId",
                principalTable: "Cab",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ManualUnderPinningService_Cab_CabId",
                table: "ManualUnderPinningService");

            migrationBuilder.DropIndex(
                name: "IX_ManualUnderPinningService_CabId",
                table: "ManualUnderPinningService");

            migrationBuilder.DropColumn(
                name: "CabId",
                table: "ManualUnderPinningService");

            migrationBuilder.AddColumn<string>(
                name: "CABName",
                table: "ManualUnderPinningService",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
