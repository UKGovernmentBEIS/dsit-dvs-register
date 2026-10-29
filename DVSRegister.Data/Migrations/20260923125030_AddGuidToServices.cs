using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DVSRegister.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddGuidToServices : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "Guid",
                table: "Service",
                type: "uuid",
                nullable: true);

            migrationBuilder.Sql(
                "UPDATE \"Service\" SET \"Guid\" = gen_random_uuid() WHERE \"Guid\" IS NULL;");

            migrationBuilder.AlterColumn<Guid>(
                name: "Guid",
                table: "Service",
                type: "uuid",
                nullable: false,
                defaultValueSql: "gen_random_uuid()",
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Service_Guid",
                table: "Service",
                column: "Guid",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Service_Guid",
                table: "Service");

            migrationBuilder.DropColumn(
                name: "Guid",
                table: "Service");
        }
    }
}
