using System;
using System.Text.Json;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DVSRegister.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddActionLogsTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ActionCategory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ActionKey = table.Column<string>(type: "text", nullable: false),
                    ActionName = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActionCategory", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ActionDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ActionDetailsKey = table.Column<string>(type: "text", nullable: false),
                    ActionDescription = table.Column<string>(type: "text", nullable: false),
                    ActionCategoryId = table.Column<int>(type: "integer", nullable: false),
                    ShowInRegisterUpdates = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActionDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ActionDetails_ActionCategory_ActionCategoryId",
                        column: x => x.ActionCategoryId,
                        principalTable: "ActionCategory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ActionLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ActionCategoryId = table.Column<int>(type: "integer", nullable: false),
                    ActionDetailsId = table.Column<int>(type: "integer", nullable: false),
                    ProviderProfileId = table.Column<int>(type: "integer", nullable: false),
                    ServiceId = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: true),
                    CabUserId = table.Column<int>(type: "integer", nullable: true),
                    DisplayMessage = table.Column<string>(type: "text", nullable: false),
                    OldValues = table.Column<JsonDocument>(type: "jsonb", nullable: true),
                    NewValues = table.Column<JsonDocument>(type: "jsonb", nullable: true),
                    LogDate = table.Column<DateTime>(type: "date", nullable: false),
                    LoggedTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActionLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ActionLogs_ActionCategory_ActionCategoryId",
                        column: x => x.ActionCategoryId,
                        principalTable: "ActionCategory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ActionLogs_ActionDetails_ActionDetailsId",
                        column: x => x.ActionDetailsId,
                        principalTable: "ActionDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ActionLogs_CabUser_CabUserId",
                        column: x => x.CabUserId,
                        principalTable: "CabUser",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ActionLogs_ProviderProfile_ProviderProfileId",
                        column: x => x.ProviderProfileId,
                        principalTable: "ProviderProfile",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ActionLogs_Service_ServiceId",
                        column: x => x.ServiceId,
                        principalTable: "Service",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ActionLogs_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "ActionCategory",
                columns: new[] { "Id", "ActionKey", "ActionName" },
                values: new object[,]
                {
                    { 1, "CR", "Certificate review" },
                    { 2, "PI", "Public interest checks" },
                    { 3, "ServiceUpdates", "Service updates" },
                    { 4, "ProviderUpdates", "Provider updates" }
                });

            migrationBuilder.InsertData(
                table: "ActionDetails",
                columns: new[] { "Id", "ActionCategoryId", "ActionDescription", "ActionDetailsKey", "ShowInRegisterUpdates" },
                values: new object[,]
                {
                    { 1, 1, "Passed", "CR_APR", false },
                    { 2, 1, "Rejected", "CR_Rej", false },
                    { 3, 1, "Restored", "CR_Restore", false },
                    { 4, 1, "Sent back to CAB", "CR_SentBack", false },
                    { 5, 1, "Declined by provider", "CR_DeclinedByProvider", false },
                    { 6, 2, "Primary check passed", "PI_Primary_Pass", false },
                    { 7, 2, "Sent back by second reviewer", "PI_SentBack", false },
                    { 8, 2, "Primary check failed", "PI_Primary_Fail", false },
                    { 9, 2, "Application rejected", "PI_Fail", false },
                    { 10, 2, "Publication of provider", "PI_ProviderPublish", true },
                    { 11, 2, "Service published", "PI_ServicePublish", true },
                    { 12, 2, "Service updated", "PI_ServiceRePublish", true },
                    { 13, 3, "Service name changed", "ServiceNameUpdate", true },
                    { 14, 3, "Updates published", "ServiceUpdates", true },
                    { 15, 4, "Contact details changed", "ProviderContactUpdate", true },
                    { 16, 4, "Trading name changed", "TradingNameUpdate", true },
                    { 17, 4, "Registered name changed", "RegisteredNameUpdate", true }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ActionCategory_ActionKey",
                table: "ActionCategory",
                column: "ActionKey",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ActionDetails_ActionCategoryId",
                table: "ActionDetails",
                column: "ActionCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ActionDetails_ActionDetailsKey",
                table: "ActionDetails",
                column: "ActionDetailsKey",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ActionLogs_ActionCategoryId",
                table: "ActionLogs",
                column: "ActionCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ActionLogs_ActionDetailsId",
                table: "ActionLogs",
                column: "ActionDetailsId");

            migrationBuilder.CreateIndex(
                name: "IX_ActionLogs_CabUserId",
                table: "ActionLogs",
                column: "CabUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ActionLogs_ProviderProfileId",
                table: "ActionLogs",
                column: "ProviderProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_ActionLogs_ServiceId",
                table: "ActionLogs",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_ActionLogs_UserId",
                table: "ActionLogs",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ActionLogs");

            migrationBuilder.DropTable(
                name: "ActionDetails");

            migrationBuilder.DropTable(
                name: "ActionCategory");
        }
    }
}
