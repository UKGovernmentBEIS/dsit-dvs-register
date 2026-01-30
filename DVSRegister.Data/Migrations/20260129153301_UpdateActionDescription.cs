using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DVSRegister.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateActionDescription : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "ActionDetails",
                keyColumn: "Id",
                keyValue: 1,
                column: "ActionDescription",
                value: "Certificate review passed\nInvitation email sent");

            migrationBuilder.UpdateData(
                table: "ActionDetails",
                keyColumn: "Id",
                keyValue: 3,
                column: "ActionDescription",
                value: "Submission back to certificate review\nSubmission restored from rejected certificate review");

            migrationBuilder.UpdateData(
                table: "ActionDetails",
                keyColumn: "Id",
                keyValue: 5,
                column: "ActionDescription",
                value: "Submission back to certificate review\nInvitation declined by provider");

            migrationBuilder.UpdateData(
                table: "ActionDetails",
                keyColumn: "Id",
                keyValue: 17,
                column: "ActionDescription",
                value: "Submission back to certificate review\nInvitation to join register cancelled");

            migrationBuilder.UpdateData(
                table: "ActionDetails",
                keyColumn: "Id",
                keyValue: 18,
                column: "ActionDescription",
                value: "Submission back to certificate review\nSubmission sent back from primary review in public interest checks");

            migrationBuilder.UpdateData(
                table: "ActionDetails",
                keyColumn: "Id",
                keyValue: 19,
                column: "ActionDescription",
                value: "Submission back to certificate review\nSubmission sent back from secondary review in public interest checks");

            migrationBuilder.UpdateData(
                table: "ActionDetails",
                keyColumn: "Id",
                keyValue: 20,
                column: "ActionDescription",
                value: "Submission back to certificate review\nSubmission restored from failed public interest checks");

            migrationBuilder.UpdateData(
                table: "ActionDetails",
                keyColumn: "Id",
                keyValue: 24,
                column: "ActionDescription",
                value: "Provider removed from register\nService removed from register");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "ActionDetails",
                keyColumn: "Id",
                keyValue: 1,
                column: "ActionDescription",
                value: "Certificate review passed\r\nInvitation email sent");

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

            migrationBuilder.UpdateData(
                table: "ActionDetails",
                keyColumn: "Id",
                keyValue: 24,
                column: "ActionDescription",
                value: "Provider removed from register\r\nService removed from register");
        }
    }
}
