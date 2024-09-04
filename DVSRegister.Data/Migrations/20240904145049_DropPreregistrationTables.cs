using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DVSRegister.Data.Migrations
{
    /// <inheritdoc />
    public partial class DropPreregistrationTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Provider_PreRegistration_PreRegistrationId",
                table: "Provider");

            migrationBuilder.DropTable(
                name: "PreRegistrationCountryMapping");

            migrationBuilder.DropTable(
                name: "PreRegistrationReview");

            migrationBuilder.DropTable(
                name: "UniqueReferenceNumber");

            migrationBuilder.DropTable(
                name: "Country");

            migrationBuilder.DropTable(
                name: "PreRegistration");

            migrationBuilder.DropIndex(
                name: "IX_Provider_PreRegistrationId",
                table: "Provider");

            migrationBuilder.DropIndex(
                name: "IX_CertificateReview_ServiceId",
                table: "CertificateReview");

            migrationBuilder.DropColumn(
                name: "PreRegistrationId",
                table: "Provider");

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedTime",
                value: new DateTime(2024, 9, 4, 14, 50, 48, 676, DateTimeKind.Utc).AddTicks(2173));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedTime",
                value: new DateTime(2024, 9, 4, 14, 50, 48, 676, DateTimeKind.Utc).AddTicks(2180));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedTime",
                value: new DateTime(2024, 9, 4, 14, 50, 48, 676, DateTimeKind.Utc).AddTicks(2181));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedTime",
                value: new DateTime(2024, 9, 4, 14, 50, 48, 676, DateTimeKind.Utc).AddTicks(2182));

            migrationBuilder.CreateIndex(
                name: "IX_CertificateReview_ServiceId",
                table: "CertificateReview",
                column: "ServiceId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CertificateReview_ServiceId",
                table: "CertificateReview");

            migrationBuilder.AddColumn<int>(
                name: "PreRegistrationId",
                table: "Provider",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Country",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CountryName = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Country", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PreRegistration",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ApplicationReviewStatus = table.Column<int>(type: "integer", nullable: false),
                    CompanyRegistrationNumber = table.Column<string>(type: "varchar(8)", nullable: false),
                    ConfirmAccuracy = table.Column<int>(type: "integer", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Email = table.Column<string>(type: "varchar(254)", nullable: false),
                    FullName = table.Column<string>(type: "text", nullable: false),
                    HasParentCompany = table.Column<bool>(type: "boolean", nullable: false),
                    IsApplicationSponsor = table.Column<bool>(type: "boolean", nullable: false),
                    JobTitle = table.Column<string>(type: "text", nullable: false),
                    ModifiedBy = table.Column<string>(type: "text", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ParentCompanyLocation = table.Column<string>(type: "varchar(160)", nullable: true),
                    ParentCompanyRegisteredName = table.Column<string>(type: "varchar(160)", nullable: true),
                    RegisteredCompanyName = table.Column<string>(type: "varchar(160)", nullable: false),
                    SponsorEmail = table.Column<string>(type: "varchar(254)", nullable: true),
                    SponsorFullName = table.Column<string>(type: "text", nullable: true),
                    SponsorJobTitle = table.Column<string>(type: "text", nullable: true),
                    SponsorTelephoneNumber = table.Column<string>(type: "text", nullable: true),
                    TelephoneNumber = table.Column<string>(type: "text", nullable: false),
                    TradingName = table.Column<string>(type: "text", nullable: false),
                    URN = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PreRegistration", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PreRegistrationCountryMapping",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CountryId = table.Column<int>(type: "integer", nullable: false),
                    PreRegistrationId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PreRegistrationCountryMapping", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PreRegistrationCountryMapping_Country_CountryId",
                        column: x => x.CountryId,
                        principalTable: "Country",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PreRegistrationCountryMapping_PreRegistration_PreRegistrati~",
                        column: x => x.PreRegistrationId,
                        principalTable: "PreRegistration",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PreRegistrationReview",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PreRegistrationId = table.Column<int>(type: "integer", nullable: false),
                    PrimaryCheckUserId = table.Column<int>(type: "integer", nullable: false),
                    SecondaryCheckUserId = table.Column<int>(type: "integer", nullable: true),
                    ApplicationReviewStatus = table.Column<int>(type: "integer", nullable: false),
                    Comment = table.Column<string>(type: "text", nullable: true),
                    IsBannedPoliticalApproved = table.Column<bool>(type: "boolean", nullable: false),
                    IsCheckListApproved = table.Column<bool>(type: "boolean", nullable: false),
                    IsCompanyApproved = table.Column<bool>(type: "boolean", nullable: false),
                    IsCountryApproved = table.Column<bool>(type: "boolean", nullable: false),
                    IsDirectorshipsAndRelationApproved = table.Column<bool>(type: "boolean", nullable: false),
                    IsDirectorshipsApproved = table.Column<bool>(type: "boolean", nullable: false),
                    IsECCheckApproved = table.Column<bool>(type: "boolean", nullable: false),
                    IsProvidersWebpageApproved = table.Column<bool>(type: "boolean", nullable: false),
                    IsSanctionListApproved = table.Column<bool>(type: "boolean", nullable: false),
                    IsTARICApproved = table.Column<bool>(type: "boolean", nullable: false),
                    IsTradingAddressApproved = table.Column<bool>(type: "boolean", nullable: false),
                    IsUNFCApproved = table.Column<bool>(type: "boolean", nullable: false),
                    PrimaryCheckTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    RejectionReason = table.Column<int>(type: "integer", nullable: true),
                    SecondaryCheckTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PreRegistrationReview", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PreRegistrationReview_PreRegistration_PreRegistrationId",
                        column: x => x.PreRegistrationId,
                        principalTable: "PreRegistration",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PreRegistrationReview_User_PrimaryCheckUserId",
                        column: x => x.PrimaryCheckUserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PreRegistrationReview_User_SecondaryCheckUserId",
                        column: x => x.SecondaryCheckUserId,
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "UniqueReferenceNumber",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PreRegistrationId = table.Column<int>(type: "integer", nullable: true),
                    CheckedByCAB = table.Column<string>(type: "text", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastCheckedTimeStamp = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ModifiedBy = table.Column<string>(type: "text", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    RegisteredDIPName = table.Column<string>(type: "text", nullable: true),
                    ReleasedTimeStamp = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    URN = table.Column<string>(type: "varchar(100)", nullable: false),
                    URNStatus = table.Column<int>(type: "integer", nullable: false),
                    Validity = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UniqueReferenceNumber", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UniqueReferenceNumber_PreRegistration_PreRegistrationId",
                        column: x => x.PreRegistrationId,
                        principalTable: "PreRegistration",
                        principalColumn: "Id");
                });

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedTime",
                value: new DateTime(2024, 9, 3, 10, 18, 35, 671, DateTimeKind.Utc).AddTicks(3353));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedTime",
                value: new DateTime(2024, 9, 3, 10, 18, 35, 671, DateTimeKind.Utc).AddTicks(3357));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedTime",
                value: new DateTime(2024, 9, 3, 10, 18, 35, 671, DateTimeKind.Utc).AddTicks(3359));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedTime",
                value: new DateTime(2024, 9, 3, 10, 18, 35, 671, DateTimeKind.Utc).AddTicks(3360));

            migrationBuilder.CreateIndex(
                name: "IX_Provider_PreRegistrationId",
                table: "Provider",
                column: "PreRegistrationId");

            migrationBuilder.CreateIndex(
                name: "IX_CertificateReview_ServiceId",
                table: "CertificateReview",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_PreRegistrationCountryMapping_CountryId",
                table: "PreRegistrationCountryMapping",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_PreRegistrationCountryMapping_PreRegistrationId",
                table: "PreRegistrationCountryMapping",
                column: "PreRegistrationId");

            migrationBuilder.CreateIndex(
                name: "IX_PreRegistrationReview_PreRegistrationId",
                table: "PreRegistrationReview",
                column: "PreRegistrationId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PreRegistrationReview_PrimaryCheckUserId",
                table: "PreRegistrationReview",
                column: "PrimaryCheckUserId");

            migrationBuilder.CreateIndex(
                name: "IX_PreRegistrationReview_SecondaryCheckUserId",
                table: "PreRegistrationReview",
                column: "SecondaryCheckUserId");

            migrationBuilder.CreateIndex(
                name: "IX_UniqueReferenceNumber_PreRegistrationId",
                table: "UniqueReferenceNumber",
                column: "PreRegistrationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Provider_PreRegistration_PreRegistrationId",
                table: "Provider",
                column: "PreRegistrationId",
                principalTable: "PreRegistration",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
