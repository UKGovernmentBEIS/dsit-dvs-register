using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DVSRegister.Data.Migrations
{
    /// <inheritdoc />
    public partial class CreatePreAssessmentTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PreAssessment",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RegisteredCompanyName = table.Column<string>(type: "varchar(160)", nullable: false),
                    CompanyRegistrationNumber = table.Column<string>(type: "varchar(8)", nullable: false),
                    DIPForeignJurisdictionID = table.Column<string>(type: "text", nullable: false),
                    SROFullName = table.Column<string>(type: "text", nullable: false),
                    SRORole = table.Column<string>(type: "text", nullable: false),
                    SROEmail = table.Column<string>(type: "varchar(254)", nullable: false),
                    SROTelephoneNumber = table.Column<string>(type: "text", nullable: true),
                    AltContactRole = table.Column<string>(type: "text", nullable: true),
                    AltContactEmail = table.Column<string>(type: "varchar(254)", nullable: true),
                    AltContactTelephoneNumber = table.Column<string>(type: "text", nullable: true),
                    GeographicalAreas = table.Column<string>(type: "text", nullable: false),
                    IDSP = table.Column<bool>(type: "boolean", nullable: true),
                    ASP = table.Column<bool>(type: "boolean", nullable: true),
                    OSP = table.Column<bool>(type: "boolean", nullable: true),
                    WalletProvider = table.Column<bool>(type: "boolean", nullable: true),
                    Other = table.Column<bool>(type: "boolean", nullable: true),
                    OtherRoleDescription = table.Column<string>(type: "text", nullable: true),
                    ConfirmLegalRequirements = table.Column<int>(type: "integer", nullable: false),
                    ConfirmAccuracy = table.Column<int>(type: "integer", nullable: false),
                    URN = table.Column<string>(type: "text", nullable: false),
                    PreAssessmentStatus = table.Column<int>(type: "integer", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedBy = table.Column<string>(type: "text", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PreAssessment", x => x.ID);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PreAssessment");
        }
    }
}
