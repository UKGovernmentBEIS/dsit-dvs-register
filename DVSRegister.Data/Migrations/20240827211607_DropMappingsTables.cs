using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DVSRegister.Data.Migrations
{
    /// <inheritdoc />
    public partial class DropMappingsTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CertificateInfoIdentityProfileMapping");

            migrationBuilder.DropTable(
                name: "CertificateInfoSupSchemeMappings");

            migrationBuilder.DropTable(
                name: "CertificateReviewRejectionReasonMappings");

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedTime",
                value: new DateTime(2024, 8, 27, 21, 16, 6, 816, DateTimeKind.Utc).AddTicks(2742));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedTime",
                value: new DateTime(2024, 8, 27, 21, 16, 6, 816, DateTimeKind.Utc).AddTicks(2745));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedTime",
                value: new DateTime(2024, 8, 27, 21, 16, 6, 816, DateTimeKind.Utc).AddTicks(2746));

            migrationBuilder.UpdateData(
                table: "Cab",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedTime",
                value: new DateTime(2024, 8, 27, 21, 16, 6, 816, DateTimeKind.Utc).AddTicks(2747));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CertificateInfoIdentityProfileMapping",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CertificateInformationId = table.Column<int>(type: "integer", nullable: false),
                    IdentityProfileId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CertificateInfoIdentityProfileMapping", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CertificateInfoIdentityProfileMapping_CertificateInformatio~",
                        column: x => x.CertificateInformationId,
                        principalTable: "CertificateInformation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CertificateInfoIdentityProfileMapping_IdentityProfile_Ident~",
                        column: x => x.IdentityProfileId,
                        principalTable: "IdentityProfile",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CertificateInfoSupSchemeMappings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CertificateInformationId = table.Column<int>(type: "integer", nullable: false),
                    SupplementarySchemeId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CertificateInfoSupSchemeMappings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CertificateInfoSupSchemeMappings_CertificateInformation_Cer~",
                        column: x => x.CertificateInformationId,
                        principalTable: "CertificateInformation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CertificateInfoSupSchemeMappings_SupplementaryScheme_Supple~",
                        column: x => x.SupplementarySchemeId,
                        principalTable: "SupplementaryScheme",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CertificateReviewRejectionReasonMappings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CertificateReviewRejectionReasonId = table.Column<int>(type: "integer", nullable: false),
                    CetificateReviewId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CertificateReviewRejectionReasonMappings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CertificateReviewRejectionReasonMappings_CertificateReviewR~",
                        column: x => x.CertificateReviewRejectionReasonId,
                        principalTable: "CertificateReviewRejectionReason",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CertificateReviewRejectionReasonMappings_CertificateReview_~",
                        column: x => x.CetificateReviewId,
                        principalTable: "CertificateReview",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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

            migrationBuilder.CreateIndex(
                name: "IX_CertificateInfoIdentityProfileMapping_CertificateInformatio~",
                table: "CertificateInfoIdentityProfileMapping",
                column: "CertificateInformationId");

            migrationBuilder.CreateIndex(
                name: "IX_CertificateInfoIdentityProfileMapping_IdentityProfileId",
                table: "CertificateInfoIdentityProfileMapping",
                column: "IdentityProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_CertificateInfoSupSchemeMappings_CertificateInformationId",
                table: "CertificateInfoSupSchemeMappings",
                column: "CertificateInformationId");

            migrationBuilder.CreateIndex(
                name: "IX_CertificateInfoSupSchemeMappings_SupplementarySchemeId",
                table: "CertificateInfoSupSchemeMappings",
                column: "SupplementarySchemeId");

            migrationBuilder.CreateIndex(
                name: "IX_CertificateReviewRejectionReasonMappings_CertificateReviewR~",
                table: "CertificateReviewRejectionReasonMappings",
                column: "CertificateReviewRejectionReasonId");

            migrationBuilder.CreateIndex(
                name: "IX_CertificateReviewRejectionReasonMappings_CetificateReviewId",
                table: "CertificateReviewRejectionReasonMappings",
                column: "CetificateReviewId");
        }
    }
}
