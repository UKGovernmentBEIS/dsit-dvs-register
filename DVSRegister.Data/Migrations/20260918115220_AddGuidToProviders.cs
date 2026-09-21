using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DVSRegister.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddGuidToProviders : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "Guid",
                table: "ProviderProfile",
                type: "uuid",
                nullable: true);

            migrationBuilder.Sql(
                "UPDATE \"ProviderProfile\" SET \"Guid\" = gen_random_uuid() WHERE \"Guid\" IS NULL;");

            migrationBuilder.AlterColumn<Guid>(
                name: "Guid",
                table: "ProviderProfile",
                type: "uuid",
                nullable: false,
                defaultValueSql: "gen_random_uuid()",
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProviderProfile_Guid",
                table: "ProviderProfile",
                column: "Guid",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ProviderProfile_Guid",
                table: "ProviderProfile");

            migrationBuilder.DropColumn(
                name: "Guid",
                table: "ProviderProfile");
        }
    }
}
