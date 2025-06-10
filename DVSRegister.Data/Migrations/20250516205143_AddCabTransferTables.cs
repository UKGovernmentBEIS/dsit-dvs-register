using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DVSRegister.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCabTransferTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "CabUser",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Cab",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "RequestManagement",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    InitiatedUserId = table.Column<int>(type: "integer", nullable: false),
                    CabId = table.Column<int>(type: "integer", nullable: false),
                    RequestType = table.Column<int>(type: "integer", nullable: false),
                    RequestStatus = table.Column<int>(type: "integer", nullable: false),
                    ModifiedTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequestManagement", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RequestManagement_Cab_CabId",
                        column: x => x.CabId,
                        principalTable: "Cab",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RequestManagement_User_InitiatedUserId",
                        column: x => x.InitiatedUserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CabTransferRequest",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ServiceId = table.Column<int>(type: "integer", nullable: false),
                    ProviderProfileId = table.Column<int>(type: "integer", nullable: false),
                    FromCabUserId = table.Column<int>(type: "integer", nullable: false),
                    ToCabId = table.Column<int>(type: "integer", nullable: false),
                    PreviousServiceStatus = table.Column<int>(type: "integer", nullable: false),
                    CertificateUploaded = table.Column<bool>(type: "boolean", nullable: false),
                    RequestManagementId = table.Column<string>(type: "text", nullable: false),
                    DecisionTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CabTransferRequest", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CabTransferRequest_CabUser_FromCabUserId",
                        column: x => x.FromCabUserId,
                        principalTable: "CabUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CabTransferRequest_Cab_ToCabId",
                        column: x => x.ToCabId,
                        principalTable: "Cab",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CabTransferRequest_ProviderProfile_ProviderProfileId",
                        column: x => x.ProviderProfileId,
                        principalTable: "ProviderProfile",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CabTransferRequest_RequestManagement_RequestManagementId",
                        column: x => x.RequestManagementId,
                        principalTable: "RequestManagement",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CabTransferRequest_Service_ServiceId",
                        column: x => x.ServiceId,
                        principalTable: "Service",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 1,
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 2,
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 3,
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 4,
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 6,
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 7,
                column: "IsActive",
                value: true);

            migrationBuilder.CreateIndex(
                name: "IX_CabTransferRequest_FromCabUserId",
                table: "CabTransferRequest",
                column: "FromCabUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CabTransferRequest_ProviderProfileId",
                table: "CabTransferRequest",
                column: "ProviderProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_CabTransferRequest_RequestManagementId",
                table: "CabTransferRequest",
                column: "RequestManagementId");

            migrationBuilder.CreateIndex(
                name: "IX_CabTransferRequest_ServiceId",
                table: "CabTransferRequest",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_CabTransferRequest_ToCabId",
                table: "CabTransferRequest",
                column: "ToCabId");

            migrationBuilder.CreateIndex(
                name: "IX_RequestManagement_CabId",
                table: "RequestManagement",
                column: "CabId");

            migrationBuilder.CreateIndex(
                name: "IX_RequestManagement_InitiatedUserId",
                table: "RequestManagement",
                column: "InitiatedUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CabTransferRequest");

            migrationBuilder.DropTable(
                name: "RequestManagement");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "CabUser");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Cab");
        }
    }
}
