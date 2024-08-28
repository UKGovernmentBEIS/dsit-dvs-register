using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DVSRegister.Data.Migrations
{
    /// <inheritdoc />
    public partial class DropCertificateReviewTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ConsentToken_CertificateReview_CertificateReviewId",
                table: "ConsentToken");

            migrationBuilder.DropTable(
                name: "CertificateReview");

            migrationBuilder.DropIndex(
                name: "IX_ConsentToken_CertificateReviewId",
                table: "ConsentToken");

            migrationBuilder.DropColumn(
                name: "CertificateReviewId",
                table: "ConsentToken");

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedTime",
                value: new DateTime(2024, 8, 27, 21, 25, 50, 971, DateTimeKind.Utc).AddTicks(4304));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedTime",
                value: new DateTime(2024, 8, 27, 21, 25, 50, 971, DateTimeKind.Utc).AddTicks(4309));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedTime",
                value: new DateTime(2024, 8, 27, 21, 25, 50, 971, DateTimeKind.Utc).AddTicks(4310));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedTime",
                value: new DateTime(2024, 8, 27, 21, 25, 50, 971, DateTimeKind.Utc).AddTicks(4311));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CertificateReviewId",
                table: "ConsentToken",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "CertificateReview",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PreRegistrationId = table.Column<int>(type: "integer", nullable: false),
                    ProviderId = table.Column<int>(type: "integer", nullable: false),
                    CertificateInfoStatus = table.Column<int>(type: "integer", nullable: false),
                    Comments = table.Column<string>(type: "text", nullable: true),
                    CommentsForIncorrect = table.Column<string>(type: "text", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    InformationMatched = table.Column<bool>(type: "boolean", nullable: true),
                    IsAuthenticyVerifiedCorrect = table.Column<bool>(type: "boolean", nullable: false),
                    IsCabDetailsCorrect = table.Column<bool>(type: "boolean", nullable: false),
                    IsCabLogoCorrect = table.Column<bool>(type: "boolean", nullable: false),
                    IsCertificationScopeCorrect = table.Column<bool>(type: "boolean", nullable: false),
                    IsDateOfExpiryCorrect = table.Column<bool>(type: "boolean", nullable: false),
                    IsDateOfIssueCorrect = table.Column<bool>(type: "boolean", nullable: false),
                    IsIdentityProfilesCorrect = table.Column<bool>(type: "boolean", nullable: false),
                    IsLocationCorrect = table.Column<bool>(type: "boolean", nullable: false),
                    IsProviderDetailsCorrect = table.Column<bool>(type: "boolean", nullable: false),
                    IsQualityAssessmentCorrect = table.Column<bool>(type: "boolean", nullable: false),
                    IsRolesCertifiedCorrect = table.Column<bool>(type: "boolean", nullable: false),
                    IsServiceNameCorrect = table.Column<bool>(type: "boolean", nullable: false),
                    IsServiceProvisionCorrect = table.Column<bool>(type: "boolean", nullable: false),
                    IsServiceSummaryCorrect = table.Column<bool>(type: "boolean", nullable: false),
                    IsURLLinkToServiceCorrect = table.Column<bool>(type: "boolean", nullable: false),
                    ModifiedBy = table.Column<string>(type: "text", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    RejectionComments = table.Column<string>(type: "text", nullable: true),
                    VerifiedUser = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CertificateReview", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CertificateReview_PreRegistration_PreRegistrationId",
                        column: x => x.PreRegistrationId,
                        principalTable: "PreRegistration",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CertificateReview_Provider_ProviderId",
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

            migrationBuilder.CreateIndex(
                name: "IX_ConsentToken_CertificateReviewId",
                table: "ConsentToken",
                column: "CertificateReviewId");

            migrationBuilder.CreateIndex(
                name: "IX_CertificateReview_PreRegistrationId",
                table: "CertificateReview",
                column: "PreRegistrationId");

            migrationBuilder.CreateIndex(
                name: "IX_CertificateReview_ProviderId",
                table: "CertificateReview",
                column: "ProviderId");

            migrationBuilder.AddForeignKey(
                name: "FK_ConsentToken_CertificateReview_CertificateReviewId",
                table: "ConsentToken",
                column: "CertificateReviewId",
                principalTable: "CertificateReview",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
