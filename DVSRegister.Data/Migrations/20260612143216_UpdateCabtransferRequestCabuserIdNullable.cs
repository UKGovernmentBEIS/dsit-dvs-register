using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DVSRegister.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCabtransferRequestCabuserIdNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CabTransferRequest_CabUser_FromCabUserId",
                table: "CabTransferRequest");

            migrationBuilder.AlterColumn<int>(
                name: "FromCabUserId",
                table: "CabTransferRequest",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "FK_CabTransferRequest_CabUser_FromCabUserId",
                table: "CabTransferRequest",
                column: "FromCabUserId",
                principalTable: "CabUser",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CabTransferRequest_CabUser_FromCabUserId",
                table: "CabTransferRequest");

            migrationBuilder.AlterColumn<int>(
                name: "FromCabUserId",
                table: "CabTransferRequest",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_CabTransferRequest_CabUser_FromCabUserId",
                table: "CabTransferRequest",
                column: "FromCabUserId",
                principalTable: "CabUser",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
