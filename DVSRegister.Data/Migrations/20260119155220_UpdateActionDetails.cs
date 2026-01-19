using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DVSRegister.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateActionDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ServiceStatus",
                table: "ActionLogs",
                type: "integer",
                nullable: true);

            migrationBuilder.InsertData(
                table: "ActionCategory",
                columns: new[] { "Id", "ActionKey", "ActionName" },
                values: new object[] { 5, "ActionRequests", "Service removal, reassign" });

            migrationBuilder.UpdateData(
                table: "ActionDetails",
                keyColumn: "Id",
                keyValue: 1,
                column: "ActionDescription",
                value: "Certificate review passed\r\nInvitation email sent");

            migrationBuilder.UpdateData(
                table: "ActionDetails",
                keyColumn: "Id",
                keyValue: 2,
                column: "ActionDescription",
                value: "Certificate review rejected");

            migrationBuilder.UpdateData(
                table: "ActionDetails",
                keyColumn: "Id",
                keyValue: 3,
                column: "ActionDescription",
                value: "Submission restored from rejected certificate review");

            migrationBuilder.UpdateData(
                table: "ActionDetails",
                keyColumn: "Id",
                keyValue: 5,
                column: "ActionDescription",
                value: "Invitation declined by provider");

            migrationBuilder.UpdateData(
                table: "ActionDetails",
                keyColumn: "Id",
                keyValue: 7,
                column: "ActionDescription",
                value: "Application sent back to primary review");

            migrationBuilder.UpdateData(
                table: "ActionDetails",
                keyColumn: "Id",
                keyValue: 9,
                column: "ActionDescription",
                value: "Public interest checks failed");

            migrationBuilder.UpdateData(
                table: "ActionDetails",
                keyColumn: "Id",
                keyValue: 17,
                column: "ActionDescription",
                value: "Invitation to join register cancelled");

            migrationBuilder.UpdateData(
                table: "ActionDetails",
                keyColumn: "Id",
                keyValue: 18,
                column: "ActionDescription",
                value: "Submission sent back from primary review in public interest checks");

            migrationBuilder.UpdateData(
                table: "ActionDetails",
                keyColumn: "Id",
                keyValue: 19,
                column: "ActionDescription",
                value: "Submission sent back from secondary review in public interest checks");

            migrationBuilder.UpdateData(
                table: "ActionDetails",
                keyColumn: "Id",
                keyValue: 20,
                column: "ActionDescription",
                value: "Submission restored from failed public interest checks");

            migrationBuilder.InsertData(
                table: "ActionDetails",
                columns: new[] { "Id", "ActionCategoryId", "ActionDescription", "ActionDetailsKey" },
                values: new object[,]
                {
                    { 21, 1, "Submission received", "CR_Submitted" },
                    { 22, 1, "Invitation accepted", "CR_OpeningLoopAccepted" },
                    { 32, 2, "Published", "PI_Pass" },
                    { 23, 5, "Service removal request sent", "ServiceRemovalRequestSent" },
                    { 24, 5, "Provider removed from register\r\nService removed from register", "ServiceAndProviderRemoved" },
                    { 25, 5, "Service removed from register", "ServiceRemoved" },
                    { 26, 5, "Service removal request cancelled", "ServiceRemovalRequestCancelled" },
                    { 27, 5, "Service removal request declined", "ServiceRemovalRequestDeclined" },
                    { 28, 5, "Reassignment request sent", "ReassignmentRequestSent" },
                    { 29, 5, "Service reassigned", "ServiceReassigned" },
                    { 30, 5, "Reassignment request cancelled", "ReassignRequestCancelled" },
                    { 31, 5, "Reassignment request rejected", "ReassignRequestRejected" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ActionDetails",
                keyColumn: "Id",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "ActionDetails",
                keyColumn: "Id",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "ActionDetails",
                keyColumn: "Id",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "ActionDetails",
                keyColumn: "Id",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "ActionDetails",
                keyColumn: "Id",
                keyValue: 25);

            migrationBuilder.DeleteData(
                table: "ActionDetails",
                keyColumn: "Id",
                keyValue: 26);

            migrationBuilder.DeleteData(
                table: "ActionDetails",
                keyColumn: "Id",
                keyValue: 27);

            migrationBuilder.DeleteData(
                table: "ActionDetails",
                keyColumn: "Id",
                keyValue: 28);

            migrationBuilder.DeleteData(
                table: "ActionDetails",
                keyColumn: "Id",
                keyValue: 29);

            migrationBuilder.DeleteData(
                table: "ActionDetails",
                keyColumn: "Id",
                keyValue: 30);

            migrationBuilder.DeleteData(
                table: "ActionDetails",
                keyColumn: "Id",
                keyValue: 31);

            migrationBuilder.DeleteData(
                table: "ActionDetails",
                keyColumn: "Id",
                keyValue: 32);

            migrationBuilder.DeleteData(
                table: "ActionCategory",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DropColumn(
                name: "ServiceStatus",
                table: "ActionLogs");

            migrationBuilder.UpdateData(
                table: "ActionDetails",
                keyColumn: "Id",
                keyValue: 1,
                column: "ActionDescription",
                value: "Passed");

            migrationBuilder.UpdateData(
                table: "ActionDetails",
                keyColumn: "Id",
                keyValue: 2,
                column: "ActionDescription",
                value: "Rejected");

            migrationBuilder.UpdateData(
                table: "ActionDetails",
                keyColumn: "Id",
                keyValue: 3,
                column: "ActionDescription",
                value: "Restored");

            migrationBuilder.UpdateData(
                table: "ActionDetails",
                keyColumn: "Id",
                keyValue: 5,
                column: "ActionDescription",
                value: "Declined by provider");

            migrationBuilder.UpdateData(
                table: "ActionDetails",
                keyColumn: "Id",
                keyValue: 7,
                column: "ActionDescription",
                value: "Sent back by second reviewer");

            migrationBuilder.UpdateData(
                table: "ActionDetails",
                keyColumn: "Id",
                keyValue: 9,
                column: "ActionDescription",
                value: "Application rejected");

            migrationBuilder.UpdateData(
                table: "ActionDetails",
                keyColumn: "Id",
                keyValue: 17,
                column: "ActionDescription",
                value: "Invitation cancelled");

            migrationBuilder.UpdateData(
                table: "ActionDetails",
                keyColumn: "Id",
                keyValue: 18,
                column: "ActionDescription",
                value: "Send back to certificate review from primary public checks");

            migrationBuilder.UpdateData(
                table: "ActionDetails",
                keyColumn: "Id",
                keyValue: 19,
                column: "ActionDescription",
                value: "Send back to certificate review from  secondary public interest checks");

            migrationBuilder.UpdateData(
                table: "ActionDetails",
                keyColumn: "Id",
                keyValue: 20,
                column: "ActionDescription",
                value: "Restore rejected public interest check");
        }
    }
}
