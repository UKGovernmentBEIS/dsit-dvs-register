using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DVSRegister.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSchemeOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "SupplementaryScheme",
                keyColumn: "Id",
                keyValue: 1,
                column: "Order",
                value: 1);

            migrationBuilder.UpdateData(
                table: "SupplementaryScheme",
                keyColumn: "Id",
                keyValue: 2,
                column: "Order",
                value: 2);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "SupplementaryScheme",
                keyColumn: "Id",
                keyValue: 1,
                column: "Order",
                value: 2);

            migrationBuilder.UpdateData(
                table: "SupplementaryScheme",
                keyColumn: "Id",
                keyValue: 2,
                column: "Order",
                value: 1);
        }
    }
}
