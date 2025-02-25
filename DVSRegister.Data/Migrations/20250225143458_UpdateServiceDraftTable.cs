using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DVSRegister.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateServiceDraftTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProviderProfileId",
                table: "ServiceDraft",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ServiceDraft_ProviderProfileId",
                table: "ServiceDraft",
                column: "ProviderProfileId");

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceDraft_ProviderProfile_ProviderProfileId",
                table: "ServiceDraft",
                column: "ProviderProfileId",
                principalTable: "ProviderProfile",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ServiceDraft_ProviderProfile_ProviderProfileId",
                table: "ServiceDraft");

            migrationBuilder.DropIndex(
                name: "IX_ServiceDraft_ProviderProfileId",
                table: "ServiceDraft");

            migrationBuilder.DropColumn(
                name: "ProviderProfileId",
                table: "ServiceDraft");
        }
    }
}
