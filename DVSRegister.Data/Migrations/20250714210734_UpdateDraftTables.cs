using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DVSRegister.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDraftTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ServiceDraft_ServiceId",
                table: "ServiceDraft");

            migrationBuilder.AddColumn<bool>(
                name: "HasGpg44Mapping",
                table: "ServiceSupSchemeMappingDraft",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsUnderpinningServicePublished",
                table: "ServiceDraft",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ManualUnderPinningServiceDraftId",
                table: "ServiceDraft",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ManualUnderPinningServiceId",
                table: "ServiceDraft",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UnderPinningServiceId",
                table: "ServiceDraft",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ManualUnderPinningServiceDraft",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ServiceName = table.Column<string>(type: "text", nullable: true),
                    ProviderName = table.Column<string>(type: "text", nullable: true),
                    CabId = table.Column<int>(type: "integer", nullable: true),
                    CertificateExpiryDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ManualUnderPinningServiceDraft", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ManualUnderPinningServiceDraft_Cab_CabId",
                        column: x => x.CabId,
                        principalTable: "Cab",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SchemeGPG44MappingDraft",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    QualityLevelId = table.Column<int>(type: "integer", nullable: false),
                    ServiceSupSchemeMappingDraftId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SchemeGPG44MappingDraft", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SchemeGPG44MappingDraft_QualityLevel_QualityLevelId",
                        column: x => x.QualityLevelId,
                        principalTable: "QualityLevel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SchemeGPG44MappingDraft_ServiceSupSchemeMappingDraft_Servic~",
                        column: x => x.ServiceSupSchemeMappingDraftId,
                        principalTable: "ServiceSupSchemeMappingDraft",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SchemeGPG45MappingDraft",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdentityProfileId = table.Column<int>(type: "integer", nullable: false),
                    ServiceSupSchemeMappingDraftId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SchemeGPG45MappingDraft", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SchemeGPG45MappingDraft_IdentityProfile_IdentityProfileId",
                        column: x => x.IdentityProfileId,
                        principalTable: "IdentityProfile",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SchemeGPG45MappingDraft_ServiceSupSchemeMappingDraft_Servic~",
                        column: x => x.ServiceSupSchemeMappingDraftId,
                        principalTable: "ServiceSupSchemeMappingDraft",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ServiceDraft_ManualUnderPinningServiceDraftId",
                table: "ServiceDraft",
                column: "ManualUnderPinningServiceDraftId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceDraft_ManualUnderPinningServiceId",
                table: "ServiceDraft",
                column: "ManualUnderPinningServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceDraft_ServiceId",
                table: "ServiceDraft",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceDraft_UnderPinningServiceId",
                table: "ServiceDraft",
                column: "UnderPinningServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_ManualUnderPinningServiceDraft_CabId",
                table: "ManualUnderPinningServiceDraft",
                column: "CabId");

            migrationBuilder.CreateIndex(
                name: "IX_SchemeGPG44MappingDraft_QualityLevelId",
                table: "SchemeGPG44MappingDraft",
                column: "QualityLevelId");

            migrationBuilder.CreateIndex(
                name: "IX_SchemeGPG44MappingDraft_ServiceSupSchemeMappingDraftId",
                table: "SchemeGPG44MappingDraft",
                column: "ServiceSupSchemeMappingDraftId");

            migrationBuilder.CreateIndex(
                name: "IX_SchemeGPG45MappingDraft_IdentityProfileId",
                table: "SchemeGPG45MappingDraft",
                column: "IdentityProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_SchemeGPG45MappingDraft_ServiceSupSchemeMappingDraftId",
                table: "SchemeGPG45MappingDraft",
                column: "ServiceSupSchemeMappingDraftId");

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceDraft_ManualUnderPinningServiceDraft_ManualUnderPinn~",
                table: "ServiceDraft",
                column: "ManualUnderPinningServiceDraftId",
                principalTable: "ManualUnderPinningServiceDraft",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceDraft_ManualUnderPinningService_ManualUnderPinningSe~",
                table: "ServiceDraft",
                column: "ManualUnderPinningServiceId",
                principalTable: "ManualUnderPinningService",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceDraft_Service_UnderPinningServiceId",
                table: "ServiceDraft",
                column: "UnderPinningServiceId",
                principalTable: "Service",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ServiceDraft_ManualUnderPinningServiceDraft_ManualUnderPinn~",
                table: "ServiceDraft");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceDraft_ManualUnderPinningService_ManualUnderPinningSe~",
                table: "ServiceDraft");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceDraft_Service_UnderPinningServiceId",
                table: "ServiceDraft");

            migrationBuilder.DropTable(
                name: "ManualUnderPinningServiceDraft");

            migrationBuilder.DropTable(
                name: "SchemeGPG44MappingDraft");

            migrationBuilder.DropTable(
                name: "SchemeGPG45MappingDraft");

            migrationBuilder.DropIndex(
                name: "IX_ServiceDraft_ManualUnderPinningServiceDraftId",
                table: "ServiceDraft");

            migrationBuilder.DropIndex(
                name: "IX_ServiceDraft_ManualUnderPinningServiceId",
                table: "ServiceDraft");

            migrationBuilder.DropIndex(
                name: "IX_ServiceDraft_ServiceId",
                table: "ServiceDraft");

            migrationBuilder.DropIndex(
                name: "IX_ServiceDraft_UnderPinningServiceId",
                table: "ServiceDraft");

            migrationBuilder.DropColumn(
                name: "HasGpg44Mapping",
                table: "ServiceSupSchemeMappingDraft");

            migrationBuilder.DropColumn(
                name: "IsUnderpinningServicePublished",
                table: "ServiceDraft");

            migrationBuilder.DropColumn(
                name: "ManualUnderPinningServiceDraftId",
                table: "ServiceDraft");

            migrationBuilder.DropColumn(
                name: "ManualUnderPinningServiceId",
                table: "ServiceDraft");

            migrationBuilder.DropColumn(
                name: "UnderPinningServiceId",
                table: "ServiceDraft");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceDraft_ServiceId",
                table: "ServiceDraft",
                column: "ServiceId",
                unique: true);
        }
    }
}
