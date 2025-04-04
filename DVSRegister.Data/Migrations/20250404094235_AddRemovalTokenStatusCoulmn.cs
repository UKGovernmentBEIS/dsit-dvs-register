using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DVSRegister.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddRemovalTokenStatusCoulmn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RemovalTokenStatus",
                table: "Service",
                type: "integer",
                nullable: false,
                defaultValue: 0);


            migrationBuilder.Sql(@"
            UPDATE ""Service""
            SET ""RemovalTokenStatus"" = CASE 
                WHEN ""ServiceStatus"" = 6 OR ""ServiceStatus"" = 7   THEN 1                
                WHEN ""ServiceStatus"" = 5 THEN 5               
                ELSE 0
            END
        ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RemovalTokenStatus",
                table: "Service");
        }
    }
}
