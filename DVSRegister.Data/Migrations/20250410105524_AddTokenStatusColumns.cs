using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DVSRegister.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddTokenStatusColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedTime",
                table: "ServiceDraftToken",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ClosingLoopTokenStatus",
                table: "Service",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "EditServiceTokenStatus",
                table: "Service",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "OpeningLoopTokenStatus",
                table: "Service",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedTime",
                table: "RemoveProviderToken",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EditProviderTokenStatus",
                table: "ProviderProfile",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedTime",
                table: "ProviderDraftToken",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedTime",
                table: "ProceedPublishConsentToken",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedTime",
                table: "ProceedApplicationConsentToken",
                type: "timestamp without time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ModifiedTime",
                table: "ServiceDraftToken");

            migrationBuilder.DropColumn(
                name: "ClosingLoopTokenStatus",
                table: "Service");

            migrationBuilder.DropColumn(
                name: "EditServiceTokenStatus",
                table: "Service");

            migrationBuilder.DropColumn(
                name: "OpeningLoopTokenStatus",
                table: "Service");

            migrationBuilder.DropColumn(
                name: "ModifiedTime",
                table: "RemoveProviderToken");

            migrationBuilder.DropColumn(
                name: "EditProviderTokenStatus",
                table: "ProviderProfile");

            migrationBuilder.DropColumn(
                name: "ModifiedTime",
                table: "ProviderDraftToken");

            migrationBuilder.DropColumn(
                name: "ModifiedTime",
                table: "ProceedPublishConsentToken");

            migrationBuilder.DropColumn(
                name: "ModifiedTime",
                table: "ProceedApplicationConsentToken");
        }
    }
}
