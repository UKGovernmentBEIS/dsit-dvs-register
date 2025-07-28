using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DVSRegister.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateManualUnderPinningServiceTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ManualUnderPinningService_Cab_CabId",
                table: "ManualUnderPinningService");

            migrationBuilder.AlterColumn<string>(
                name: "ServiceName",
                table: "ManualUnderPinningService",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "ProviderName",
                table: "ManualUnderPinningService",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<int>(
                name: "CabId",
                table: "ManualUnderPinningService",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "FK_ManualUnderPinningService_Cab_CabId",
                table: "ManualUnderPinningService",
                column: "CabId",
                principalTable: "Cab",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ManualUnderPinningService_Cab_CabId",
                table: "ManualUnderPinningService");

            migrationBuilder.AlterColumn<string>(
                name: "ServiceName",
                table: "ManualUnderPinningService",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ProviderName",
                table: "ManualUnderPinningService",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CabId",
                table: "ManualUnderPinningService",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ManualUnderPinningService_Cab_CabId",
                table: "ManualUnderPinningService",
                column: "CabId",
                principalTable: "Cab",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
