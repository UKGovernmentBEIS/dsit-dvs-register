using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DVSRegister.Data.Migrations
{
    /// <inheritdoc />
    public partial class InsertReviewIdsToActionLogs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
             INSERT INTO ""ActionLogs"" (
                ""ActionCategoryId"",
                ""ActionDetailsId"",
                ""ProviderProfileId"",
                ""ServiceId"",
                ""CertificateReviewId"",
                ""DisplayMessage"",
                ""UpdatedByUserId"",
                ""LogDate"",
                ""LoggedTime"",
                ""ShowInRegisterUpdates""
            )
                SELECT
                    1 AS ""ActionCategoryId"",
                    CASE 
                        WHEN cr.""CertificateReviewStatus"" = 2 THEN 1 
                        WHEN cr.""CertificateReviewStatus"" = 3 THEN 2 
                        WHEN cr.""CertificateReviewStatus"" = 5 THEN 4
                    END AS ""ActionDetailsId"",
                    cr.""ProviProviderProfileId"",
                    cr.""ServiceId"",
                    cr.""Id"" AS ""CertificateReviewId"",
                    CASE 
                        WHEN cr.""CertificateReviewStatus"" = 2 THEN 'Certificate review passed'
                        WHEN cr.""CertificateReviewStatus"" = 3 THEN 'Certificate review failed'
                        WHEN cr.""CertificateReviewStatus"" = 5 THEN 'Sent back to CAB'
                    END AS ""DisplayMessage"",
                    cr.""VerifiedUser"" AS ""UpdatedByUserId"",
                    cr.""ModifiedDate""::DATE AS ""LogDate"",
                    cr.""ModifiedDate"" AS ""LoggedTime"",
                    FALSE AS ""ShowInRegisterUpdates""
                FROM ""CertificateReview"" AS cr
                WHERE cr.""ServiceId"" IS NOT NULL
                AND cr.""ProviProviderProfileId"" IS NOT NULL
                AND cr.""CertificateReviewStatus"" IN (2, 3,5);
                ");

            migrationBuilder.Sql(@"
            INSERT INTO ""ActionLogs"" (
                ""ActionCategoryId"",
                ""ActionDetailsId"",
                ""ProviderProfileId"",
                ""ServiceId"",
                ""PublicInterestCheckId"",
                ""DisplayMessage"",
                ""UpdatedByUserId"",
                ""LogDate"",
                ""LoggedTime"",
                ""ShowInRegisterUpdates""
            )
            SELECT
                2 AS ""ActionCategoryId"", 
                CASE 
                    WHEN pic.""PublicInterestCheckStatus"" = 6 THEN 9  
                    WHEN pic.""PublicInterestCheckStatus"" = 7 THEN 11  
                END AS ""ActionDetailsId"",
                pic.""ProviderProfileId"",
                pic.""ServiceId"",
                pic.""Id"" AS ""PublicInterestCheckId"",
                CASE 
                    WHEN pic.""PublicInterestCheckStatus"" = 6 THEN 'PI checks failed'
                    WHEN pic.""PublicInterestCheckStatus"" = 7 THEN 'Published'
                END AS ""DisplayMessage"",
                pic.""SecondaryCheckUserId"" AS ""UpdatedByUserId"",  
                pic.""SecondaryCheckTime""::DATE AS ""LogDate"",      
                pic.""SecondaryCheckTime"" AS ""LoggedTime"",           
                FALSE AS ""ShowInRegisterUpdates""
            FROM ""PublicInterestCheck"" AS pic
            WHERE pic.""ServiceId"" IS NOT NULL
            AND pic.""ProviderProfileId"" IS NOT NULL
            AND pic.""PublicInterestCheckStatus"" IN (6, 7);
        ");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
