using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DVSRegister.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCabTranferMappingCabUserDefaults : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CabTransferRequest_ProviderProfile_ProviderProfileId",
                table: "CabTransferRequest");

            migrationBuilder.DropIndex(
                name: "IX_CabTransferRequest_ProviderProfileId",
                table: "CabTransferRequest");

            migrationBuilder.DropColumn(
                name: "ProviderProfileId",
                table: "CabTransferRequest");

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                table: "CabUser",
                type: "boolean",
                nullable: false,
                defaultValue: true,
                oldClrType: typeof(bool),
                oldType: "boolean");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                table: "CabUser",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: true);

            migrationBuilder.AddColumn<int>(
                name: "ProviderProfileId",
                table: "CabTransferRequest",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_CabTransferRequest_ProviderProfileId",
                table: "CabTransferRequest",
                column: "ProviderProfileId");

            migrationBuilder.AddForeignKey(
                name: "FK_CabTransferRequest_ProviderProfile_ProviderProfileId",
                table: "CabTransferRequest",
                column: "ProviderProfileId",
                principalTable: "ProviderProfile",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
