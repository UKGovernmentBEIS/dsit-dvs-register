using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DVSRegister.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddRemovalRequetTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProviderRemovalRequest",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProviderProfileId = table.Column<int>(type: "integer", nullable: false),
                    RemovalReason = table.Column<int>(type: "integer", nullable: false),
                    TokenId = table.Column<string>(type: "text", nullable: true),
                    Token = table.Column<string>(type: "text", nullable: true),
                    RemovalRequestedUserId = table.Column<int>(type: "integer", nullable: true),
                    IsRequestPending = table.Column<bool>(type: "boolean", nullable: false),
                    RemovalRequestTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    RemovedTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    PreviousProviderStatus = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProviderRemovalRequest", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProviderRemovalRequest_ProviderProfile_ProviderProfileId",
                        column: x => x.ProviderProfileId,
                        principalTable: "ProviderProfile",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProviderRemovalRequest_User_RemovalRequestedUserId",
                        column: x => x.RemovalRequestedUserId,
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ServiceRemovalRequest",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ServiceId = table.Column<int>(type: "integer", nullable: false),
                    ServiceRemovalReason = table.Column<int>(type: "integer", nullable: true),
                    RemovalReasonByCab = table.Column<string>(type: "text", nullable: true),
                    TokenId = table.Column<string>(type: "text", nullable: true),
                    Token = table.Column<string>(type: "text", nullable: true),
                    RemovalRequestedUserId = table.Column<int>(type: "integer", nullable: true),
                    RemovalRequestedCabUserId = table.Column<int>(type: "integer", nullable: true),
                    IsRequestPending = table.Column<bool>(type: "boolean", nullable: false),
                    RemovalRequestTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    RemovedTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    PreviousServiceStatus = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceRemovalRequest", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiceRemovalRequest_CabUser_RemovalRequestedCabUserId",
                        column: x => x.RemovalRequestedCabUserId,
                        principalTable: "CabUser",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ServiceRemovalRequest_Service_ServiceId",
                        column: x => x.ServiceId,
                        principalTable: "Service",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServiceRemovalRequest_User_RemovalRequestedUserId",
                        column: x => x.RemovalRequestedUserId,
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProviderRemovalRequest_ProviderProfileId",
                table: "ProviderRemovalRequest",
                column: "ProviderProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_ProviderRemovalRequest_RemovalRequestedUserId",
                table: "ProviderRemovalRequest",
                column: "RemovalRequestedUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceRemovalRequest_RemovalRequestedCabUserId",
                table: "ServiceRemovalRequest",
                column: "RemovalRequestedCabUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceRemovalRequest_RemovalRequestedUserId",
                table: "ServiceRemovalRequest",
                column: "RemovalRequestedUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceRemovalRequest_ServiceId",
                table: "ServiceRemovalRequest",
                column: "ServiceId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProviderRemovalRequest");

            migrationBuilder.DropTable(
                name: "ServiceRemovalRequest");
        }
    }
}
