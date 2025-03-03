using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DVSRegister.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddRemoveTokenTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "RemovalReasons",
                keyColumn: "RemovalReasonId",
                keyValue: 2);

            migrationBuilder.CreateTable(
                name: "RemoveProviderToken",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TokenId = table.Column<string>(type: "text", nullable: false),
                    Token = table.Column<string>(type: "text", nullable: false),
                    ProviderProfileId = table.Column<int>(type: "integer", nullable: false),
                    CreatedTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RemoveProviderToken", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RemoveProviderToken_ProviderProfile_ProviderProfileId",
                        column: x => x.ProviderProfileId,
                        principalTable: "ProviderProfile",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RemoveTokenServiceMapping",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RemoveProviderTokenId = table.Column<int>(type: "integer", nullable: false),
                    ServiceId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RemoveTokenServiceMapping", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RemoveTokenServiceMapping_RemoveProviderToken_RemoveProvide~",
                        column: x => x.RemoveProviderTokenId,
                        principalTable: "RemoveProviderToken",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RemoveTokenServiceMapping_Service_ServiceId",
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
                value: new DateTime(2025, 1, 14, 11, 23, 49, 142, DateTimeKind.Utc).AddTicks(3348));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedTime",
                value: new DateTime(2025, 1, 14, 11, 23, 49, 142, DateTimeKind.Utc).AddTicks(3353));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedTime",
                value: new DateTime(2025, 1, 14, 11, 23, 49, 142, DateTimeKind.Utc).AddTicks(3354));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedTime",
                value: new DateTime(2025, 1, 14, 11, 23, 49, 142, DateTimeKind.Utc).AddTicks(3355));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedTime",
                value: new DateTime(2025, 1, 14, 11, 23, 49, 142, DateTimeKind.Utc).AddTicks(3356));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedTime",
                value: new DateTime(2025, 1, 14, 11, 23, 49, 142, DateTimeKind.Utc).AddTicks(3357));

            migrationBuilder.UpdateData(
                table: "RemovalReasons",
                keyColumn: "RemovalReasonId",
                keyValue: 1,
                columns: new[] { "TimeCreated", "TimeUpdated" },
                values: new object[] { new DateTime(2025, 1, 14, 11, 23, 49, 142, DateTimeKind.Utc).AddTicks(3511), new DateTime(2025, 1, 14, 11, 23, 49, 142, DateTimeKind.Utc).AddTicks(3511) });

            migrationBuilder.UpdateData(
                table: "RemovalReasons",
                keyColumn: "RemovalReasonId",
                keyValue: 3,
                columns: new[] { "TimeCreated", "TimeUpdated" },
                values: new object[] { new DateTime(2025, 1, 14, 11, 23, 49, 142, DateTimeKind.Utc).AddTicks(3514), new DateTime(2025, 1, 14, 11, 23, 49, 142, DateTimeKind.Utc).AddTicks(3514) });

            migrationBuilder.UpdateData(
                table: "RemovalReasons",
                keyColumn: "RemovalReasonId",
                keyValue: 4,
                columns: new[] { "TimeCreated", "TimeUpdated" },
                values: new object[] { new DateTime(2025, 1, 14, 11, 23, 49, 142, DateTimeKind.Utc).AddTicks(3515), new DateTime(2025, 1, 14, 11, 23, 49, 142, DateTimeKind.Utc).AddTicks(3515) });

            migrationBuilder.UpdateData(
                table: "RemovalReasons",
                keyColumn: "RemovalReasonId",
                keyValue: 5,
                columns: new[] { "TimeCreated", "TimeUpdated" },
                values: new object[] { new DateTime(2025, 1, 14, 11, 23, 49, 142, DateTimeKind.Utc).AddTicks(3516), new DateTime(2025, 1, 14, 11, 23, 49, 142, DateTimeKind.Utc).AddTicks(3516) });

            migrationBuilder.UpdateData(
                table: "RemovalReasons",
                keyColumn: "RemovalReasonId",
                keyValue: 6,
                columns: new[] { "TimeCreated", "TimeUpdated" },
                values: new object[] { new DateTime(2025, 1, 14, 11, 23, 49, 142, DateTimeKind.Utc).AddTicks(3517), new DateTime(2025, 1, 14, 11, 23, 49, 142, DateTimeKind.Utc).AddTicks(3517) });

            migrationBuilder.UpdateData(
                table: "RemovalReasons",
                keyColumn: "RemovalReasonId",
                keyValue: 7,
                columns: new[] { "TimeCreated", "TimeUpdated" },
                values: new object[] { new DateTime(2025, 1, 14, 11, 23, 49, 142, DateTimeKind.Utc).AddTicks(3518), new DateTime(2025, 1, 14, 11, 23, 49, 142, DateTimeKind.Utc).AddTicks(3518) });

            migrationBuilder.CreateIndex(
                name: "IX_RemoveProviderToken_ProviderProfileId",
                table: "RemoveProviderToken",
                column: "ProviderProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_RemoveTokenServiceMapping_RemoveProviderTokenId",
                table: "RemoveTokenServiceMapping",
                column: "RemoveProviderTokenId");

            migrationBuilder.CreateIndex(
                name: "IX_RemoveTokenServiceMapping_ServiceId",
                table: "RemoveTokenServiceMapping",
                column: "ServiceId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RemoveTokenServiceMapping");

            migrationBuilder.DropTable(
                name: "RemoveProviderToken");

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedTime",
                value: new DateTime(2025, 1, 9, 11, 16, 20, 850, DateTimeKind.Utc).AddTicks(5984));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedTime",
                value: new DateTime(2025, 1, 9, 11, 16, 20, 850, DateTimeKind.Utc).AddTicks(5988));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedTime",
                value: new DateTime(2025, 1, 9, 11, 16, 20, 850, DateTimeKind.Utc).AddTicks(5989));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedTime",
                value: new DateTime(2025, 1, 9, 11, 16, 20, 850, DateTimeKind.Utc).AddTicks(5990));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedTime",
                value: new DateTime(2025, 1, 9, 11, 16, 20, 850, DateTimeKind.Utc).AddTicks(5991));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedTime",
                value: new DateTime(2025, 1, 9, 11, 16, 20, 850, DateTimeKind.Utc).AddTicks(5992));

            migrationBuilder.UpdateData(
                table: "RemovalReasons",
                keyColumn: "RemovalReasonId",
                keyValue: 1,
                columns: new[] { "TimeCreated", "TimeUpdated" },
                values: new object[] { new DateTime(2025, 1, 9, 11, 16, 20, 850, DateTimeKind.Utc).AddTicks(6203), new DateTime(2025, 1, 9, 11, 16, 20, 850, DateTimeKind.Utc).AddTicks(6203) });

            migrationBuilder.UpdateData(
                table: "RemovalReasons",
                keyColumn: "RemovalReasonId",
                keyValue: 3,
                columns: new[] { "TimeCreated", "TimeUpdated" },
                values: new object[] { new DateTime(2025, 1, 9, 11, 16, 20, 850, DateTimeKind.Utc).AddTicks(6207), new DateTime(2025, 1, 9, 11, 16, 20, 850, DateTimeKind.Utc).AddTicks(6207) });

            migrationBuilder.UpdateData(
                table: "RemovalReasons",
                keyColumn: "RemovalReasonId",
                keyValue: 4,
                columns: new[] { "TimeCreated", "TimeUpdated" },
                values: new object[] { new DateTime(2025, 1, 9, 11, 16, 20, 850, DateTimeKind.Utc).AddTicks(6208), new DateTime(2025, 1, 9, 11, 16, 20, 850, DateTimeKind.Utc).AddTicks(6208) });

            migrationBuilder.UpdateData(
                table: "RemovalReasons",
                keyColumn: "RemovalReasonId",
                keyValue: 5,
                columns: new[] { "TimeCreated", "TimeUpdated" },
                values: new object[] { new DateTime(2025, 1, 9, 11, 16, 20, 850, DateTimeKind.Utc).AddTicks(6209), new DateTime(2025, 1, 9, 11, 16, 20, 850, DateTimeKind.Utc).AddTicks(6209) });

            migrationBuilder.UpdateData(
                table: "RemovalReasons",
                keyColumn: "RemovalReasonId",
                keyValue: 6,
                columns: new[] { "TimeCreated", "TimeUpdated" },
                values: new object[] { new DateTime(2025, 1, 9, 11, 16, 20, 850, DateTimeKind.Utc).AddTicks(6209), new DateTime(2025, 1, 9, 11, 16, 20, 850, DateTimeKind.Utc).AddTicks(6210) });

            migrationBuilder.UpdateData(
                table: "RemovalReasons",
                keyColumn: "RemovalReasonId",
                keyValue: 7,
                columns: new[] { "TimeCreated", "TimeUpdated" },
                values: new object[] { new DateTime(2025, 1, 9, 11, 16, 20, 850, DateTimeKind.Utc).AddTicks(6210), new DateTime(2025, 1, 9, 11, 16, 20, 850, DateTimeKind.Utc).AddTicks(6210) });

            migrationBuilder.InsertData(
                table: "RemovalReasons",
                columns: new[] { "RemovalReasonId", "IsActiveReason", "RemovalReason", "RequiresAdditionalInfo", "TimeCreated", "TimeUpdated" },
                values: new object[] { 2, true, "The Conformity Assessment Body has withdrawn the certificate for the service and there are no other services published for this provider", true, new DateTime(2025, 1, 9, 11, 16, 20, 850, DateTimeKind.Utc).AddTicks(6206), new DateTime(2025, 1, 9, 11, 16, 20, 850, DateTimeKind.Utc).AddTicks(6206) });
        }
    }
}
