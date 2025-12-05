using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DVSRegister.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePiCheckTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PICheckFailReason",
                table: "PublicInterestCheck",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SendBackReviewType",
                table: "PublicInterestCheck",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "SendBackTime",
                table: "PublicInterestCheck",
                type: "timestamp without time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PICheckFailReason",
                table: "PublicInterestCheck");

            migrationBuilder.DropColumn(
                name: "SendBackReviewType",
                table: "PublicInterestCheck");

            migrationBuilder.DropColumn(
                name: "SendBackTime",
                table: "PublicInterestCheck");
        }
    }
}
