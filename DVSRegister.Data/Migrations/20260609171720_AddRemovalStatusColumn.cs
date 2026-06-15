using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DVSRegister.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddRemovalStatusColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsRemoved",
                table: "OfDIAUserRemoval");

            migrationBuilder.AddColumn<int>(
                name: "RemovalStatus",
                table: "OfDIAUserRemoval",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RemovalStatus",
                table: "OfDIAUserRemoval");

            migrationBuilder.AddColumn<bool>(
                name: "IsRemoved",
                table: "OfDIAUserRemoval",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
