using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DVSRegister.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddModifyAccountManagementTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "User");

            migrationBuilder.RenameColumn(
                name: "ModifiedBy",
                table: "User",
                newName: "FullName");

            migrationBuilder.AlterColumn<string>(
                name: "UserName",
                table: "User",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<int>(
                name: "AccountStatus",
                table: "User",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "User",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastLoggedIn",
                table: "User",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UserRole",
                table: "User",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "AccountStatus",
                table: "CabUser",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "CabUser",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastLoggedIn",
                table: "CabUser",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedDate",
                table: "CabUser",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserName",
                table: "CabUser",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CabUserRemoval",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CabUserUserId = table.Column<int>(type: "integer", nullable: false),
                    RemovalReason = table.Column<int>(type: "integer", nullable: true),
                    Comment = table.Column<string>(type: "text", nullable: true),
                    IsRemoved = table.Column<bool>(type: "boolean", nullable: false),
                    RemovalTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CabUserRemoval", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CabUserRemoval_CabUser_CabUserUserId",
                        column: x => x.CabUserUserId,
                        principalTable: "CabUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OfDIAUserRemoval",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    RemovalReason = table.Column<int>(type: "integer", nullable: true),
                    Comment = table.Column<string>(type: "text", nullable: true),
                    IsRemoved = table.Column<bool>(type: "boolean", nullable: false),
                    RemovedTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OfDIAUserRemoval", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OfDIAUserRemoval_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CabUserRemoval_CabUserUserId",
                table: "CabUserRemoval",
                column: "CabUserUserId");

            migrationBuilder.CreateIndex(
                name: "IX_OfDIAUserRemoval_UserId",
                table: "OfDIAUserRemoval",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CabUserRemoval");

            migrationBuilder.DropTable(
                name: "OfDIAUserRemoval");

            migrationBuilder.DropColumn(
                name: "AccountStatus",
                table: "User");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "User");

            migrationBuilder.DropColumn(
                name: "LastLoggedIn",
                table: "User");

            migrationBuilder.DropColumn(
                name: "UserRole",
                table: "User");

            migrationBuilder.DropColumn(
                name: "AccountStatus",
                table: "CabUser");

            migrationBuilder.DropColumn(
                name: "FullName",
                table: "CabUser");

            migrationBuilder.DropColumn(
                name: "LastLoggedIn",
                table: "CabUser");

            migrationBuilder.DropColumn(
                name: "ModifiedDate",
                table: "CabUser");

            migrationBuilder.DropColumn(
                name: "UserName",
                table: "CabUser");

            migrationBuilder.RenameColumn(
                name: "FullName",
                table: "User",
                newName: "ModifiedBy");

            migrationBuilder.AlterColumn<string>(
                name: "UserName",
                table: "User",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "User",
                type: "text",
                nullable: true);
        }
    }
}
