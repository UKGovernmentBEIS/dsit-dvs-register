using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DVSRegister.Data.Migrations
{
    /// <inheritdoc />
    public partial class DropCertificateInfoRoleMapping : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CertificateInfoRoleMapping");

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedTime",
                value: new DateTime(2024, 8, 27, 21, 8, 10, 76, DateTimeKind.Utc).AddTicks(2170));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedTime",
                value: new DateTime(2024, 8, 27, 21, 8, 10, 76, DateTimeKind.Utc).AddTicks(2173));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedTime",
                value: new DateTime(2024, 8, 27, 21, 8, 10, 76, DateTimeKind.Utc).AddTicks(2174));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedTime",
                value: new DateTime(2024, 8, 27, 21, 8, 10, 76, DateTimeKind.Utc).AddTicks(2175));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CertificateInfoRoleMapping",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CertificateInformationId = table.Column<int>(type: "integer", nullable: false),
                    RoleId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CertificateInfoRoleMapping", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CertificateInfoRoleMapping_CertificateInformation_Certifica~",
                        column: x => x.CertificateInformationId,
                        principalTable: "CertificateInformation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CertificateInfoRoleMapping_Role_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Role",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedTime",
                value: new DateTime(2024, 8, 27, 18, 26, 2, 991, DateTimeKind.Utc).AddTicks(4613));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedTime",
                value: new DateTime(2024, 8, 27, 18, 26, 2, 991, DateTimeKind.Utc).AddTicks(4618));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedTime",
                value: new DateTime(2024, 8, 27, 18, 26, 2, 991, DateTimeKind.Utc).AddTicks(4619));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedTime",
                value: new DateTime(2024, 8, 27, 18, 26, 2, 991, DateTimeKind.Utc).AddTicks(4621));

            migrationBuilder.CreateIndex(
                name: "IX_CertificateInfoRoleMapping_CertificateInformationId",
                table: "CertificateInfoRoleMapping",
                column: "CertificateInformationId");

            migrationBuilder.CreateIndex(
                name: "IX_CertificateInfoRoleMapping_RoleId",
                table: "CertificateInfoRoleMapping",
                column: "RoleId");
        }
    }
}
