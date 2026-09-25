using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DVSRegister.Data.Migrations
{
    /// <inheritdoc />
    public partial class BackfillProviderRemovedTimeFromRemovalRequests : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.Sql("""
                UPDATE "ProviderProfile" AS p
                SET "RemovedTime" = src."RemovedTime"
                FROM (
                  SELECT "ProviderProfileId", MAX("RemovedTime") AS "RemovedTime"
                  FROM "ProviderRemovalRequest"
                  WHERE "RemovedTime" IS NOT NULL
                  GROUP BY "ProviderProfileId"
                ) AS src
                WHERE p."Id" = src."ProviderProfileId"
                  AND p."RemovedTime" IS NULL AND p."IsInRegister" = false;
                """);
        }
        

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
