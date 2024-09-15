using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DVSRegister.Data.Migrations
{
    /// <inheritdoc />
    public partial class DropCertificateInformationTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CertificateReview_CertificateInformation_CertificateInforma~",
                table: "CertificateReview");

            migrationBuilder.DropTable(
                name: "CertificateInformation");

            migrationBuilder.DropIndex(
                name: "IX_CertificateReview_CertificateInformationId",
                table: "CertificateReview");

            migrationBuilder.DropColumn(
                name: "CertificateInformationId",
                table: "CertificateReview");

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedTime",
                value: new DateTime(2024, 8, 27, 21, 22, 8, 772, DateTimeKind.Utc).AddTicks(6998));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedTime",
                value: new DateTime(2024, 8, 27, 21, 22, 8, 772, DateTimeKind.Utc).AddTicks(7001));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedTime",
                value: new DateTime(2024, 8, 27, 21, 22, 8, 772, DateTimeKind.Utc).AddTicks(7003));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedTime",
                value: new DateTime(2024, 8, 27, 21, 22, 8, 772, DateTimeKind.Utc).AddTicks(7004));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CertificateInformationId",
                table: "CertificateReview",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "CertificateInformation",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProviderId = table.Column<int>(type: "integer", nullable: false),
                    CertificateInfoStatus = table.Column<int>(type: "integer", nullable: false),
                    ConformityExpiryDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ConformityIssueDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    FileLink = table.Column<string>(type: "text", nullable: false),
                    FileName = table.Column<string>(type: "text", nullable: false),
                    HasSupplementarySchemes = table.Column<bool>(type: "boolean", nullable: false),
                    ModifiedBy = table.Column<string>(type: "text", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ServiceName = table.Column<string>(type: "text", nullable: false),
                    SubmittedCAB = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CertificateInformation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CertificateInformation_Provider_ProviderId",
                        column: x => x.ProviderId,
                        principalTable: "Provider",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedTime",
                value: new DateTime(2024, 8, 27, 21, 16, 6, 816, DateTimeKind.Utc).AddTicks(2742));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedTime",
                value: new DateTime(2024, 8, 27, 21, 16, 6, 816, DateTimeKind.Utc).AddTicks(2745));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedTime",
                value: new DateTime(2024, 8, 27, 21, 16, 6, 816, DateTimeKind.Utc).AddTicks(2746));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedTime",
                value: new DateTime(2024, 8, 27, 21, 16, 6, 816, DateTimeKind.Utc).AddTicks(2747));

            migrationBuilder.CreateIndex(
                name: "IX_CertificateReview_CertificateInformationId",
                table: "CertificateReview",
                column: "CertificateInformationId");

            migrationBuilder.CreateIndex(
                name: "IX_CertificateInformation_ProviderId",
                table: "CertificateInformation",
                column: "ProviderId");

            migrationBuilder.AddForeignKey(
                name: "FK_CertificateReview_CertificateInformation_CertificateInforma~",
                table: "CertificateReview",
                column: "CertificateInformationId",
                principalTable: "CertificateInformation",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
