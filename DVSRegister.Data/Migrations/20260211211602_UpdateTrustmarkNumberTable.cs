using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DVSRegister.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTrustmarkNumberTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TrustmarkNumber_ServiceId",
                table: "TrustmarkNumber");

            migrationBuilder.AlterColumn<string>(
                name: "TrustMarkNumber",
                table: "TrustmarkNumber",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldComputedColumnSql: "LPAD(\"CompanyId\"::VARCHAR(4), 4, '0') || LPAD(\"ServiceNumber\"::VARCHAR(2), 2, '0')");

            migrationBuilder.AddColumn<string>(
                name: "JpegLogoLink",
                table: "TrustmarkNumber",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "LogoVerified",
                table: "TrustmarkNumber",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "PngLogoLink",
                table: "TrustmarkNumber",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SvgLogoLink",
                table: "TrustmarkNumber",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "TrustMarkNumberVerified",
                table: "TrustmarkNumber",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_TrustmarkNumber_ServiceId",
                table: "TrustmarkNumber",
                column: "ServiceId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TrustmarkNumber_ServiceId",
                table: "TrustmarkNumber");

            migrationBuilder.DropColumn(
                name: "JpegLogoLink",
                table: "TrustmarkNumber");

            migrationBuilder.DropColumn(
                name: "LogoVerified",
                table: "TrustmarkNumber");

            migrationBuilder.DropColumn(
                name: "PngLogoLink",
                table: "TrustmarkNumber");

            migrationBuilder.DropColumn(
                name: "SvgLogoLink",
                table: "TrustmarkNumber");

            migrationBuilder.DropColumn(
                name: "TrustMarkNumberVerified",
                table: "TrustmarkNumber");

            migrationBuilder.AlterColumn<string>(
                name: "TrustMarkNumber",
                table: "TrustmarkNumber",
                type: "text",
                nullable: false,
                computedColumnSql: "LPAD(\"CompanyId\"::VARCHAR(4), 4, '0') || LPAD(\"ServiceNumber\"::VARCHAR(2), 2, '0')",
                stored: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.CreateIndex(
                name: "IX_TrustmarkNumber_ServiceId",
                table: "TrustmarkNumber",
                column: "ServiceId");
        }
    }
}
