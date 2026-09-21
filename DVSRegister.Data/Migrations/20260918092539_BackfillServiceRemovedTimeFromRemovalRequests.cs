using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DVSRegister.Data.Migrations
{
    /// <inheritdoc />
    public partial class BackfillServiceRemovedTimeFromRemovalRequests : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                UPDATE "Service" AS s
                SET "RemovedTime" = src."RemovedTime"
                FROM (
                    SELECT "ServiceId", MAX("RemovedTime") AS "RemovedTime"
                    FROM "ServiceRemovalRequest"
                    WHERE "RemovedTime" IS NOT NULL
                    GROUP BY "ServiceId"
                ) AS src
                WHERE s."Id" = src."ServiceId"
                  AND s."RemovedTime" IS NULL;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // This data migration cannot be safely reversed.
        }
    }
}
