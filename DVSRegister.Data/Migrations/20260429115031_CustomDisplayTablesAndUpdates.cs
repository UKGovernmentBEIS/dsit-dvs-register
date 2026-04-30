using System;
using System.Text.Json;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DVSRegister.Data.Migrations
{
    /// <inheritdoc />
    public partial class CustomDisplayTablesAndUpdates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ServiceSupSchemeCustomDisplayId",
                table: "ServiceSupSchemeMapping",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ServiceCustomDisplayChangeRequestId",
                table: "ActionLogs",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ServiceCustomDisplayChangeRequest",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ServiceId = table.Column<int>(type: "integer", nullable: false),
                    OldValue = table.Column<JsonDocument>(type: "jsonb", nullable: false),
                    NewValue = table.Column<JsonDocument>(type: "jsonb", nullable: false),
                    HiddenValue = table.Column<JsonDocument>(type: "jsonb", nullable: false),
                    Comments = table.Column<string>(type: "text", nullable: false),
                    RequestedUserId = table.Column<int>(type: "integer", nullable: false),
                    ModifiedUserId = table.Column<int>(type: "integer", nullable: true),
                    RequestedTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ModifiedTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsRequestPending = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceCustomDisplayChangeRequest", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiceCustomDisplayChangeRequest_Service_ServiceId",
                        column: x => x.ServiceId,
                        principalTable: "Service",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServiceCustomDisplayChangeRequest_User_ModifiedUserId",
                        column: x => x.ModifiedUserId,
                        principalTable: "User",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ServiceCustomDisplayChangeRequest_User_RequestedUserId",
                        column: x => x.RequestedUserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServiceSupSchemeCustomDisplay",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ServiceSupSchemeMappingId = table.Column<int>(type: "integer", nullable: false),
                    IsSupplementarySchemeHidden = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceSupSchemeCustomDisplay", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiceSupSchemeCustomDisplay_ServiceSupSchemeMapping_Servi~",
                        column: x => x.ServiceSupSchemeMappingId,
                        principalTable: "ServiceSupSchemeMapping",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "ActionCategory",
                keyColumn: "Id",
                keyValue: 5,
                column: "ActionName",
                value: "Service removal, reassign, custom display");

            migrationBuilder.InsertData(
                table: "ActionDetails",
                columns: new[] { "Id", "ActionCategoryId", "ActionDescription", "ActionDetailsKey" },
                values: new object[,]
                {
                    { 35, 5, "Display change request sent", "DisplayChangeRequestSent" },
                    { 36, 5, "Display change completed", "DisplayChangeCompleted" },
                    { 37, 5, "Display change request cancelled", "DisplayChangeRequestCancelled" },
                    { 38, 5, "Display change request declined", "DisplayChangeRequestDeclined" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ServiceSupSchemeMapping_ServiceSupSchemeCustomDisplayId",
                table: "ServiceSupSchemeMapping",
                column: "ServiceSupSchemeCustomDisplayId");

            migrationBuilder.CreateIndex(
                name: "IX_ActionLogs_ServiceCustomDisplayChangeRequestId",
                table: "ActionLogs",
                column: "ServiceCustomDisplayChangeRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceCustomDisplayChangeRequest_ModifiedUserId",
                table: "ServiceCustomDisplayChangeRequest",
                column: "ModifiedUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceCustomDisplayChangeRequest_RequestedUserId",
                table: "ServiceCustomDisplayChangeRequest",
                column: "RequestedUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceCustomDisplayChangeRequest_ServiceId",
                table: "ServiceCustomDisplayChangeRequest",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceSupSchemeCustomDisplay_ServiceSupSchemeMappingId",
                table: "ServiceSupSchemeCustomDisplay",
                column: "ServiceSupSchemeMappingId");

            migrationBuilder.AddForeignKey(
                name: "FK_ActionLogs_ServiceCustomDisplayChangeRequest_ServiceCustomD~",
                table: "ActionLogs",
                column: "ServiceCustomDisplayChangeRequestId",
                principalTable: "ServiceCustomDisplayChangeRequest",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceSupSchemeMapping_ServiceSupSchemeCustomDisplay_Servi~",
                table: "ServiceSupSchemeMapping",
                column: "ServiceSupSchemeCustomDisplayId",
                principalTable: "ServiceSupSchemeCustomDisplay",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ActionLogs_ServiceCustomDisplayChangeRequest_ServiceCustomD~",
                table: "ActionLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceSupSchemeMapping_ServiceSupSchemeCustomDisplay_Servi~",
                table: "ServiceSupSchemeMapping");

            migrationBuilder.DropTable(
                name: "ServiceCustomDisplayChangeRequest");

            migrationBuilder.DropTable(
                name: "ServiceSupSchemeCustomDisplay");

            migrationBuilder.DropIndex(
                name: "IX_ServiceSupSchemeMapping_ServiceSupSchemeCustomDisplayId",
                table: "ServiceSupSchemeMapping");

            migrationBuilder.DropIndex(
                name: "IX_ActionLogs_ServiceCustomDisplayChangeRequestId",
                table: "ActionLogs");

            migrationBuilder.DeleteData(
                table: "ActionDetails",
                keyColumn: "Id",
                keyValue: 35);

            migrationBuilder.DeleteData(
                table: "ActionDetails",
                keyColumn: "Id",
                keyValue: 36);

            migrationBuilder.DeleteData(
                table: "ActionDetails",
                keyColumn: "Id",
                keyValue: 37);

            migrationBuilder.DeleteData(
                table: "ActionDetails",
                keyColumn: "Id",
                keyValue: 38);

            migrationBuilder.DropColumn(
                name: "ServiceSupSchemeCustomDisplayId",
                table: "ServiceSupSchemeMapping");

            migrationBuilder.DropColumn(
                name: "ServiceCustomDisplayChangeRequestId",
                table: "ActionLogs");

            migrationBuilder.UpdateData(
                table: "ActionCategory",
                keyColumn: "Id",
                keyValue: 5,
                column: "ActionName",
                value: "Service removal, reassign");
        }
    }
}
