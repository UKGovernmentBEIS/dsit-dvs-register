using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DVSRegister.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRegisterPublishLogTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RegisterPublishLog_Provider_ProviderId",
                table: "RegisterPublishLog");

            migrationBuilder.RenameColumn(
                name: "ProviderId",
                table: "RegisterPublishLog",
                newName: "ProviderProfileId");

            migrationBuilder.RenameIndex(
                name: "IX_RegisterPublishLog_ProviderId",
                table: "RegisterPublishLog",
                newName: "IX_RegisterPublishLog_ProviderProfileId");

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedTime",
                value: new DateTime(2024, 9, 9, 9, 52, 7, 272, DateTimeKind.Utc).AddTicks(8741));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedTime",
                value: new DateTime(2024, 9, 9, 9, 52, 7, 272, DateTimeKind.Utc).AddTicks(8744));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedTime",
                value: new DateTime(2024, 9, 9, 9, 52, 7, 272, DateTimeKind.Utc).AddTicks(8746));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedTime",
                value: new DateTime(2024, 9, 9, 9, 52, 7, 272, DateTimeKind.Utc).AddTicks(8747));

            migrationBuilder.AddForeignKey(
                name: "FK_RegisterPublishLog_ProviderProfile_ProviderProfileId",
                table: "RegisterPublishLog",
                column: "ProviderProfileId",
                principalTable: "ProviderProfile",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RegisterPublishLog_ProviderProfile_ProviderProfileId",
                table: "RegisterPublishLog");

            migrationBuilder.RenameColumn(
                name: "ProviderProfileId",
                table: "RegisterPublishLog",
                newName: "ProviderId");

            migrationBuilder.RenameIndex(
                name: "IX_RegisterPublishLog_ProviderProfileId",
                table: "RegisterPublishLog",
                newName: "IX_RegisterPublishLog_ProviderId");

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

            migrationBuilder.AddForeignKey(
                name: "FK_RegisterPublishLog_Provider_ProviderId",
                table: "RegisterPublishLog",
                column: "ProviderId",
                principalTable: "Provider",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
