using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DVSRegister.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddVouchingGuidanceTmIssuanceAndSupSchemeTFVersions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "HasVouchingGuidance",
                table: "ServiceDraft",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "HasVouchingGuidance",
                table: "Service",
                type: "boolean",
                nullable: true);

            migrationBuilder.InsertData(
                table: "ActionDetails",
                columns: new[] { "Id", "ActionCategoryId", "ActionDescription", "ActionDetailsKey" },
                values: new object[,]
                {
                    { 33, 2, "Service published", "PI_ServicePublish_With_TM" },
                    { 34, 2, "Service updated", "PI_ServiceRePublish_With_TM" }
                });

            migrationBuilder.UpdateData(
                table: "IdentityProfile",
                keyColumn: "Id",
                keyValue: 1,
                column: "TrustFrameworkVersionId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "IdentityProfile",
                keyColumn: "Id",
                keyValue: 2,
                column: "TrustFrameworkVersionId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "IdentityProfile",
                keyColumn: "Id",
                keyValue: 3,
                column: "TrustFrameworkVersionId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "IdentityProfile",
                keyColumn: "Id",
                keyValue: 4,
                column: "TrustFrameworkVersionId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "IdentityProfile",
                keyColumn: "Id",
                keyValue: 5,
                column: "TrustFrameworkVersionId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "IdentityProfile",
                keyColumn: "Id",
                keyValue: 6,
                column: "TrustFrameworkVersionId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "IdentityProfile",
                keyColumn: "Id",
                keyValue: 7,
                column: "TrustFrameworkVersionId",
                value: 3);

            migrationBuilder.UpdateData(
                table: "IdentityProfile",
                keyColumn: "Id",
                keyValue: 8,
                column: "TrustFrameworkVersionId",
                value: 3);

            migrationBuilder.UpdateData(
                table: "IdentityProfile",
                keyColumn: "Id",
                keyValue: 9,
                column: "TrustFrameworkVersionId",
                value: 3);

            migrationBuilder.UpdateData(
                table: "IdentityProfile",
                keyColumn: "Id",
                keyValue: 10,
                column: "TrustFrameworkVersionId",
                value: 3);

            migrationBuilder.UpdateData(
                table: "IdentityProfile",
                keyColumn: "Id",
                keyValue: 11,
                column: "TrustFrameworkVersionId",
                value: 3);

            migrationBuilder.UpdateData(
                table: "IdentityProfile",
                keyColumn: "Id",
                keyValue: 12,
                column: "TrustFrameworkVersionId",
                value: 3);

            migrationBuilder.UpdateData(
                table: "IdentityProfile",
                keyColumn: "Id",
                keyValue: 13,
                column: "TrustFrameworkVersionId",
                value: 3);

            migrationBuilder.UpdateData(
                table: "IdentityProfile",
                keyColumn: "Id",
                keyValue: 14,
                column: "TrustFrameworkVersionId",
                value: 3);

            migrationBuilder.UpdateData(
                table: "IdentityProfile",
                keyColumn: "Id",
                keyValue: 15,
                column: "TrustFrameworkVersionId",
                value: 3);

            migrationBuilder.UpdateData(
                table: "IdentityProfile",
                keyColumn: "Id",
                keyValue: 16,
                column: "TrustFrameworkVersionId",
                value: 3);

            migrationBuilder.UpdateData(
                table: "IdentityProfile",
                keyColumn: "Id",
                keyValue: 17,
                column: "TrustFrameworkVersionId",
                value: 3);

            migrationBuilder.UpdateData(
                table: "IdentityProfile",
                keyColumn: "Id",
                keyValue: 18,
                column: "TrustFrameworkVersionId",
                value: 3);

            migrationBuilder.UpdateData(
                table: "IdentityProfile",
                keyColumn: "Id",
                keyValue: 19,
                column: "TrustFrameworkVersionId",
                value: 3);

            migrationBuilder.UpdateData(
                table: "IdentityProfile",
                keyColumn: "Id",
                keyValue: 20,
                column: "TrustFrameworkVersionId",
                value: 3);

            migrationBuilder.UpdateData(
                table: "IdentityProfile",
                keyColumn: "Id",
                keyValue: 21,
                column: "TrustFrameworkVersionId",
                value: 3);

            migrationBuilder.UpdateData(
                table: "IdentityProfile",
                keyColumn: "Id",
                keyValue: 22,
                column: "TrustFrameworkVersionId",
                value: 3);

            migrationBuilder.UpdateData(
                table: "IdentityProfile",
                keyColumn: "Id",
                keyValue: 23,
                column: "TrustFrameworkVersionId",
                value: 3);

            migrationBuilder.UpdateData(
                table: "IdentityProfile",
                keyColumn: "Id",
                keyValue: 24,
                column: "TrustFrameworkVersionId",
                value: 3);

            migrationBuilder.UpdateData(
                table: "IdentityProfile",
                keyColumn: "Id",
                keyValue: 25,
                column: "TrustFrameworkVersionId",
                value: 3);

            migrationBuilder.UpdateData(
                table: "IdentityProfile",
                keyColumn: "Id",
                keyValue: 26,
                column: "TrustFrameworkVersionId",
                value: 3);

            migrationBuilder.UpdateData(
                table: "IdentityProfile",
                keyColumn: "Id",
                keyValue: 27,
                column: "TrustFrameworkVersionId",
                value: 3);

            migrationBuilder.UpdateData(
                table: "IdentityProfile",
                keyColumn: "Id",
                keyValue: 28,
                column: "TrustFrameworkVersionId",
                value: 3);

            migrationBuilder.UpdateData(
                table: "IdentityProfile",
                keyColumn: "Id",
                keyValue: 29,
                column: "TrustFrameworkVersionId",
                value: 3);

            migrationBuilder.UpdateData(
                table: "IdentityProfile",
                keyColumn: "Id",
                keyValue: 30,
                column: "TrustFrameworkVersionId",
                value: 3);

            migrationBuilder.UpdateData(
                table: "IdentityProfile",
                keyColumn: "Id",
                keyValue: 31,
                column: "TrustFrameworkVersionId",
                value: 3);

            migrationBuilder.UpdateData(
                table: "IdentityProfile",
                keyColumn: "Id",
                keyValue: 32,
                column: "TrustFrameworkVersionId",
                value: 3);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ActionDetails",
                keyColumn: "Id",
                keyValue: 33);

            migrationBuilder.DeleteData(
                table: "ActionDetails",
                keyColumn: "Id",
                keyValue: 34);

            migrationBuilder.DropColumn(
                name: "HasVouchingGuidance",
                table: "ServiceDraft");

            migrationBuilder.DropColumn(
                name: "HasVouchingGuidance",
                table: "Service");

            migrationBuilder.UpdateData(
                table: "IdentityProfile",
                keyColumn: "Id",
                keyValue: 1,
                column: "TrustFrameworkVersionId",
                value: 0);

            migrationBuilder.UpdateData(
                table: "IdentityProfile",
                keyColumn: "Id",
                keyValue: 2,
                column: "TrustFrameworkVersionId",
                value: 0);

            migrationBuilder.UpdateData(
                table: "IdentityProfile",
                keyColumn: "Id",
                keyValue: 3,
                column: "TrustFrameworkVersionId",
                value: 0);

            migrationBuilder.UpdateData(
                table: "IdentityProfile",
                keyColumn: "Id",
                keyValue: 4,
                column: "TrustFrameworkVersionId",
                value: 0);

            migrationBuilder.UpdateData(
                table: "IdentityProfile",
                keyColumn: "Id",
                keyValue: 5,
                column: "TrustFrameworkVersionId",
                value: 0);

            migrationBuilder.UpdateData(
                table: "IdentityProfile",
                keyColumn: "Id",
                keyValue: 6,
                column: "TrustFrameworkVersionId",
                value: 0);

            migrationBuilder.UpdateData(
                table: "IdentityProfile",
                keyColumn: "Id",
                keyValue: 7,
                column: "TrustFrameworkVersionId",
                value: 0);

            migrationBuilder.UpdateData(
                table: "IdentityProfile",
                keyColumn: "Id",
                keyValue: 8,
                column: "TrustFrameworkVersionId",
                value: 0);

            migrationBuilder.UpdateData(
                table: "IdentityProfile",
                keyColumn: "Id",
                keyValue: 9,
                column: "TrustFrameworkVersionId",
                value: 0);

            migrationBuilder.UpdateData(
                table: "IdentityProfile",
                keyColumn: "Id",
                keyValue: 10,
                column: "TrustFrameworkVersionId",
                value: 0);

            migrationBuilder.UpdateData(
                table: "IdentityProfile",
                keyColumn: "Id",
                keyValue: 11,
                column: "TrustFrameworkVersionId",
                value: 0);

            migrationBuilder.UpdateData(
                table: "IdentityProfile",
                keyColumn: "Id",
                keyValue: 12,
                column: "TrustFrameworkVersionId",
                value: 0);

            migrationBuilder.UpdateData(
                table: "IdentityProfile",
                keyColumn: "Id",
                keyValue: 13,
                column: "TrustFrameworkVersionId",
                value: 0);

            migrationBuilder.UpdateData(
                table: "IdentityProfile",
                keyColumn: "Id",
                keyValue: 14,
                column: "TrustFrameworkVersionId",
                value: 0);

            migrationBuilder.UpdateData(
                table: "IdentityProfile",
                keyColumn: "Id",
                keyValue: 15,
                column: "TrustFrameworkVersionId",
                value: 0);

            migrationBuilder.UpdateData(
                table: "IdentityProfile",
                keyColumn: "Id",
                keyValue: 16,
                column: "TrustFrameworkVersionId",
                value: 0);

            migrationBuilder.UpdateData(
                table: "IdentityProfile",
                keyColumn: "Id",
                keyValue: 17,
                column: "TrustFrameworkVersionId",
                value: 0);

            migrationBuilder.UpdateData(
                table: "IdentityProfile",
                keyColumn: "Id",
                keyValue: 18,
                column: "TrustFrameworkVersionId",
                value: 0);

            migrationBuilder.UpdateData(
                table: "IdentityProfile",
                keyColumn: "Id",
                keyValue: 19,
                column: "TrustFrameworkVersionId",
                value: 0);

            migrationBuilder.UpdateData(
                table: "IdentityProfile",
                keyColumn: "Id",
                keyValue: 20,
                column: "TrustFrameworkVersionId",
                value: 0);

            migrationBuilder.UpdateData(
                table: "IdentityProfile",
                keyColumn: "Id",
                keyValue: 21,
                column: "TrustFrameworkVersionId",
                value: 0);

            migrationBuilder.UpdateData(
                table: "IdentityProfile",
                keyColumn: "Id",
                keyValue: 22,
                column: "TrustFrameworkVersionId",
                value: 0);

            migrationBuilder.UpdateData(
                table: "IdentityProfile",
                keyColumn: "Id",
                keyValue: 23,
                column: "TrustFrameworkVersionId",
                value: 0);

            migrationBuilder.UpdateData(
                table: "IdentityProfile",
                keyColumn: "Id",
                keyValue: 24,
                column: "TrustFrameworkVersionId",
                value: 0);

            migrationBuilder.UpdateData(
                table: "IdentityProfile",
                keyColumn: "Id",
                keyValue: 25,
                column: "TrustFrameworkVersionId",
                value: 0);

            migrationBuilder.UpdateData(
                table: "IdentityProfile",
                keyColumn: "Id",
                keyValue: 26,
                column: "TrustFrameworkVersionId",
                value: 0);

            migrationBuilder.UpdateData(
                table: "IdentityProfile",
                keyColumn: "Id",
                keyValue: 27,
                column: "TrustFrameworkVersionId",
                value: 0);

            migrationBuilder.UpdateData(
                table: "IdentityProfile",
                keyColumn: "Id",
                keyValue: 28,
                column: "TrustFrameworkVersionId",
                value: 0);

            migrationBuilder.UpdateData(
                table: "IdentityProfile",
                keyColumn: "Id",
                keyValue: 29,
                column: "TrustFrameworkVersionId",
                value: 0);

            migrationBuilder.UpdateData(
                table: "IdentityProfile",
                keyColumn: "Id",
                keyValue: 30,
                column: "TrustFrameworkVersionId",
                value: 0);

            migrationBuilder.UpdateData(
                table: "IdentityProfile",
                keyColumn: "Id",
                keyValue: 31,
                column: "TrustFrameworkVersionId",
                value: 0);

            migrationBuilder.UpdateData(
                table: "IdentityProfile",
                keyColumn: "Id",
                keyValue: 32,
                column: "TrustFrameworkVersionId",
                value: 0);
        }
    }
}
