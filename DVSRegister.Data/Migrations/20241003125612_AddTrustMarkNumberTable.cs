using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DVSRegister.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddTrustMarkNumberTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TrustmarkNumber",
                columns: table => new
                {
                    CompanyId = table.Column<int>(type: "integer", nullable: false),
                    ServiceNumber = table.Column<int>(type: "integer", nullable: false),
                    ProviderProfileId = table.Column<int>(type: "integer", nullable: false),
                    ServiceId = table.Column<int>(type: "integer", nullable: false),
                    TrustMarkNumber = table.Column<string>(type: "text", nullable: false, computedColumnSql: "\"CompanyId\"::VARCHAR(4) || LPAD(\"ServiceNumber\"::VARCHAR(2), 2, '0')", stored: true),
                    TimeStamp = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrustmarkNumber", x => new { x.CompanyId, x.ServiceNumber });
                    table.CheckConstraint("CK_CompanyId", "\"CompanyId\" BETWEEN 2000 AND 9999");
                    table.CheckConstraint("CK_ServiceNumber", "\"ServiceNumber\" BETWEEN 1 AND 99");
                    table.ForeignKey(
                        name: "FK_TrustmarkNumber_ProviderProfile_ProviderProfileId",
                        column: x => x.ProviderProfileId,
                        principalTable: "ProviderProfile",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TrustmarkNumber_Service_ServiceId",
                        column: x => x.ServiceId,
                        principalTable: "Service",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedTime",
                value: new DateTime(2024, 10, 3, 12, 56, 11, 760, DateTimeKind.Utc).AddTicks(2256));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedTime",
                value: new DateTime(2024, 10, 3, 12, 56, 11, 760, DateTimeKind.Utc).AddTicks(2260));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedTime",
                value: new DateTime(2024, 10, 3, 12, 56, 11, 760, DateTimeKind.Utc).AddTicks(2261));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedTime",
                value: new DateTime(2024, 10, 3, 12, 56, 11, 760, DateTimeKind.Utc).AddTicks(2262));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedTime",
                value: new DateTime(2024, 10, 3, 12, 56, 11, 760, DateTimeKind.Utc).AddTicks(2264));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedTime",
                value: new DateTime(2024, 10, 3, 12, 56, 11, 760, DateTimeKind.Utc).AddTicks(2265));

            migrationBuilder.CreateIndex(
                name: "IX_TrustmarkNumber_ProviderProfileId_ServiceId",
                table: "TrustmarkNumber",
                columns: new[] { "ProviderProfileId", "ServiceId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TrustmarkNumber_ServiceId",
                table: "TrustmarkNumber",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_TrustmarkNumber_TrustMarkNumber",
                table: "TrustmarkNumber",
                column: "TrustMarkNumber",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TrustmarkNumber");

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedTime",
                value: new DateTime(2024, 9, 20, 15, 24, 4, 6, DateTimeKind.Utc).AddTicks(3151));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedTime",
                value: new DateTime(2024, 9, 20, 15, 24, 4, 6, DateTimeKind.Utc).AddTicks(3155));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedTime",
                value: new DateTime(2024, 9, 20, 15, 24, 4, 6, DateTimeKind.Utc).AddTicks(3156));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedTime",
                value: new DateTime(2024, 9, 20, 15, 24, 4, 6, DateTimeKind.Utc).AddTicks(3158));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedTime",
                value: new DateTime(2024, 9, 20, 15, 24, 4, 6, DateTimeKind.Utc).AddTicks(3159));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedTime",
                value: new DateTime(2024, 9, 20, 15, 24, 4, 6, DateTimeKind.Utc).AddTicks(3161));
        }
    }
}
