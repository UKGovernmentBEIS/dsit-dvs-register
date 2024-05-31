using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DVSRegister.Data.Migrations
{
    /// <inheritdoc />
    public partial class CertificateReviewTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "User",
                type: "timestamp without time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "User",
                type: "timestamp without time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "ReleasedTimeStamp",
                table: "UniqueReferenceNumber",
                type: "timestamp without time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "UniqueReferenceNumber",
                type: "timestamp without time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastCheckedTimeStamp",
                table: "UniqueReferenceNumber",
                type: "timestamp without time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "UniqueReferenceNumber",
                type: "timestamp without time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "SecondaryCheckTime",
                table: "PreRegistrationReview",
                type: "timestamp without time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "PrimaryCheckTime",
                table: "PreRegistrationReview",
                type: "timestamp without time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "PreRegistration",
                type: "timestamp without time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "PreRegistration",
                type: "timestamp without time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "CertificateInformation",
                type: "timestamp without time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "CertificateInformation",
                type: "timestamp without time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "ConformityIssueDate",
                table: "CertificateInformation",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ConformityExpiryDate",
                table: "CertificateInformation",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.CreateTable(
                name: "CertificateReviewRejectionReason",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Reason = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CertificateReviewRejectionReason", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CetificateReview",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PreRegistrationId = table.Column<int>(type: "integer", nullable: false),
                    CertificateInformationId = table.Column<int>(type: "integer", nullable: false),
                    IsCabLogoCorrect = table.Column<bool>(type: "boolean", nullable: false),
                    IsCabDetailsCorrect = table.Column<bool>(type: "boolean", nullable: false),
                    IsProviderDetailsCorrect = table.Column<bool>(type: "boolean", nullable: false),
                    IsServiceNameCorrect = table.Column<bool>(type: "boolean", nullable: false),
                    IsRolesCertifiedCorrect = table.Column<bool>(type: "boolean", nullable: false),
                    IsCertificationScopeCorrect = table.Column<bool>(type: "boolean", nullable: false),
                    IsServiceSummaryCorrect = table.Column<bool>(type: "boolean", nullable: false),
                    IsURLLinkToServiceCorrect = table.Column<bool>(type: "boolean", nullable: false),
                    IsIdentityProfilesCorrect = table.Column<bool>(type: "boolean", nullable: false),
                    IsQualityAssessmentCorrect = table.Column<bool>(type: "boolean", nullable: false),
                    IsServiceProvisionCorrect = table.Column<bool>(type: "boolean", nullable: false),
                    IsLocationCorrect = table.Column<bool>(type: "boolean", nullable: false),
                    IsDateOfIssueCorrect = table.Column<bool>(type: "boolean", nullable: false),
                    IsDateOfExpiryCorrect = table.Column<bool>(type: "boolean", nullable: false),
                    IsAuthenticyVerifiedCorrect = table.Column<bool>(type: "boolean", nullable: false),
                    CommentsForIncorrect = table.Column<string>(type: "text", nullable: false),
                    InformationMatched = table.Column<bool>(type: "boolean", nullable: true),
                    Comments = table.Column<string>(type: "text", nullable: true),
                    RejectionComments = table.Column<string>(type: "text", nullable: true),
                    VerifiedUser = table.Column<int>(type: "integer", nullable: false),
                    CertificateInfoStatus = table.Column<int>(type: "integer", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ModifiedBy = table.Column<string>(type: "text", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CetificateReview", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CetificateReview_CertificateInformation_CertificateInformat~",
                        column: x => x.CertificateInformationId,
                        principalTable: "CertificateInformation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CetificateReview_PreRegistration_PreRegistrationId",
                        column: x => x.PreRegistrationId,
                        principalTable: "PreRegistration",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CertificateReviewRejectionReasonMappings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CetificateReviewId = table.Column<int>(type: "integer", nullable: false),
                    CertificateReviewRejectionReasonId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CertificateReviewRejectionReasonMappings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CertificateReviewRejectionReasonMappings_CertificateReviewR~",
                        column: x => x.CertificateReviewRejectionReasonId,
                        principalTable: "CertificateReviewRejectionReason",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CertificateReviewRejectionReasonMappings_CetificateReview_C~",
                        column: x => x.CetificateReviewId,
                        principalTable: "CetificateReview",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "CertificateReviewRejectionReason",
                columns: new[] { "Id", "Reason" },
                values: new object[,]
                {
                    { 1, "Information is missing from the certificate" },
                    { 2, "The certificate contains invalid information" },
                    { 3, "The information submitted does not match the information on the certificate" },
                    { 4, "The certificate or information submitted contains errors" },
                    { 5, "Other" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_CertificateReviewRejectionReasonMappings_CertificateReviewR~",
                table: "CertificateReviewRejectionReasonMappings",
                column: "CertificateReviewRejectionReasonId");

            migrationBuilder.CreateIndex(
                name: "IX_CertificateReviewRejectionReasonMappings_CetificateReviewId",
                table: "CertificateReviewRejectionReasonMappings",
                column: "CetificateReviewId");

            migrationBuilder.CreateIndex(
                name: "IX_CetificateReview_CertificateInformationId",
                table: "CetificateReview",
                column: "CertificateInformationId");

            migrationBuilder.CreateIndex(
                name: "IX_CetificateReview_PreRegistrationId",
                table: "CetificateReview",
                column: "PreRegistrationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CertificateReviewRejectionReasonMappings");

            migrationBuilder.DropTable(
                name: "CertificateReviewRejectionReason");

            migrationBuilder.DropTable(
                name: "CetificateReview");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "User",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "User",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "ReleasedTimeStamp",
                table: "UniqueReferenceNumber",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "UniqueReferenceNumber",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastCheckedTimeStamp",
                table: "UniqueReferenceNumber",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "UniqueReferenceNumber",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "SecondaryCheckTime",
                table: "PreRegistrationReview",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "PrimaryCheckTime",
                table: "PreRegistrationReview",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "PreRegistration",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "PreRegistration",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "CertificateInformation",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "CertificateInformation",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "ConformityIssueDate",
                table: "CertificateInformation",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ConformityExpiryDate",
                table: "CertificateInformation",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");
        }
    }
}
