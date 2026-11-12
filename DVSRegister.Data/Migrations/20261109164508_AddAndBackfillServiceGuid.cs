using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DVSRegister.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddAndBackfillServiceGuid : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ServiceGuid",
                table: "Service",
                type: "uuid",
                nullable: true);

            migrationBuilder.Sql(@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM ""Service""
                        WHERE ""ServiceKey"" IS NULL OR ""ServiceKey"" <= 0
                    ) THEN
                        RAISE EXCEPTION 'ServiceGuid backfill stopped: a Service row has a NULL or non-positive ServiceKey.';
                    END IF;

                    IF EXISTS (
                        SELECT 1 FROM ""Service""
                        GROUP BY ""ServiceKey""
                        HAVING COUNT(DISTINCT ""ProviderProfileId"") > 1
                    ) THEN
                        RAISE EXCEPTION 'ServiceGuid backfill stopped: a ServiceKey belongs to more than one provider.';
                    END IF;

                    IF EXISTS (
                        SELECT 1 FROM ""Service""
                        GROUP BY ""ServiceKey"", ""ServiceVersion""
                        HAVING COUNT(*) > 1
                    ) THEN
                        RAISE EXCEPTION 'ServiceGuid backfill stopped: a ServiceKey has duplicate ServiceVersion values.';
                    END IF;
                END $$;");

            migrationBuilder.Sql(
                "WITH key_guids AS (" +
                "SELECT \"ServiceKey\", gen_random_uuid() AS guid FROM \"Service\" GROUP BY \"ServiceKey\") " +
                "UPDATE \"Service\" s SET \"ServiceGuid\" = k.guid " +
                "FROM key_guids k " +
                "WHERE s.\"ServiceKey\" = k.\"ServiceKey\" AND s.\"ServiceGuid\" IS NULL;");

            migrationBuilder.AlterColumn<Guid>(
                name: "ServiceGuid",
                table: "Service",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Service_ServiceGuid_ServiceVersion",
                table: "Service",
                columns: new[] { "ServiceGuid", "ServiceVersion" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Service_ServiceGuid_ServiceVersion",
                table: "Service");

            migrationBuilder.DropColumn(
                name: "ServiceGuid",
                table: "Service");
        }
    }
}
