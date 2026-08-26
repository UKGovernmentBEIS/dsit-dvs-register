using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DVSRegister.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddServiceTypeToReport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsUnderpinningOrWhiteLabelledService",
                table: "PublishedRegisterEntryRevisions");

            migrationBuilder.AddColumn<string>(
                name: "ServiceType",
                table: "PublishedRegisterEntryRevisions",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ServiceType",
                table: "PublishedRegisterEntryRevisions");

            migrationBuilder.AddColumn<bool>(
                name: "IsUnderpinningOrWhiteLabelledService",
                table: "PublishedRegisterEntryRevisions",
                type: "boolean",
                nullable: true);
        }
    }
}
