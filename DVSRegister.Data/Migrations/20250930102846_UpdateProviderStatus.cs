using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DVSRegister.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateProviderStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE \"ProviderProfile\" SET \"ProviderStatus\" = 1 WHERE \"ProviderStatus\" != 1 AND \"ProviderStatus\" != 6 AND \"ProviderStatus\" != 8 ;");
            migrationBuilder.Sql("UPDATE \"ProviderProfileDraft\" SET \"PreviousProviderStatus\" = 1 WHERE \"PreviousProviderStatus\" != 1 AND \"PreviousProviderStatus\" != 6 AND \"PreviousProviderStatus\" != 8 ;");
            migrationBuilder.Sql("UPDATE \"ProviderRemovalRequest\" SET \"PreviousProviderStatus\" = 1 WHERE \"PreviousProviderStatus\" != 1 AND \"PreviousProviderStatus\" != 6 AND \"PreviousProviderStatus\" != 8 ;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
