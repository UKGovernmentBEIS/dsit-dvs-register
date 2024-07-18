using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DVSRegister.Data.Migrations
{
    /// <inheritdoc />
    public partial class RenameStatusColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ReviewProgress",
                table: "PreRegistrationReview",
                newName: "ApplicationReviewStatus");

            migrationBuilder.RenameColumn(
                name: "PreRegistrationStatus",
                table: "PreRegistration",
                newName: "ApplicationReviewStatus");

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2024, 4, 22, 11, 10, 43, 445, DateTimeKind.Utc).AddTicks(5384));

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2024, 4, 22, 11, 10, 43, 445, DateTimeKind.Utc).AddTicks(5437));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ApplicationReviewStatus",
                table: "PreRegistrationReview",
                newName: "ReviewProgress");

            migrationBuilder.RenameColumn(
                name: "ApplicationReviewStatus",
                table: "PreRegistration",
                newName: "PreRegistrationStatus");

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2024, 4, 19, 9, 32, 34, 599, DateTimeKind.Utc).AddTicks(6979));

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2024, 4, 19, 9, 32, 34, 599, DateTimeKind.Utc).AddTicks(6988));
        }
    }
}
