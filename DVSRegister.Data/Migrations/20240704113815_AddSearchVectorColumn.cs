using Microsoft.EntityFrameworkCore.Migrations;
using NpgsqlTypes;

#nullable disable

namespace DVSRegister.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddSearchVectorColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CertificateReview_CertificateInformationId",
                table: "CertificateReview");

            migrationBuilder.AddColumn<NpgsqlTsVector>(
                name: "SearchVector",
                table: "Provider",
                type: "tsvector",
                nullable: false)
                .Annotation("Npgsql:TsVectorConfig", "english")
                .Annotation("Npgsql:TsVectorProperties", new[] { "RegisteredName", "TradingName" });

            migrationBuilder.CreateIndex(
                name: "IX_Provider_SearchVector",
                table: "Provider",
                column: "SearchVector")
                .Annotation("Npgsql:IndexMethod", "GIN");

            migrationBuilder.CreateIndex(
                name: "IX_CertificateReview_CertificateInformationId",
                table: "CertificateReview",
                column: "CertificateInformationId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Provider_SearchVector",
                table: "Provider");

            migrationBuilder.DropIndex(
                name: "IX_CertificateReview_CertificateInformationId",
                table: "CertificateReview");

            migrationBuilder.DropColumn(
                name: "SearchVector",
                table: "Provider");

            migrationBuilder.CreateIndex(
                name: "IX_CertificateReview_CertificateInformationId",
                table: "CertificateReview",
                column: "CertificateInformationId");
        }
    }
}
