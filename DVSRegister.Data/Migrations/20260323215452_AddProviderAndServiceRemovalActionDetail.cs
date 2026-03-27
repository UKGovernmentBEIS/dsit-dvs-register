using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DVSRegister.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddProviderAndServiceRemovalActionDetail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "ActionDetails",
                columns: new[] { "Id", "ActionCategoryId", "ActionDescription", "ActionDetailsKey" },
                values: new object[] { 32, 5, "Service removed from register\nProvider removed from register", "ProviderAndServiceRemoved" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ActionDetails",
                keyColumn: "Id",
                keyValue: 32);
        }
    }
}
