using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DVSRegister.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddIdentityProfileTypeColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IdentityProfileType",
                table: "IdentityProfile",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "IdentityProfile",
                keyColumn: "Id",
                keyValue: 1,
                column: "IdentityProfileType",
                value: 1);

            migrationBuilder.UpdateData(
                table: "IdentityProfile",
                keyColumn: "Id",
                keyValue: 2,
                column: "IdentityProfileType",
                value: 1);

            migrationBuilder.UpdateData(
                table: "IdentityProfile",
                keyColumn: "Id",
                keyValue: 3,
                column: "IdentityProfileType",
                value: 1);

            migrationBuilder.UpdateData(
                table: "IdentityProfile",
                keyColumn: "Id",
                keyValue: 4,
                column: "IdentityProfileType",
                value: 1);

            migrationBuilder.UpdateData(
                table: "IdentityProfile",
                keyColumn: "Id",
                keyValue: 5,
                column: "IdentityProfileType",
                value: 1);

            migrationBuilder.UpdateData(
                table: "IdentityProfile",
                keyColumn: "Id",
                keyValue: 6,
                column: "IdentityProfileType",
                value: 1);

            migrationBuilder.UpdateData(
                table: "IdentityProfile",
                keyColumn: "Id",
                keyValue: 7,
                column: "IdentityProfileType",
                value: 2);

            migrationBuilder.UpdateData(
                table: "IdentityProfile",
                keyColumn: "Id",
                keyValue: 8,
                column: "IdentityProfileType",
                value: 2);

            migrationBuilder.UpdateData(
                table: "IdentityProfile",
                keyColumn: "Id",
                keyValue: 9,
                column: "IdentityProfileType",
                value: 2);

            migrationBuilder.UpdateData(
                table: "IdentityProfile",
                keyColumn: "Id",
                keyValue: 10,
                column: "IdentityProfileType",
                value: 2);

            migrationBuilder.UpdateData(
                table: "IdentityProfile",
                keyColumn: "Id",
                keyValue: 11,
                column: "IdentityProfileType",
                value: 2);

            migrationBuilder.UpdateData(
                table: "IdentityProfile",
                keyColumn: "Id",
                keyValue: 12,
                column: "IdentityProfileType",
                value: 2);

            migrationBuilder.UpdateData(
                table: "IdentityProfile",
                keyColumn: "Id",
                keyValue: 13,
                column: "IdentityProfileType",
                value: 2);

            migrationBuilder.UpdateData(
                table: "IdentityProfile",
                keyColumn: "Id",
                keyValue: 14,
                column: "IdentityProfileType",
                value: 2);

            migrationBuilder.UpdateData(
                table: "IdentityProfile",
                keyColumn: "Id",
                keyValue: 15,
                column: "IdentityProfileType",
                value: 3);

            migrationBuilder.UpdateData(
                table: "IdentityProfile",
                keyColumn: "Id",
                keyValue: 16,
                column: "IdentityProfileType",
                value: 3);

            migrationBuilder.UpdateData(
                table: "IdentityProfile",
                keyColumn: "Id",
                keyValue: 17,
                column: "IdentityProfileType",
                value: 3);

            migrationBuilder.UpdateData(
                table: "IdentityProfile",
                keyColumn: "Id",
                keyValue: 18,
                column: "IdentityProfileType",
                value: 3);

            migrationBuilder.UpdateData(
                table: "IdentityProfile",
                keyColumn: "Id",
                keyValue: 19,
                column: "IdentityProfileType",
                value: 3);

            migrationBuilder.UpdateData(
                table: "IdentityProfile",
                keyColumn: "Id",
                keyValue: 20,
                column: "IdentityProfileType",
                value: 3);

            migrationBuilder.UpdateData(
                table: "IdentityProfile",
                keyColumn: "Id",
                keyValue: 21,
                column: "IdentityProfileType",
                value: 3);

            migrationBuilder.UpdateData(
                table: "IdentityProfile",
                keyColumn: "Id",
                keyValue: 22,
                column: "IdentityProfileType",
                value: 3);

            migrationBuilder.UpdateData(
                table: "IdentityProfile",
                keyColumn: "Id",
                keyValue: 23,
                column: "IdentityProfileType",
                value: 3);

            migrationBuilder.UpdateData(
                table: "IdentityProfile",
                keyColumn: "Id",
                keyValue: 24,
                column: "IdentityProfileType",
                value: 4);

            migrationBuilder.UpdateData(
                table: "IdentityProfile",
                keyColumn: "Id",
                keyValue: 25,
                column: "IdentityProfileType",
                value: 4);

            migrationBuilder.UpdateData(
                table: "IdentityProfile",
                keyColumn: "Id",
                keyValue: 26,
                column: "IdentityProfileType",
                value: 4);

            migrationBuilder.UpdateData(
                table: "IdentityProfile",
                keyColumn: "Id",
                keyValue: 27,
                column: "IdentityProfileType",
                value: 4);

            migrationBuilder.UpdateData(
                table: "IdentityProfile",
                keyColumn: "Id",
                keyValue: 28,
                column: "IdentityProfileType",
                value: 4);

            migrationBuilder.UpdateData(
                table: "IdentityProfile",
                keyColumn: "Id",
                keyValue: 29,
                column: "IdentityProfileType",
                value: 4);

            migrationBuilder.UpdateData(
                table: "IdentityProfile",
                keyColumn: "Id",
                keyValue: 30,
                column: "IdentityProfileType",
                value: 4);

            migrationBuilder.UpdateData(
                table: "IdentityProfile",
                keyColumn: "Id",
                keyValue: 31,
                column: "IdentityProfileType",
                value: 4);

            migrationBuilder.UpdateData(
                table: "IdentityProfile",
                keyColumn: "Id",
                keyValue: 32,
                column: "IdentityProfileType",
                value: 4);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IdentityProfileType",
                table: "IdentityProfile");
        }
    }
}
