using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DVSRegister.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCertificateReviewField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CertificateReview_ServiceId",
                table: "CertificateReview");

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedTime",
                value: new DateTime(2024, 8, 29, 11, 18, 59, 112, DateTimeKind.Utc).AddTicks(1775));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedTime",
                value: new DateTime(2024, 8, 29, 11, 18, 59, 112, DateTimeKind.Utc).AddTicks(1778));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedTime",
                value: new DateTime(2024, 8, 29, 11, 18, 59, 112, DateTimeKind.Utc).AddTicks(1780));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedTime",
                value: new DateTime(2024, 8, 29, 11, 18, 59, 112, DateTimeKind.Utc).AddTicks(1782));

            migrationBuilder.CreateIndex(
                name: "IX_CertificateReview_ServiceId",
                table: "CertificateReview",
                column: "ServiceId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CertificateReview_ServiceId",
                table: "CertificateReview");

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedTime",
                value: new DateTime(2024, 8, 27, 21, 33, 53, 172, DateTimeKind.Utc).AddTicks(4848));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedTime",
                value: new DateTime(2024, 8, 27, 21, 33, 53, 172, DateTimeKind.Utc).AddTicks(4852));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedTime",
                value: new DateTime(2024, 8, 27, 21, 33, 53, 172, DateTimeKind.Utc).AddTicks(4853));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedTime",
                value: new DateTime(2024, 8, 27, 21, 33, 53, 172, DateTimeKind.Utc).AddTicks(4855));

            migrationBuilder.CreateIndex(
                name: "IX_CertificateReview_ServiceId",
                table: "CertificateReview",
                column: "ServiceId");
        }
    }
}
