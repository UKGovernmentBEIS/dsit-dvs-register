using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DVSRegister.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddServiceBulkRemovalTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ServiceBulkRemovalRequestId",
                table: "ServiceRemovalRequest",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ServiceBulkRemovalRequest",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Comment = table.Column<string>(type: "text", nullable: false),
                    IsRequestPending = table.Column<bool>(type: "boolean", nullable: false),
                    RequestedTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    RequestedBy = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceBulkRemovalRequest", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ServiceBulkRemovalRequestDraft",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ServiceId = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    TimeStamp = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceBulkRemovalRequestDraft", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ServiceRemovalRequest_ServiceBulkRemovalRequestId",
                table: "ServiceRemovalRequest",
                column: "ServiceBulkRemovalRequestId");

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceRemovalRequest_ServiceBulkRemovalRequest_ServiceBulk~",
                table: "ServiceRemovalRequest",
                column: "ServiceBulkRemovalRequestId",
                principalTable: "ServiceBulkRemovalRequest",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ServiceRemovalRequest_ServiceBulkRemovalRequest_ServiceBulk~",
                table: "ServiceRemovalRequest");

            migrationBuilder.DropTable(
                name: "ServiceBulkRemovalRequest");

            migrationBuilder.DropTable(
                name: "ServiceBulkRemovalRequestDraft");

            migrationBuilder.DropIndex(
                name: "IX_ServiceRemovalRequest_ServiceBulkRemovalRequestId",
                table: "ServiceRemovalRequest");

            migrationBuilder.DropColumn(
                name: "ServiceBulkRemovalRequestId",
                table: "ServiceRemovalRequest");
        }
    }
}
