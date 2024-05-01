using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DVSRegister.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveTestUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "Id",
                keyValue: 2);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "User",
                columns: new[] { "Id", "CreatedBy", "CreatedDate", "Email", "ModifiedBy", "ModifiedDate", "UserName" },
                values: new object[,]
                {
                    { 1, "Dev", new DateTime(2024, 4, 26, 12, 37, 54, 593, DateTimeKind.Utc).AddTicks(4533), "Aiswarya.Rajendran@dsit.gov.uk", null, null, "Aiswarya" },
                    { 2, "Dev", new DateTime(2024, 4, 26, 12, 37, 54, 593, DateTimeKind.Utc).AddTicks(4606), "vishal.vishwanathan@ics.gov.uk", null, null, "Vishal" }
                });
        }
    }
}
