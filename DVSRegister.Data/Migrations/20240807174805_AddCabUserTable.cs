using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DVSRegister.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCabUserTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CabEmail",
                table: "Service");

            migrationBuilder.DropColumn(
                name: "SubmittedCAB",
                table: "Service");

            migrationBuilder.AddColumn<int>(
                name: "CabUserId",
                table: "Service",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CabUserId",
                table: "ProviderProfile",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "CabUser",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CAB = table.Column<string>(type: "text", nullable: false),
                    UserName = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CabUser", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Service_CabUserId",
                table: "Service",
                column: "CabUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ProviderProfile_CabUserId",
                table: "ProviderProfile",
                column: "CabUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProviderProfile_CabUser_CabUserId",
                table: "ProviderProfile",
                column: "CabUserId",
                principalTable: "CabUser",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Service_CabUser_CabUserId",
                table: "Service",
                column: "CabUserId",
                principalTable: "CabUser",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProviderProfile_CabUser_CabUserId",
                table: "ProviderProfile");

            migrationBuilder.DropForeignKey(
                name: "FK_Service_CabUser_CabUserId",
                table: "Service");

            migrationBuilder.DropTable(
                name: "CabUser");

            migrationBuilder.DropIndex(
                name: "IX_Service_CabUserId",
                table: "Service");

            migrationBuilder.DropIndex(
                name: "IX_ProviderProfile_CabUserId",
                table: "ProviderProfile");

            migrationBuilder.DropColumn(
                name: "CabUserId",
                table: "Service");

            migrationBuilder.DropColumn(
                name: "CabUserId",
                table: "ProviderProfile");

            migrationBuilder.AddColumn<string>(
                name: "CabEmail",
                table: "Service",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SubmittedCAB",
                table: "Service",
                type: "text",
                nullable: true);
        }
    }
}
