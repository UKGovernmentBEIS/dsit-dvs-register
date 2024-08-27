using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DVSRegister.Data.Migrations
{
    /// <inheritdoc />
    public partial class CreateCertificateReviewTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CertificateReview",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ServiceId = table.Column<int>(type: "integer", nullable: false),
                    ProviProviderProfileId = table.Column<int>(type: "integer", nullable: false),
                    IsCabLogoCorrect = table.Column<bool>(type: "boolean", nullable: false),
                    IsCabDetailsCorrect = table.Column<bool>(type: "boolean", nullable: false),
                    IsProviderDetailsCorrect = table.Column<bool>(type: "boolean", nullable: false),
                    IsServiceNameCorrect = table.Column<bool>(type: "boolean", nullable: false),
                    IsRolesCertifiedCorrect = table.Column<bool>(type: "boolean", nullable: false),
                    IsCertificationScopeCorrect = table.Column<bool>(type: "boolean", nullable: false),
                    IsServiceSummaryCorrect = table.Column<bool>(type: "boolean", nullable: false),
                    IsURLLinkToServiceCorrect = table.Column<bool>(type: "boolean", nullable: false),
                    IsGPG44Correct = table.Column<bool>(type: "boolean", nullable: false),
                    IsGPG45Correct = table.Column<bool>(type: "boolean", nullable: false),
                    IsServiceProvisionCorrect = table.Column<bool>(type: "boolean", nullable: false),
                    IsLocationCorrect = table.Column<bool>(type: "boolean", nullable: false),
                    IsDateOfIssueCorrect = table.Column<bool>(type: "boolean", nullable: false),
                    IsDateOfExpiryCorrect = table.Column<bool>(type: "boolean", nullable: false),
                    IsAuthenticyVerifiedCorrect = table.Column<bool>(type: "boolean", nullable: false),
                    Comments = table.Column<string>(type: "text", nullable: true),
                    InformationMatched = table.Column<bool>(type: "boolean", nullable: false),
                    CommentsForIncorrect = table.Column<string>(type: "text", nullable: false),
                    RejectionComments = table.Column<string>(type: "text", nullable: true),
                    VerifiedUser = table.Column<int>(type: "integer", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CertificateReviewStatus = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CertificateReview", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CertificateReview_ProviderProfile_ProviProviderProfileId",
                        column: x => x.ProviProviderProfileId,
                        principalTable: "ProviderProfile",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CertificateReview_Service_ServiceId",
                        column: x => x.ServiceId,
                        principalTable: "Service",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CertificateReviewRejectionReasonMapping",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CetificateReviewId = table.Column<int>(type: "integer", nullable: false),
                    CertificateReviewRejectionReasonId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CertificateReviewRejectionReasonMapping", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CertificateReviewRejectionReasonMapping_CertificateReviewRe~",
                        column: x => x.CertificateReviewRejectionReasonId,
                        principalTable: "CertificateReviewRejectionReason",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CertificateReviewRejectionReasonMapping_CertificateReview_C~",
                        column: x => x.CetificateReviewId,
                        principalTable: "CertificateReview",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedTime",
                value: new DateTime(2024, 8, 27, 21, 33, 53, 172, DateTimeKind.Utc).AddTicks(4848));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedTime",
                value: new DateTime(2024, 8, 27, 21, 33, 53, 172, DateTimeKind.Utc).AddTicks(4852));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedTime",
                value: new DateTime(2024, 8, 27, 21, 33, 53, 172, DateTimeKind.Utc).AddTicks(4853));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedTime",
                value: new DateTime(2024, 8, 27, 21, 33, 53, 172, DateTimeKind.Utc).AddTicks(4855));

            migrationBuilder.CreateIndex(
                name: "IX_CertificateReview_ProviProviderProfileId",
                table: "CertificateReview",
                column: "ProviProviderProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_CertificateReview_ServiceId",
                table: "CertificateReview",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_CertificateReviewRejectionReasonMapping_CertificateReviewRe~",
                table: "CertificateReviewRejectionReasonMapping",
                column: "CertificateReviewRejectionReasonId");

            migrationBuilder.CreateIndex(
                name: "IX_CertificateReviewRejectionReasonMapping_CetificateReviewId",
                table: "CertificateReviewRejectionReasonMapping",
                column: "CetificateReviewId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CertificateReviewRejectionReasonMapping");

            migrationBuilder.DropTable(
                name: "CertificateReview");

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
    }
}
