using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DVSRegister.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCabTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RegisteredName",
                table: "Cab",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TradingName",
                table: "Cab",
                type: "text",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "RegisteredName", "TradingName" },
                values: new object[] { "EY registered name", "EY" });

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "RegisteredName", "TradingName" },
                values: new object[] { "DSIT registered name", "DSIT" });

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "RegisteredName", "TradingName" },
                values: new object[] { "", "" });

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "RegisteredName", "TradingName" },
                values: new object[] { "Kantara Initiative", "Kantara" });

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "RegisteredName", "TradingName" },
                values: new object[] { "", "" });

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "RegisteredName", "TradingName" },
                values: new object[] { "BSI Assurance UK Limited", "BSI" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RegisteredName",
                table: "Cab");

            migrationBuilder.DropColumn(
                name: "TradingName",
                table: "Cab");
        }
    }
}
