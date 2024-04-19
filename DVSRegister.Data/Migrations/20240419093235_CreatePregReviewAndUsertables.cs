using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DVSRegister.Data.Migrations
{
    /// <inheritdoc />
    public partial class CreatePregReviewAndUsertables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserName = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ModifiedBy = table.Column<string>(type: "text", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PreRegistrationReview",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PreRegistrationId = table.Column<int>(type: "integer", nullable: false),
                    IsCountryApproved = table.Column<bool>(type: "boolean", nullable: false),
                    IsCompanyApproved = table.Column<bool>(type: "boolean", nullable: false),
                    IsCheckListApproved = table.Column<bool>(type: "boolean", nullable: false),
                    IsDirectorshipsAndRelationApproved = table.Column<bool>(type: "boolean", nullable: false),
                    IsTradingAddressApproved = table.Column<bool>(type: "boolean", nullable: false),
                    IsSanctionListApproved = table.Column<bool>(type: "boolean", nullable: false),
                    IsUNFCApproved = table.Column<bool>(type: "boolean", nullable: false),
                    IsECCheckApproved = table.Column<bool>(type: "boolean", nullable: false),
                    IsTARICApproved = table.Column<bool>(type: "boolean", nullable: false),
                    IsBannedPoliticalApproved = table.Column<bool>(type: "boolean", nullable: false),
                    IsProvidersWebpageApproved = table.Column<bool>(type: "boolean", nullable: false),
                    ReviewProgress = table.Column<int>(type: "integer", nullable: false),
                    Comment = table.Column<string>(type: "text", nullable: true),
                    PrimaryCheckUserId = table.Column<int>(type: "integer", nullable: false),
                    PrimaryCheckTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    SecondaryCheckUserId = table.Column<int>(type: "integer", nullable: true),
                    SecondaryCheckTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
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

            migrationBuilder.InsertData(
                table: "User",
                columns: new[] { "Id", "CreatedBy", "CreatedDate", "Email", "ModifiedBy", "ModifiedDate", "UserName" },
                values: new object[,]
                {
                    { 1, "Dev", new DateTime(2024, 4, 19, 9, 32, 34, 599, DateTimeKind.Utc).AddTicks(6979), "Aiswarya.Rajendran@dsit.gov.uk", null, null, "Aiswarya" },
                    { 2, "Dev", new DateTime(2024, 4, 19, 9, 32, 34, 599, DateTimeKind.Utc).AddTicks(6988), "vishal.vishwanathan@ics.gov.uk", null, null, "Vishal" }
                });

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PreRegistrationReview");

            migrationBuilder.DropTable(
                name: "User");
        }
    }
}
