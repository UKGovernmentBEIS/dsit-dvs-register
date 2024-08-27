
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
        public DbSet<Role> Role { get; set; }       
        public DbSet<IdentityProfile> IdentityProfile { get; set; }       
        public DbSet<SupplementaryScheme> SupplementaryScheme { get; set; }
        public DbSet<CertificateReviewRejectionReason> CertificateReviewRejectionReason { get; set; }       
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
        public DbSet<CertificateReview> CertificateReview { get; set; }
        public DbSet<CertificateReviewRejectionReasonMapping> CertificateReviewRejectionReasonMapping { get; set; }

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
            new Cab { Id =2, CabName = "DSIT", CreatedTime = DateTime.UtcNow },
            new Cab { Id =3, CabName = "ACCS", CreatedTime = DateTime.UtcNow },
            new Cab { Id =4, CabName = "Kantara Initiative", CreatedTime = DateTime.UtcNow });



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

                


        }
    }
}
