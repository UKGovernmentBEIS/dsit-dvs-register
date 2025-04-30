using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DVSRegister.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddIsInRegisterColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsInRegister",
                table: "Service",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsInRegister",
                table: "ProviderProfile",
                type: "boolean",
                nullable: false,
                defaultValue: false);


            migrationBuilder.Sql(@"
             UPDATE ""Service""
             SET ""IsInRegister"" = true WHERE ""ServiceStatus"" = 4 OR ""ServiceStatus""= 6 OR ""ServiceStatus"" = 7 ");

            migrationBuilder.Sql(@"
             UPDATE ""Service"" s
             SET ""IsInRegister"" = true FROM ""ServiceDraft""  sd
             WHERE s.""Id"" = sd.""ServiceId"" AND  s.""ServiceStatus"" = 9 AND sd.""PreviousServiceStatus"" = 4 ");

            migrationBuilder.Sql(@"
             UPDATE ""ProviderProfile"" p
             SET ""IsInRegister"" = true FROM ""ProviderProfileDraft""   pd
             WHERE   p.""Id"" =  pd.""ProviderProfileId"" AND  p.""ProviderStatus"" = 8 AND pd.""PreviousProviderStatus"" = 3");          

            migrationBuilder.Sql(@"
             UPDATE ""ProviderProfile"" p
             SET ""IsInRegister"" = true FROM  ""ServiceDraft""   sd        
             WHERE  p.""Id"" = sd.""ProviderProfileId"" AND p.""ProviderStatus"" = 8 AND sd.""PreviousServiceStatus"" = 4 ");

            migrationBuilder.Sql(@"
             UPDATE ""ProviderProfile""
             SET ""IsInRegister"" = true WHERE ""ProviderStatus"" = 3 OR ""ProviderStatus"" =4 OR ""ProviderStatus"" =6 OR ""ProviderStatus"" = 7 ");







            //---------Service status ------------//
            //Published = 4,
            //AwaitingRemovalConfirmation = 6,
            //CabAwaitingRemovalConfirmation = 7,
            //UpdatesRequested = 9,

            //---------Provider status ------------//
            //Published = 3,
            //ReadyToPublishNext = 4
            //AwaitingRemovalConfirmation = 6,
            //CabAwaitingRemovalConfirmation = 7,
            //UpdatesRequested = 8
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsInRegister",
                table: "Service");

            migrationBuilder.DropColumn(
                name: "IsInRegister",
                table: "ProviderProfile");
        }
    }
}
