using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DVSRegister.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateProviderDraft : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProviderProfileDraft_User_RequestedUserId",
                table: "ProviderProfileDraft");

            migrationBuilder.AlterColumn<int>(
                name: "RequestedUserId",
                table: "ProviderProfileDraft",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<bool>(
                name: "IsAdminRequested",
                table: "ProviderProfileDraft",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "LinkToContactPage",
                table: "ProviderProfileDraft",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RequestedCabUserId",
                table: "ProviderProfileDraft",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProviderProfileDraft_RequestedCabUserId",
                table: "ProviderProfileDraft",
                column: "RequestedCabUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProviderProfileDraft_CabUser_RequestedCabUserId",
                table: "ProviderProfileDraft",
                column: "RequestedCabUserId",
                principalTable: "CabUser",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProviderProfileDraft_User_RequestedUserId",
                table: "ProviderProfileDraft",
                column: "RequestedUserId",
                principalTable: "User",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProviderProfileDraft_CabUser_RequestedCabUserId",
                table: "ProviderProfileDraft");

            migrationBuilder.DropForeignKey(
                name: "FK_ProviderProfileDraft_User_RequestedUserId",
                table: "ProviderProfileDraft");

            migrationBuilder.DropIndex(
                name: "IX_ProviderProfileDraft_RequestedCabUserId",
                table: "ProviderProfileDraft");

            migrationBuilder.DropColumn(
                name: "IsAdminRequested",
                table: "ProviderProfileDraft");

            migrationBuilder.DropColumn(
                name: "LinkToContactPage",
                table: "ProviderProfileDraft");

            migrationBuilder.DropColumn(
                name: "RequestedCabUserId",
                table: "ProviderProfileDraft");

            migrationBuilder.AlterColumn<int>(
                name: "RequestedUserId",
                table: "ProviderProfileDraft",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ProviderProfileDraft_User_RequestedUserId",
                table: "ProviderProfileDraft",
                column: "RequestedUserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
