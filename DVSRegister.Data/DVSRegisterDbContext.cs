﻿
using DVSRegister.CommonUtility.Models;
using DVSRegister.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace DVSRegister.Data
{
    public class DVSRegisterDbContext : DbContext
    {
        public DVSRegisterDbContext(DbContextOptions<DVSRegisterDbContext> options) : base(options)
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        }
        public DbSet<PreRegistration> PreRegistration { get; set; }
        public DbSet<Country> Country { get; set; }
        public DbSet<PreRegistrationCountryMapping> PreRegistrationCountryMapping { get; set; }
        public DbSet<UniqueReferenceNumber> UniqueReferenceNumber { get; set; }
        public DbSet<User> User { get; set; }
        public DbSet<PreRegistrationReview> PreRegistrationReview { get; set; }
        public DbSet<CertificateInformation> CertificateInformation { get; set; }
        public DbSet<Role> Role { get; set; }
        public DbSet<CertificateInfoRoleMapping> CertificateInfoRoleMapping { get; set; }
        public DbSet<IdentityProfile> IdentityProfile { get; set; }
        public DbSet<CertificateInfoIdentityProfileMapping> CertificateInfoIdentityProfileMapping { get; set; }
        public DbSet<SupplementaryScheme> SupplementaryScheme { get; set; }
        public DbSet<CertificateInfoSupSchemeMapping> CertificateInfoSupSchemeMappings { get; set; }

        public DbSet<CertificateReview> CertificateReview { get; set; }
        public DbSet<CertificateReviewRejectionReason> CertificateReviewRejectionReason { get; set; }
        public DbSet<CertificateReviewRejectionReasonMappings> CertificateReviewRejectionReasonMappings { get; set; }
        public DbSet<Provider> Provider { get; set; }
        public DbSet<ConsentToken> ConsentToken { get; set; }
        public DbSet<RegisterPublishLog> RegisterPublishLog { get; set; }

        #region new path
        public DbSet<Cab> Cab { get; set; }
        public DbSet<CabUser> CabUser { get; set; }
        public DbSet<ProviderProfile> ProviderProfile { get; set; }
        public DbSet<Service> Service { get; set; }
        public DbSet<QualityLevel> QualityLevel { get; set; }
        public DbSet<ServiceQualityLevelMapping> ServiceQualityLevelMapping { get; set; }
        public DbSet<ServiceIdentityProfileMapping> ServiceIdentityProfileMapping { get; set; }
        public DbSet<ServiceRoleMapping> ServiceRoleMapping { get; set; }
        public DbSet<ServiceSupSchemeMapping> ServiceSupSchemeMapping { get; set; }
       
        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProviderProfile>()
            .HasGeneratedTsVectorColumn(p => p.SearchVector, "english", p => new { p.RegisteredName, p.TradingName })
            .HasIndex(p => p.SearchVector)
            .HasMethod("GIN");

            modelBuilder.Entity<Service>()
           .HasGeneratedTsVectorColumn(p => p.SearchVector, "english", p => new { p.ServiceName })
           .HasIndex(p => p.SearchVector)
           .HasMethod("GIN");

            modelBuilder.Entity<QualityLevel>().HasData(
            new QualityLevel { Id =1, Level = "Low", QualityType = QualityTypeEnum.Authentication },
            new QualityLevel { Id =2, Level = "Medium", QualityType = QualityTypeEnum.Authentication },
            new QualityLevel { Id =3, Level = "High", QualityType = QualityTypeEnum.Authentication },
            new QualityLevel { Id =4, Level = "Low", QualityType = QualityTypeEnum.Protection },
            new QualityLevel { Id =5, Level = "Medium", QualityType = QualityTypeEnum.Protection },
            new QualityLevel { Id =6, Level = "High", QualityType = QualityTypeEnum.Protection },
            new QualityLevel { Id =7, Level = "Very High", QualityType = QualityTypeEnum.Protection });

            modelBuilder.Entity<Cab>().HasData(
            new Cab { Id =1, CabName = "EY", CreatedTime = DateTime.UtcNow },
            new Cab { Id =2, CabName = "DSIT", CreatedTime = DateTime.UtcNow });
       

            modelBuilder.Entity<Provider>()
            .HasGeneratedTsVectorColumn( p => p.SearchVector,  "english", p => new { p.RegisteredName, p.TradingName })  
            .HasIndex(p => p.SearchVector)
            .HasMethod("GIN"); 

            modelBuilder.Entity<CertificateReviewRejectionReason>().HasData(
            new CertificateReviewRejectionReason { Id =1, Reason = "Information is missing from the certificate" },
            new CertificateReviewRejectionReason { Id =2, Reason = "The certificate contains invalid information" },
            new CertificateReviewRejectionReason { Id =3, Reason = "The information submitted does not match the information on the certificate" },
            new CertificateReviewRejectionReason { Id =4, Reason = "The certificate or information submitted contains errors" },
            new CertificateReviewRejectionReason { Id =5, Reason = "Other" });

            modelBuilder.Entity<Role>().HasData(
            new Role { Id =1, RoleName = "Identity Service Provider (IDSP)" },
            new Role { Id =2, RoleName = "Attribute Service Provider (ASP)" },
            new Role { Id =3, RoleName = "Orchestration Service Provider (OSP)" });

                modelBuilder.Entity<IdentityProfile>().HasData(
                new IdentityProfile { Id =1, IdentityProfileName = "L1A " },
                new IdentityProfile { Id =2, IdentityProfileName = "L1B " },
                new IdentityProfile { Id =3, IdentityProfileName = "L1C " },
                new IdentityProfile { Id =4, IdentityProfileName = "L2A " },
                new IdentityProfile { Id =5, IdentityProfileName = "L2B " },
                new IdentityProfile { Id =6, IdentityProfileName = "L3A " },
                new IdentityProfile { Id =7, IdentityProfileName = "M1A " },
                new IdentityProfile { Id =8, IdentityProfileName = "M1B " },
                new IdentityProfile { Id =9, IdentityProfileName = "M1C " },
                new IdentityProfile { Id =10, IdentityProfileName = "M1D " },
                new IdentityProfile { Id =11, IdentityProfileName = "M2A " },
                new IdentityProfile { Id =12, IdentityProfileName = "M2B " },
                new IdentityProfile { Id =13, IdentityProfileName = "M2C " },
                new IdentityProfile { Id =14, IdentityProfileName = "M3A " },
                new IdentityProfile { Id =15, IdentityProfileName = "H1A " },
                new IdentityProfile { Id =16, IdentityProfileName = "H1B " },
                new IdentityProfile { Id =17, IdentityProfileName = "H1C " },
                new IdentityProfile { Id =18, IdentityProfileName = "H2A " },
                new IdentityProfile { Id =19, IdentityProfileName = "H2B " },
                new IdentityProfile { Id =20, IdentityProfileName = "H2C " },
                new IdentityProfile { Id =21, IdentityProfileName = "H2D " },
                new IdentityProfile { Id =22, IdentityProfileName = "H2E " },
                new IdentityProfile { Id =23, IdentityProfileName = "H3A " },
                new IdentityProfile { Id =24, IdentityProfileName = "V1A " },
                new IdentityProfile { Id =25, IdentityProfileName = "V1B " },
                new IdentityProfile { Id =26, IdentityProfileName = "V1C " },
                new IdentityProfile { Id =27, IdentityProfileName = "V1D " },
                new IdentityProfile { Id =28, IdentityProfileName = "V2A " },
                new IdentityProfile { Id =29, IdentityProfileName = "V2B " },
                new IdentityProfile { Id =30, IdentityProfileName = "V2C " },
                new IdentityProfile { Id =31, IdentityProfileName = "V2D " },
                new IdentityProfile { Id =32, IdentityProfileName = "V3A " });

                modelBuilder.Entity<SupplementaryScheme>().HasData(
                new SupplementaryScheme { Id =1, SchemeName = "Right to Work" },
                new SupplementaryScheme { Id =2, SchemeName = "Right to Rent" },
                new SupplementaryScheme { Id =3, SchemeName = "Disclosure and Barring Service" });

                 modelBuilder.Entity<Country>().HasData(
                 new Country { Id =1, CountryName = "Afghanistan" },
                 new Country { Id =2, CountryName = "Albania" },
                 new Country { Id =3, CountryName = "Algeria" },
                 new Country { Id =4, CountryName = "Andorra" },
                 new Country { Id =5, CountryName = "Angola" },
                 new Country { Id =6, CountryName = "Antigua and Barbuda" },
                 new Country { Id =7, CountryName = "Argentina" },
                 new Country { Id =8, CountryName = "Armenia" },
                 new Country { Id =9, CountryName = "Australia" },
                 new Country { Id =10, CountryName = "Austria" },
                 new Country { Id =11, CountryName = "Azerbaijan" },
                 new Country { Id =12, CountryName = "Bahrain" },
                 new Country { Id =13, CountryName = "Bangladesh" },
                 new Country { Id =14, CountryName = "Barbados" },
                 new Country { Id =15, CountryName = "Belarus" },
                 new Country { Id =16, CountryName = "Belgium" },
                 new Country { Id =17, CountryName = "Belize" },
                 new Country { Id =18, CountryName = "Benin" },
                 new Country { Id =19, CountryName = "Bhutan" },
                 new Country { Id =20, CountryName = "Bolivia" },
                 new Country { Id =21, CountryName = "Bosnia and Herzegovina" },
                 new Country { Id =22, CountryName = "Botswana" },
                 new Country { Id =23, CountryName = "Brazil" },
                 new Country { Id =24, CountryName = "Brunei" },
                 new Country { Id =25, CountryName = "Bulgaria" },
                 new Country { Id =26, CountryName = "Burkina Faso" },
                 new Country { Id =27, CountryName = "Burundi" },
                 new Country { Id =28, CountryName = "Cambodia" },
                 new Country { Id =29, CountryName = "Cameroon" },
                 new Country { Id =30, CountryName = "Canada" },
                 new Country { Id =31, CountryName = "Cape Verde" },
                 new Country { Id =32, CountryName = "Central African Republic" },
                 new Country { Id =33, CountryName = "Chad" },
                 new Country { Id =34, CountryName = "Chile" },
                 new Country { Id =35, CountryName = "China" },
                 new Country { Id =36, CountryName = "Colombia" },
                 new Country { Id =37, CountryName = "Comoros" },
                 new Country { Id =38, CountryName = "Congo" },
                 new Country { Id =39, CountryName = "Congo (Democratic Republic)" },
                 new Country { Id =40, CountryName = "Costa Rica" },
                 new Country { Id =41, CountryName = "Croatia" },
                 new Country { Id =42, CountryName = "Cuba" },
                 new Country { Id =43, CountryName = "Cyprus" },
                 new Country { Id =44, CountryName = "Czechia" },
                 new Country { Id =45, CountryName = "Denmark" },
                 new Country { Id =46, CountryName = "Djibouti" },
                 new Country { Id =47, CountryName = "Dominica" },
                 new Country { Id =48, CountryName = "Dominican Republic" },
                 new Country { Id =49, CountryName = "East Timor" },
                 new Country { Id =50, CountryName = "Ecuador" },
                 new Country { Id =51, CountryName = "Egypt" },
                 new Country { Id =52, CountryName = "El Salvador" },
                 new Country { Id =53, CountryName = "Equatorial Guinea" },
                 new Country { Id =54, CountryName = "Eritrea" },
                 new Country { Id =55, CountryName = "Estonia" },
                 new Country { Id =56, CountryName = "Eswatini" },
                 new Country { Id =57, CountryName = "Ethiopia" },
                 new Country { Id =58, CountryName = "Fiji" },
                 new Country { Id =59, CountryName = "Finland" },
                 new Country { Id =60, CountryName = "France" },
                 new Country { Id =61, CountryName = "Gabon" },
                 new Country { Id =62, CountryName = "Georgia" },
                 new Country { Id =63, CountryName = "Germany" },
                 new Country { Id =64, CountryName = "Ghana" },
                 new Country { Id =65, CountryName = "Greece" },
                 new Country { Id =66, CountryName = "Grenada" },
                 new Country { Id =67, CountryName = "Guatemala" },
                 new Country { Id =68, CountryName = "Guinea" },
                 new Country { Id =69, CountryName = "Guinea-Bissau" },
                 new Country { Id =70, CountryName = "Guyana" },
                 new Country { Id =71, CountryName = "Haiti" },
                 new Country { Id =72, CountryName = "Honduras" },
                 new Country { Id =73, CountryName = "Hungary" },
                 new Country { Id =74, CountryName = "Iceland" },
                 new Country { Id =75, CountryName = "India" },
                 new Country { Id =76, CountryName = "Indonesia" },
                 new Country { Id =77, CountryName = "Iran" },
                 new Country { Id =78, CountryName = "Iraq" },
                 new Country { Id =79, CountryName = "Ireland" },
                 new Country { Id =80, CountryName = "Israel" },
                 new Country { Id =81, CountryName = "Italy" },
                 new Country { Id =82, CountryName = "Ivory Coast" },
                 new Country { Id =83, CountryName = "Jamaica" },
                 new Country { Id =84, CountryName = "Japan" },
                 new Country { Id =85, CountryName = "Jordan" },
                 new Country { Id =86, CountryName = "Kazakhstan" },
                 new Country { Id =87, CountryName = "Kenya" },
                 new Country { Id =88, CountryName = "Kiribati" },
                 new Country { Id =89, CountryName = "Kosovo" },
                 new Country { Id =90, CountryName = "Kuwait" },
                 new Country { Id =91, CountryName = "Kyrgyzstan" },
                 new Country { Id =92, CountryName = "Laos" },
                 new Country { Id =93, CountryName = "Latvia" },
                 new Country { Id =94, CountryName = "Lebanon" },
                 new Country { Id =95, CountryName = "Lesotho" },
                 new Country { Id =96, CountryName = "Liberia" },
                 new Country { Id =97, CountryName = "Libya" },
                 new Country { Id =98, CountryName = "Liechtenstein" },
                 new Country { Id =99, CountryName = "Lithuania" },
                 new Country { Id =100, CountryName = "Luxembourg" },
                 new Country { Id =101, CountryName = "Madagascar" },
                 new Country { Id =102, CountryName = "Malawi" },
                 new Country { Id =103, CountryName = "Malaysia" },
                 new Country { Id =104, CountryName = "Maldives" },
                 new Country { Id =105, CountryName = "Mali" },
                 new Country { Id =106, CountryName = "Malta" },
                 new Country { Id =107, CountryName = "Marshall Islands" },
                 new Country { Id =108, CountryName = "Mauritania" },
                 new Country { Id =109, CountryName = "Mauritius" },
                 new Country { Id =110, CountryName = "Mexico" },
                 new Country { Id =111, CountryName = "Federated States of Micronesia" },
                 new Country { Id =112, CountryName = "Moldova" },
                 new Country { Id =113, CountryName = "Monaco" },
                 new Country { Id =114, CountryName = "Mongolia" },
                 new Country { Id =115, CountryName = "Montenegro" },
                 new Country { Id =116, CountryName = "Morocco" },
                 new Country { Id =117, CountryName = "Mozambique" },
                 new Country { Id =118, CountryName = "Myanmar (Burma)" },
                 new Country { Id =119, CountryName = "Namibia" },
                 new Country { Id =120, CountryName = "Nauru" },
                 new Country { Id =121, CountryName = "Nepal" },
                 new Country { Id =122, CountryName = "Netherlands" },
                 new Country { Id =123, CountryName = "New Zealand" },
                 new Country { Id =124, CountryName = "Nicaragua" },
                 new Country { Id =125, CountryName = "Niger" },
                 new Country { Id =126, CountryName = "Nigeria" },
                 new Country { Id =127, CountryName = "North Korea" },
                 new Country { Id =128, CountryName = "North Macedonia" },
                 new Country { Id =129, CountryName = "Norway" },
                 new Country { Id =130, CountryName = "Oman" },
                 new Country { Id =131, CountryName = "Pakistan" },
                 new Country { Id =132, CountryName = "Palau" },
                 new Country { Id =133, CountryName = "Panama" },
                 new Country { Id =134, CountryName = "Papua New Guinea" },
                 new Country { Id =135, CountryName = "Paraguay" },
                 new Country { Id =136, CountryName = "Peru" },
                 new Country { Id =137, CountryName = "Philippines" },
                 new Country { Id =138, CountryName = "Poland" },
                 new Country { Id =139, CountryName = "Portugal" },
                 new Country { Id =140, CountryName = "Qatar" },
                 new Country { Id =141, CountryName = "Romania" },
                 new Country { Id =142, CountryName = "Russia" },
                 new Country { Id =143, CountryName = "Rwanda" },
                 new Country { Id =144, CountryName = "St Kitts and Nevis" },
                 new Country { Id =145, CountryName = "St Lucia" },
                 new Country { Id =146, CountryName = "St Vincent" },
                 new Country { Id =147, CountryName = "Samoa" },
                 new Country { Id =148, CountryName = "San Marino" },
                 new Country { Id =149, CountryName = "Sao Tome and Principe" },
                 new Country { Id =150, CountryName = "Saudi Arabia" },
                 new Country { Id =151, CountryName = "Senegal" },
                 new Country { Id =152, CountryName = "Serbia" },
                 new Country { Id =153, CountryName = "Seychelles" },
                 new Country { Id =154, CountryName = "Sierra Leone" },
                 new Country { Id =155, CountryName = "Singapore" },
                 new Country { Id =156, CountryName = "Slovakia" },
                 new Country { Id =157, CountryName = "Slovenia" },
                 new Country { Id =158, CountryName = "Solomon Islands" },
                 new Country { Id =159, CountryName = "Somalia" },
                 new Country { Id =160, CountryName = "South Africa" },
                 new Country { Id =161, CountryName = "South Korea" },
                 new Country { Id =162, CountryName = "South Sudan" },
                 new Country { Id =163, CountryName = "Spain" },
                 new Country { Id =164, CountryName = "Sri Lanka" },
                 new Country { Id =168, CountryName = "Sudan" },
                 new Country { Id =169, CountryName = "Suriname" },
                 new Country { Id =170, CountryName = "Sweden" },
                 new Country { Id =171, CountryName = "Switzerland" },
                 new Country { Id =172, CountryName = "Syria" },
                 new Country { Id =173, CountryName = "Tajikistan" },
                 new Country { Id =174, CountryName = "Tanzania" },
                 new Country { Id =175, CountryName = "Thailand" },
                 new Country { Id =176, CountryName = "The Bahamas" },
                 new Country { Id =177, CountryName = "The Gambia" },
                 new Country { Id =178, CountryName = "Togo" },
                 new Country { Id =179, CountryName = "Tonga" },
                 new Country { Id =180, CountryName = "Trinidad and Tobago" },
                 new Country { Id =181, CountryName = "Tunisia" },
                 new Country { Id =182, CountryName = "Turkey" },
                 new Country { Id =183, CountryName = "Turkmenistan" },
                 new Country { Id =184, CountryName = "Tuvalu" },
                 new Country { Id =185, CountryName = "Uganda" },
                 new Country { Id =186, CountryName = "Ukraine" },
                 new Country { Id =187, CountryName = "United Arab Emirates" },
                 new Country { Id =188, CountryName = "United Kingdom" },
                 new Country { Id =189, CountryName = "United States" },
                 new Country { Id =190, CountryName = "Uruguay " },
                 new Country { Id =191, CountryName = "Uzbekistan " },
                 new Country { Id =192, CountryName = "Vanuatu" },
                 new Country { Id =193, CountryName = "Vatican City" },
                 new Country { Id =194, CountryName = "Venezuela" },
                 new Country { Id =195, CountryName = "Vietnam" },
                 new Country { Id =196, CountryName = "Yemen" },
                 new Country { Id =197, CountryName = "Zambia" },
                 new Country { Id =198, CountryName = "Zimbabwe" },
                new Country { Id = 199, CountryName = "Akrotiri" },
                new Country { Id = 200, CountryName = "Anguilla" },
                new Country { Id = 201, CountryName = "Bermuda" },
                new Country { Id = 202, CountryName = "British Antarctic Territory" },
                new Country { Id = 203, CountryName = "British Indian Ocean Territory" },
                new Country { Id = 204, CountryName = "British Virgin Islands" },
                new Country { Id = 205, CountryName = "Cayman Islands" },
                new Country { Id = 206, CountryName = "Dhekelia" },
                new Country { Id = 207, CountryName = "Falkland Islands" },
                new Country { Id = 208, CountryName = "Gibraltar" },
                new Country { Id = 209, CountryName = "Montserrat" },
                new Country { Id = 210, CountryName = "Pitcairn, Henderson, Ducie and Oeno Islands" },
                new Country { Id = 211, CountryName = "St Helena, Ascension and Tristan da Cunha" },
                new Country { Id = 212, CountryName = "South Georgia and South Sandwich Islands" },
                new Country { Id = 213, CountryName = "Turks and Caicos Islands" });


        }
    }
}
