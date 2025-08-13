using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DVSRegister.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePublishStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            

            migrationBuilder.Sql(@"UPDATE ""Service"" SET ""ServiceStatus"" = 4 ,  ""IsInRegister"" = true, ""ModifiedTime"" = CURRENT_TIMESTAMP   WHERE ""ServiceStatus"" = 3; ");

            migrationBuilder.Sql(@"  UPDATE ""ProviderProfile"" SET ""ProviderStatus"" = 3 , ""IsInRegister"" = true , ""ModifiedTime"" = CURRENT_TIMESTAMP WHERE ""ProviderStatus"" IN (2,4); ");


            migrationBuilder.Sql(@"WITH updated_services AS (UPDATE ""Service"" s  SET ""ServiceStatus"" = 4 ,  ""IsInRegister"" = true ,""ModifiedTime"" = CURRENT_TIMESTAMP
            FROM ""PublicInterestCheck"" pic 
            WHERE s.""Id"" = pic.""ServiceId""
            AND s.""ServiceStatus"" = 2
            AND pic. ""PublicInterestCheckStatus"" = 7
            RETURNING s.""ProviderProfileId"")
            UPDATE ""ProviderProfile""  pp  SET ""ProviderStatus"" = 3, ""IsInRegister"" = true
            WHERE pp.""Id"" IN ( SELECT ""ProviderProfileId""    FROM updated_services  )
            AND pp.""ProviderStatus"" IN (1, 5, 10); ");
        }

      
    }
}
