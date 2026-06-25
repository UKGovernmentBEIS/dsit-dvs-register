using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DVSRegister.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCabAndCabUserActiveStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
             UPDATE ""CabUser""
             SET ""AccountStatus"" = 2 WHERE ""IsActive"" = true ");


             migrationBuilder.Sql(@"
             UPDATE ""Cab""
             SET ""IsActive"" = false  WHERE ""CabName"" IN  ('ACCS','NQA') ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            
        }
    }
}
