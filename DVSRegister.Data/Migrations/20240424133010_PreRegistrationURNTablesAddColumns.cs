using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DVSRegister.Data.Migrations
{
    /// <inheritdoc />
    public partial class PreRegistrationURNTablesAddColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "UniqueReferenceNumber");

            migrationBuilder.AddColumn<int>(
                name: "PreRegistrationId",
                table: "UniqueReferenceNumber",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "URNStatus",
                table: "UniqueReferenceNumber",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsDirectorshipsApproved",
                table: "PreRegistrationReview",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2024, 4, 24, 13, 30, 9, 570, DateTimeKind.Utc).AddTicks(7288));

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2024, 4, 24, 13, 30, 9, 570, DateTimeKind.Utc).AddTicks(7295));

            migrationBuilder.CreateIndex(
                name: "IX_UniqueReferenceNumber_PreRegistrationId",
                table: "UniqueReferenceNumber",
                column: "PreRegistrationId");

            migrationBuilder.AddForeignKey(
                name: "FK_UniqueReferenceNumber_PreRegistration_PreRegistrationId",
                table: "UniqueReferenceNumber",
                column: "PreRegistrationId",
                principalTable: "PreRegistration",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UniqueReferenceNumber_PreRegistration_PreRegistrationId",
                table: "UniqueReferenceNumber");

            migrationBuilder.DropIndex(
                name: "IX_UniqueReferenceNumber_PreRegistrationId",
                table: "UniqueReferenceNumber");

            migrationBuilder.DropColumn(
                name: "PreRegistrationId",
                table: "UniqueReferenceNumber");

            migrationBuilder.DropColumn(
                name: "URNStatus",
                table: "UniqueReferenceNumber");

            migrationBuilder.DropColumn(
                name: "IsDirectorshipsApproved",
                table: "PreRegistrationReview");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "UniqueReferenceNumber",
                type: "text",
                nullable: true);

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
    }
}
