using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DVSRegister.Data.Migrations
{
    /// <inheritdoc />
    public partial class URNUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_UniqueReferenceNumbers",
                table: "UniqueReferenceNumbers");

            migrationBuilder.RenameTable(
                name: "UniqueReferenceNumbers",
                newName: "UniqueReferenceNumber");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UniqueReferenceNumber",
                table: "UniqueReferenceNumber",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_UniqueReferenceNumber",
                table: "UniqueReferenceNumber");

            migrationBuilder.RenameTable(
                name: "UniqueReferenceNumber",
                newName: "UniqueReferenceNumbers");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UniqueReferenceNumbers",
                table: "UniqueReferenceNumbers",
                column: "Id");
        }
    }
}
