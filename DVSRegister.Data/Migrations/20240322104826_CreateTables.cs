using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DVSRegister.Data.Migrations
{
    /// <inheritdoc />
    public partial class CreateTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Country",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CountryName = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Country", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PreRegistration",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IsApplicationSponsor = table.Column<bool>(type: "boolean", nullable: false),
                    FullName = table.Column<string>(type: "text", nullable: false),
                    JobTitle = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "varchar(254)", nullable: false),
                    TelephoneNumber = table.Column<string>(type: "text", nullable: false),
                    SponsorFullName = table.Column<string>(type: "text", nullable: true),
                    SponsorJobTitle = table.Column<string>(type: "text", nullable: true),
                    SponsorEmail = table.Column<string>(type: "varchar(254)", nullable: true),
                    SponsorTelephoneNumber = table.Column<string>(type: "text", nullable: true),
                    RegisteredCompanyName = table.Column<string>(type: "varchar(160)", nullable: false),
                    TradingName = table.Column<string>(type: "text", nullable: false),
                    CompanyRegistrationNumber = table.Column<string>(type: "varchar(8)", nullable: false),
                    HasParentCompany = table.Column<bool>(type: "boolean", nullable: false),
                    ParentCompanyRegisteredName = table.Column<string>(type: "varchar(160)", nullable: true),
                    ParentCompanyLocation = table.Column<string>(type: "varchar(160)", nullable: true),
                    ConfirmAccuracy = table.Column<int>(type: "integer", nullable: false),
                    URN = table.Column<string>(type: "text", nullable: true),
                    PreRegistrationStatus = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PreRegistration", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PreRegistrationCountryMapping",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PreRegistrationId = table.Column<int>(type: "integer", nullable: false),
                    CountryId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PreRegistrationCountryMapping", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PreRegistrationCountryMapping_Country_CountryId",
                        column: x => x.CountryId,
                        principalTable: "Country",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PreRegistrationCountryMapping_PreRegistration_PreRegistrati~",
                        column: x => x.PreRegistrationId,
                        principalTable: "PreRegistration",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Country",
                columns: new[] { "Id", "CountryName" },
                values: new object[,]
                {
                    { 1, "Afghanistan " },
                    { 2, "Albania " },
                    { 3, "Algeria " },
                    { 4, "Andorra " },
                    { 5, "Angola " },
                    { 6, "Antigua and Barbuda " },
                    { 7, "Argentina " },
                    { 8, "Armenia " },
                    { 9, "Australia " },
                    { 10, "Austria " },
                    { 11, "Azerbaijan " },
                    { 12, "Bahrain " },
                    { 13, "Bangladesh " },
                    { 14, "Barbados " },
                    { 15, "Belarus " },
                    { 16, "Belgium " },
                    { 17, "Belize " },
                    { 18, "Benin " },
                    { 19, "Bhutan " },
                    { 20, "Bolivia " },
                    { 21, "Bosnia and Herzegovina " },
                    { 22, "Botswana " },
                    { 23, "Brazil " },
                    { 24, "Brunei " },
                    { 25, "Bulgaria " },
                    { 26, "Burkina Faso " },
                    { 27, "Burundi " },
                    { 28, "Cambodia " },
                    { 29, "Cameroon " },
                    { 30, "Canada " },
                    { 31, "Cape Verde " },
                    { 32, "Central African Republic " },
                    { 33, "Chad " },
                    { 34, "Chile " },
                    { 35, "China " },
                    { 36, "Colombia " },
                    { 37, "Comoros " },
                    { 38, "Congo " },
                    { 39, "Congo (Democratic Republic) " },
                    { 40, "Costa Rica " },
                    { 41, "Croatia " },
                    { 42, "Cuba " },
                    { 43, "Cyprus " },
                    { 44, "Czechia " },
                    { 45, "Denmark " },
                    { 46, "Djibouti " },
                    { 47, "Dominica " },
                    { 48, "Dominican Republic " },
                    { 49, "East Timor " },
                    { 50, "Ecuador " },
                    { 51, "Egypt " },
                    { 52, "El Salvador " },
                    { 53, "Equatorial Guinea " },
                    { 54, "Eritrea " },
                    { 55, "Estonia " },
                    { 56, "Eswatini " },
                    { 57, "Ethiopia " },
                    { 58, "Fiji " },
                    { 59, "Finland " },
                    { 60, "France " },
                    { 61, "Gabon " },
                    { 62, "Georgia " },
                    { 63, "Germany " },
                    { 64, "Ghana " },
                    { 65, "Greece " },
                    { 66, "Grenada " },
                    { 67, "Guatemala " },
                    { 68, "Guinea " },
                    { 69, "Guinea-Bissau " },
                    { 70, "Guyana " },
                    { 71, "Haiti " },
                    { 72, "Honduras " },
                    { 73, "Hungary " },
                    { 74, "Iceland " },
                    { 75, "India " },
                    { 76, "Indonesia " },
                    { 77, "Iran " },
                    { 78, "Iraq " },
                    { 79, "Ireland " },
                    { 80, "Israel " },
                    { 81, "Italy " },
                    { 82, "Ivory Coast " },
                    { 83, "Jamaica " },
                    { 84, "Japan " },
                    { 85, "Jordan " },
                    { 86, "Kazakhstan " },
                    { 87, "Kenya " },
                    { 88, "Kiribati " },
                    { 89, "Kosovo " },
                    { 90, "Kuwait " },
                    { 91, "Kyrgyzstan " },
                    { 92, "Laos " },
                    { 93, "Latvia " },
                    { 94, "Lebanon " },
                    { 95, "Lesotho " },
                    { 96, "Liberia " },
                    { 97, "Libya " },
                    { 98, "Liechtenstein " },
                    { 99, "Lithuania " },
                    { 100, "Luxembourg " },
                    { 101, "Madagascar " },
                    { 102, "Malawi " },
                    { 103, "Malaysia " },
                    { 104, "Maldives " },
                    { 105, "Mali " },
                    { 106, "Malta " },
                    { 107, "Marshall Islands " },
                    { 108, "Mauritania " },
                    { 109, "Mauritius " },
                    { 110, "Mexico " },
                    { 111, "Federated States of Micronesia " },
                    { 112, "Moldova " },
                    { 113, "Monaco " },
                    { 114, "Mongolia " },
                    { 115, "Montenegro " },
                    { 116, "Morocco " },
                    { 117, "Mozambique " },
                    { 118, "Myanmar (Burma) " },
                    { 119, "Namibia " },
                    { 120, "Nauru " },
                    { 121, "Nepal " },
                    { 122, "Netherlands " },
                    { 123, "New Zealand " },
                    { 124, "Nicaragua " },
                    { 125, "Niger " },
                    { 126, "Nigeria " },
                    { 127, "North Korea " },
                    { 128, "North Macedonia " },
                    { 129, "Norway " },
                    { 130, "Oman " },
                    { 131, "Pakistan " },
                    { 132, "Palau " },
                    { 133, "Panama " },
                    { 134, "Papua New Guinea " },
                    { 135, "Paraguay " },
                    { 136, "Peru " },
                    { 137, "Philippines " },
                    { 138, "Poland " },
                    { 139, "Portugal " },
                    { 140, "Qatar " },
                    { 141, "Romania " },
                    { 142, "Russia " },
                    { 143, "Rwanda " },
                    { 144, "St Kitts and Nevis " },
                    { 145, "St Lucia " },
                    { 146, "St Vincent " },
                    { 147, "Samoa " },
                    { 148, "San Marino " },
                    { 149, "Sao Tome and Principe " },
                    { 150, "Saudi Arabia " },
                    { 151, "Senegal " },
                    { 152, "Serbia " },
                    { 153, "Seychelles " },
                    { 154, "Sierra Leone " },
                    { 155, "Singapore " },
                    { 156, "Slovakia " },
                    { 157, "Slovenia " },
                    { 158, "Solomon Islands " },
                    { 159, "Somalia " },
                    { 160, "South Africa " },
                    { 161, "South Korea " },
                    { 162, "South Sudan " },
                    { 163, "Spain " },
                    { 164, "Sri Lanka " },
                    { 165, "St Kitts and Nevis " },
                    { 166, "St Lucia " },
                    { 167, "St Vincent " },
                    { 168, "Sudan " },
                    { 169, "Suriname " },
                    { 170, "Sweden " },
                    { 171, "Switzerland " },
                    { 172, "Syria " },
                    { 173, "Tajikistan " },
                    { 174, "Tanzania " },
                    { 175, "Thailand " },
                    { 176, "The Bahamas " },
                    { 177, "The Gambia " },
                    { 178, "Togo " },
                    { 179, "Tonga " },
                    { 180, "Trinidad and Tobago " },
                    { 181, "Tunisia " },
                    { 182, "Turkey " },
                    { 183, "Turkmenistan " },
                    { 184, "Tuvalu " },
                    { 185, "Uganda " },
                    { 186, "Ukraine " },
                    { 187, "United Arab Emirates " },
                    { 188, "United Kingdom " },
                    { 189, "United States " },
                    { 190, "Uruguay " },
                    { 191, "Uzbekistan " },
                    { 192, "Vanuatu " },
                    { 193, "Vatican City " },
                    { 194, "Venezuela " },
                    { 195, "Vietnam " },
                    { 196, "Yemen " },
                    { 197, "Zambia " },
                    { 198, "Zimbabwe " }
                });

            migrationBuilder.CreateIndex(
                name: "IX_PreRegistrationCountryMapping_CountryId",
                table: "PreRegistrationCountryMapping",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_PreRegistrationCountryMapping_PreRegistrationId",
                table: "PreRegistrationCountryMapping",
                column: "PreRegistrationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PreRegistrationCountryMapping");

            migrationBuilder.DropTable(
                name: "Country");

            migrationBuilder.DropTable(
                name: "PreRegistration");
        }
    }
}
