using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DVSRegister.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddTmVersion10Columns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TOUFileLink",
                table: "Service",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TOUFileName",
                table: "Service",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TOUFileSizeInKb",
                table: "Service",
                type: "numeric(10,1)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "TrustFrameworkVersion",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Order", "Version" },
                values: new object[] { 2, 0.4m });

            migrationBuilder.UpdateData(
                table: "TrustFrameworkVersion",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Order", "Version" },
                values: new object[] { 1, 0.3m });

            migrationBuilder.InsertData(
                table: "TrustFrameworkVersion",
                columns: new[] { "Id", "Order", "TrustFrameworkName", "Version" },
                values: new object[] { 3, 3, "1.0", 1.0m });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "TrustFrameworkVersion",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DropColumn(
                name: "TOUFileLink",
                table: "Service");

            migrationBuilder.DropColumn(
                name: "TOUFileName",
                table: "Service");

            migrationBuilder.DropColumn(
                name: "TOUFileSizeInKb",
                table: "Service");

            migrationBuilder.UpdateData(
                table: "TrustFrameworkVersion",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Order", "Version" },
                values: new object[] { 1, 0m });

            migrationBuilder.UpdateData(
                table: "TrustFrameworkVersion",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Order", "Version" },
                values: new object[] { 2, 0m });
        }
    }
}
