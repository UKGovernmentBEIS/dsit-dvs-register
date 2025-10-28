using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DVSRegister.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateReviewVersionColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE \"CertificateReview\" SET \"IsLatestReviewVersion\" = true , \"ReviewVersion\" = 1");
            migrationBuilder.Sql("UPDATE \"PublicInterestCheck\" SET \"IsLatestReviewVersion\" = true , \"ReviewVersion\" = 1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
