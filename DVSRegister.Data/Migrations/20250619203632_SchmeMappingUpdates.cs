using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DVSRegister.Data.Migrations
{
    /// <inheritdoc />
    public partial class SchmeMappingUpdates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SchemeGPG44Mapping_Service_ServiceId",
                table: "SchemeGPG44Mapping");

            migrationBuilder.DropForeignKey(
                name: "FK_SchemeGPG44Mapping_SupplementaryScheme_SupplementarySchemeId",
                table: "SchemeGPG44Mapping");

            migrationBuilder.DropForeignKey(
                name: "FK_SchemeGPG45Mapping_Service_ServiceId",
                table: "SchemeGPG45Mapping");

            migrationBuilder.DropForeignKey(
                name: "FK_SchemeGPG45Mapping_SupplementaryScheme_SupplementarySchemeId",
                table: "SchemeGPG45Mapping");

            migrationBuilder.RenameColumn(
                name: "ServiceId",
                table: "SchemeGPG45Mapping",
                newName: "ServiceSupSchemeMappingId");

            migrationBuilder.RenameIndex(
                name: "IX_SchemeGPG45Mapping_ServiceId",
                table: "SchemeGPG45Mapping",
                newName: "IX_SchemeGPG45Mapping_ServiceSupSchemeMappingId");

            migrationBuilder.RenameColumn(
                name: "ServiceId",
                table: "SchemeGPG44Mapping",
                newName: "ServiceSupSchemeMappingId");

            migrationBuilder.RenameIndex(
                name: "IX_SchemeGPG44Mapping_ServiceId",
                table: "SchemeGPG44Mapping",
                newName: "IX_SchemeGPG44Mapping_ServiceSupSchemeMappingId");

            migrationBuilder.AddColumn<bool>(
                name: "HasGpg44Mapping",
                table: "ServiceSupSchemeMapping",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsUnderPinningServicePublished",
                table: "Service",
                type: "boolean",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "SupplementarySchemeId",
                table: "SchemeGPG45Mapping",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "SupplementarySchemeId",
                table: "SchemeGPG44Mapping",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "FK_SchemeGPG44Mapping_ServiceSupSchemeMapping_ServiceSupScheme~",
                table: "SchemeGPG44Mapping",
                column: "ServiceSupSchemeMappingId",
                principalTable: "ServiceSupSchemeMapping",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SchemeGPG44Mapping_SupplementaryScheme_SupplementarySchemeId",
                table: "SchemeGPG44Mapping",
                column: "SupplementarySchemeId",
                principalTable: "SupplementaryScheme",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SchemeGPG45Mapping_ServiceSupSchemeMapping_ServiceSupScheme~",
                table: "SchemeGPG45Mapping",
                column: "ServiceSupSchemeMappingId",
                principalTable: "ServiceSupSchemeMapping",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SchemeGPG45Mapping_SupplementaryScheme_SupplementarySchemeId",
                table: "SchemeGPG45Mapping",
                column: "SupplementarySchemeId",
                principalTable: "SupplementaryScheme",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SchemeGPG44Mapping_ServiceSupSchemeMapping_ServiceSupScheme~",
                table: "SchemeGPG44Mapping");

            migrationBuilder.DropForeignKey(
                name: "FK_SchemeGPG44Mapping_SupplementaryScheme_SupplementarySchemeId",
                table: "SchemeGPG44Mapping");

            migrationBuilder.DropForeignKey(
                name: "FK_SchemeGPG45Mapping_ServiceSupSchemeMapping_ServiceSupScheme~",
                table: "SchemeGPG45Mapping");

            migrationBuilder.DropForeignKey(
                name: "FK_SchemeGPG45Mapping_SupplementaryScheme_SupplementarySchemeId",
                table: "SchemeGPG45Mapping");

            migrationBuilder.DropColumn(
                name: "HasGpg44Mapping",
                table: "ServiceSupSchemeMapping");

            migrationBuilder.DropColumn(
                name: "IsUnderPinningServicePublished",
                table: "Service");

            migrationBuilder.RenameColumn(
                name: "ServiceSupSchemeMappingId",
                table: "SchemeGPG45Mapping",
                newName: "ServiceId");

            migrationBuilder.RenameIndex(
                name: "IX_SchemeGPG45Mapping_ServiceSupSchemeMappingId",
                table: "SchemeGPG45Mapping",
                newName: "IX_SchemeGPG45Mapping_ServiceId");

            migrationBuilder.RenameColumn(
                name: "ServiceSupSchemeMappingId",
                table: "SchemeGPG44Mapping",
                newName: "ServiceId");

            migrationBuilder.RenameIndex(
                name: "IX_SchemeGPG44Mapping_ServiceSupSchemeMappingId",
                table: "SchemeGPG44Mapping",
                newName: "IX_SchemeGPG44Mapping_ServiceId");

            migrationBuilder.AlterColumn<int>(
                name: "SupplementarySchemeId",
                table: "SchemeGPG45Mapping",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "SupplementarySchemeId",
                table: "SchemeGPG44Mapping",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_SchemeGPG44Mapping_Service_ServiceId",
                table: "SchemeGPG44Mapping",
                column: "ServiceId",
                principalTable: "Service",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SchemeGPG44Mapping_SupplementaryScheme_SupplementarySchemeId",
                table: "SchemeGPG44Mapping",
                column: "SupplementarySchemeId",
                principalTable: "SupplementaryScheme",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SchemeGPG45Mapping_Service_ServiceId",
                table: "SchemeGPG45Mapping",
                column: "ServiceId",
                principalTable: "Service",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SchemeGPG45Mapping_SupplementaryScheme_SupplementarySchemeId",
                table: "SchemeGPG45Mapping",
                column: "SupplementarySchemeId",
                principalTable: "SupplementaryScheme",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
