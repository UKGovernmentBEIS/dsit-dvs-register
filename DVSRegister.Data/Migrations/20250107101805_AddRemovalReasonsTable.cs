using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DVSRegister.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddRemovalReasonsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RemovalReasons",
                columns: table => new
                {
                    RemovalReasonId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RemovalReason = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    TimeCreated = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    TimeUpdated = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    IsActiveReason = table.Column<bool>(type: "boolean", nullable: false),
                    RequiresAdditionalInfo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RemovalReasons", x => x.RemovalReasonId);
                });

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedTime",
                value: new DateTime(2025, 1, 7, 10, 18, 4, 527, DateTimeKind.Utc).AddTicks(6388));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedTime",
                value: new DateTime(2025, 1, 7, 10, 18, 4, 527, DateTimeKind.Utc).AddTicks(6391));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedTime",
                value: new DateTime(2025, 1, 7, 10, 18, 4, 527, DateTimeKind.Utc).AddTicks(6392));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedTime",
                value: new DateTime(2025, 1, 7, 10, 18, 4, 527, DateTimeKind.Utc).AddTicks(6393));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedTime",
                value: new DateTime(2025, 1, 7, 10, 18, 4, 527, DateTimeKind.Utc).AddTicks(6394));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedTime",
                value: new DateTime(2025, 1, 7, 10, 18, 4, 527, DateTimeKind.Utc).AddTicks(6395));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RemovalReasons");

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedTime",
                value: new DateTime(2024, 12, 19, 13, 50, 7, 961, DateTimeKind.Utc).AddTicks(6518));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedTime",
                value: new DateTime(2024, 12, 19, 13, 50, 7, 961, DateTimeKind.Utc).AddTicks(6523));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedTime",
                value: new DateTime(2024, 12, 19, 13, 50, 7, 961, DateTimeKind.Utc).AddTicks(6525));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedTime",
                value: new DateTime(2024, 12, 19, 13, 50, 7, 961, DateTimeKind.Utc).AddTicks(6527));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedTime",
                value: new DateTime(2024, 12, 19, 13, 50, 7, 961, DateTimeKind.Utc).AddTicks(6528));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedTime",
                value: new DateTime(2024, 12, 19, 13, 50, 7, 961, DateTimeKind.Utc).AddTicks(6530));
        }
    }
}
