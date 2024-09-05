using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DVSRegister.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPublicInterestCheckTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PublicInterestCheck",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ServiceId = table.Column<int>(type: "integer", nullable: false),
                    ProviderProfileId = table.Column<int>(type: "integer", nullable: false),
                    IsCompanyHouseNumberApproved = table.Column<bool>(type: "boolean", nullable: false),
                    IsDirectorshipsApproved = table.Column<bool>(type: "boolean", nullable: false),
                    IsDirectorshipsAndRelationApproved = table.Column<bool>(type: "boolean", nullable: false),
                    IsTradingAddressApproved = table.Column<bool>(type: "boolean", nullable: false),
                    IsSanctionListApproved = table.Column<bool>(type: "boolean", nullable: false),
                    IsUNFCApproved = table.Column<bool>(type: "boolean", nullable: false),
                    IsECCheckApproved = table.Column<bool>(type: "boolean", nullable: false),
                    IsTARICApproved = table.Column<bool>(type: "boolean", nullable: false),
                    IsBannedPoliticalApproved = table.Column<bool>(type: "boolean", nullable: false),
                    IsProvidersWebpageApproved = table.Column<bool>(type: "boolean", nullable: false),
                    PublicInterestCheckStatus = table.Column<int>(type: "integer", nullable: false),
                    RejectionReason = table.Column<int>(type: "integer", nullable: true),
                    PrimaryCheckComment = table.Column<string>(type: "text", nullable: true),
                    SecondaryCheckComment = table.Column<string>(type: "text", nullable: true),
                    PrimaryCheckUserId = table.Column<int>(type: "integer", nullable: false),
                    PrimaryCheckTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    SecondaryCheckUserId = table.Column<int>(type: "integer", nullable: true),
                    SecondaryCheckTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PublicInterestCheck", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PublicInterestCheck_ProviderProfile_ProviderProfileId",
                        column: x => x.ProviderProfileId,
                        principalTable: "ProviderProfile",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PublicInterestCheck_Service_ServiceId",
                        column: x => x.ServiceId,
                        principalTable: "Service",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PublicInterestCheck_User_PrimaryCheckUserId",
                        column: x => x.PrimaryCheckUserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PublicInterestCheck_User_SecondaryCheckUserId",
                        column: x => x.SecondaryCheckUserId,
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedTime",
                value: new DateTime(2024, 9, 5, 8, 24, 43, 396, DateTimeKind.Utc).AddTicks(8408));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedTime",
                value: new DateTime(2024, 9, 5, 8, 24, 43, 396, DateTimeKind.Utc).AddTicks(8412));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedTime",
                value: new DateTime(2024, 9, 5, 8, 24, 43, 396, DateTimeKind.Utc).AddTicks(8413));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedTime",
                value: new DateTime(2024, 9, 5, 8, 24, 43, 396, DateTimeKind.Utc).AddTicks(8414));

            migrationBuilder.CreateIndex(
                name: "IX_PublicInterestCheck_PrimaryCheckUserId",
                table: "PublicInterestCheck",
                column: "PrimaryCheckUserId");

            migrationBuilder.CreateIndex(
                name: "IX_PublicInterestCheck_ProviderProfileId",
                table: "PublicInterestCheck",
                column: "ProviderProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_PublicInterestCheck_SecondaryCheckUserId",
                table: "PublicInterestCheck",
                column: "SecondaryCheckUserId");

            migrationBuilder.CreateIndex(
                name: "IX_PublicInterestCheck_ServiceId",
                table: "PublicInterestCheck",
                column: "ServiceId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PublicInterestCheck");

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
        }
    }
}
