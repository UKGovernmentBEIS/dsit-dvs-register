using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DVSRegister.Data.Migrations
{
    /// <inheritdoc />
    public partial class TrustFrameworkVersion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Version",
                table: "TrustFrameworkVersion",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.UpdateData(
                table: "TrustFrameworkVersion",
                keyColumn: "Id",
                keyValue: 1,
                column: "Version",
                value: 0.4m);

            migrationBuilder.UpdateData(
                table: "TrustFrameworkVersion",
                keyColumn: "Id",
                keyValue: 2,
                column: "Version",
                value: 0.3m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Version",
                table: "TrustFrameworkVersion");
        }
    }
}
