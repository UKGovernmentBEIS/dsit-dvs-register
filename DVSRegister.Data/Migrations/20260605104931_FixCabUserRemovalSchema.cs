using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DVSRegister.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixCabUserRemovalSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CabUserRemoval_CabUser_CabUserUserId",
                table: "CabUserRemoval");

            migrationBuilder.RenameColumn(
                name: "CabUserUserId",
                table: "CabUserRemoval",
                newName: "CabUserId");

            migrationBuilder.RenameIndex(
                name: "IX_CabUserRemoval_CabUserUserId",
                table: "CabUserRemoval",
                newName: "IX_CabUserRemoval_CabUserId");

            migrationBuilder.RenameColumn(
                name: "RemovalTime",
                table: "CabUserRemoval",
                newName: "RequestTime");

            migrationBuilder.DropColumn(
                name: "IsRemoved",
                table: "CabUserRemoval");

            migrationBuilder.AddColumn<int>(
                name: "RemovalStatus",
                table: "CabUserRemoval",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_CabUserRemoval_CabUser_CabUserId",
                table: "CabUserRemoval",
                column: "CabUserId",
                principalTable: "CabUser",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CabUserRemoval_CabUser_CabUserId",
                table: "CabUserRemoval");

            migrationBuilder.DropColumn(
                name: "RemovalStatus",
                table: "CabUserRemoval");

            migrationBuilder.RenameColumn(
                name: "CabUserId",
                table: "CabUserRemoval",
                newName: "CabUserUserId");

            migrationBuilder.RenameIndex(
                name: "IX_CabUserRemoval_CabUserId",
                table: "CabUserRemoval",
                newName: "IX_CabUserRemoval_CabUserUserId");

            migrationBuilder.RenameColumn(
                name: "RequestTime",
                table: "CabUserRemoval",
                newName: "RemovalTime");

            migrationBuilder.AddColumn<bool>(
                name: "IsRemoved",
                table: "CabUserRemoval",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddForeignKey(
                name: "FK_CabUserRemoval_CabUser_CabUserUserId",
                table: "CabUserRemoval",
                column: "CabUserUserId",
                principalTable: "CabUser",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}