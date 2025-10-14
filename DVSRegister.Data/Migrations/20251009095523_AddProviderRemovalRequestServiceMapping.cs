using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DVSRegister.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddProviderRemovalRequestServiceMapping : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProviderRemovalRequestServiceMapping",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProviderRemovalRequestId = table.Column<int>(type: "integer", nullable: false),
                    ServiceId = table.Column<int>(type: "integer", nullable: false),
                    PreviousServiceStatus = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProviderRemovalRequestServiceMapping", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProviderRemovalRequestServiceMapping_ProviderRemovalRequest~",
                        column: x => x.ProviderRemovalRequestId,
                        principalTable: "ProviderRemovalRequest",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProviderRemovalRequestServiceMapping_Service_ServiceId",
                        column: x => x.ServiceId,
                        principalTable: "Service",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProviderRemovalRequestServiceMapping_ProviderRemovalRequest~",
                table: "ProviderRemovalRequestServiceMapping",
                column: "ProviderRemovalRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_ProviderRemovalRequestServiceMapping_ServiceId",
                table: "ProviderRemovalRequestServiceMapping",
                column: "ServiceId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProviderRemovalRequestServiceMapping");
        }
    }
}
