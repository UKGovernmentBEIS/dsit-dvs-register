﻿
using DVSRegister.CommonUtility.Models;
using DVSRegister.Data.Entities;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DVSRegister.Data
{
    public class DVSRegisterDbContext : DbContext, IDataProtectionKeyContext
    {
        public DVSRegisterDbContext(DbContextOptions<DVSRegisterDbContext> options) : base(options)
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        }
      
        public DbSet<User> User { get; set; }    
        public DbSet<Role> Role { get; set; }       
        public DbSet<IdentityProfile> IdentityProfile { get; set; }       
        public DbSet<SupplementaryScheme> SupplementaryScheme { get; set; }
        public DbSet<CertificateReviewRejectionReason> CertificateReviewRejectionReason { get; set; } 
        
        public DbSet<RegisterPublishLog> RegisterPublishLog { get; set; } 
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
        public DbSet<ProceedApplicationConsentToken> ProceedApplicationConsentToken { get; set; }
        public DbSet<PublicInterestCheck> PublicInterestCheck { get; set; }
        public DbSet<PICheckLogs> PICheckLogs { get; set; }
        public DbSet<ProceedPublishConsentToken> ProceedPublishConsentToken { get; set; }
        public DbSet<DataProtectionKey> DataProtectionKeys { get; set; }
        public DbSet<TrustmarkNumber> TrustmarkNumber { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            Console.WriteLine(environment);

            modelBuilder.Entity<TrustmarkNumber>()
            .Property(t => t.TrustMarkNumber)
            .HasComputedColumnSql("LPAD(\"CompanyId\"::VARCHAR(4), 4, '0') || LPAD(\"ServiceNumber\"::VARCHAR(2), 2, '0')", stored: true);

            modelBuilder.Entity<TrustmarkNumber>()
            .HasIndex(b => b.TrustMarkNumber)
            .IsUnique(); // Trustmark number unique

            modelBuilder.Entity<TrustmarkNumber>()
            .HasIndex(p => new { p.ProviderProfileId, p.ServiceId })
             .IsUnique();  // Second composite unique key

            modelBuilder.Entity<TrustmarkNumber>()
            .ToTable(b => b.HasCheckConstraint("CK_CompanyId", "\"CompanyId\" BETWEEN 200 AND 9999"));
            modelBuilder.Entity<TrustmarkNumber>()
            .ToTable(b => b.HasCheckConstraint("CK_ServiceNumber", "\"ServiceNumber\" BETWEEN 1 AND 99"));  

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

            if(environment != "Production")
            {
               modelBuilder.Entity<Cab>().HasData(
               new Cab { Id =1, CabName = "EY", CreatedTime = DateTime.UtcNow },
               new Cab { Id =2, CabName = "DSIT", CreatedTime = DateTime.UtcNow },
               new Cab { Id =3, CabName = "ACCS", CreatedTime = DateTime.UtcNow },
               new Cab { Id =4, CabName = "Kantara", CreatedTime = DateTime.UtcNow },           
               new Cab { Id =6, CabName = "NQA", CreatedTime = DateTime.UtcNow },
               new Cab { Id =7, CabName = "BSI", CreatedTime = DateTime.UtcNow });

            }
            else
            {
              modelBuilder.Entity<Cab>().HasData(
               new Cab { Id =1, CabName = "ACCS", CreatedTime = DateTime.UtcNow },
               new Cab { Id =2, CabName = "Kantara", CreatedTime = DateTime.UtcNow },
               new Cab { Id =3, CabName = "NQA", CreatedTime = DateTime.UtcNow },
               new Cab { Id =4, CabName = "BSI", CreatedTime = DateTime.UtcNow },
               new Cab { Id =5, CabName = "DSIT", CreatedTime = DateTime.UtcNow });
            }

           

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
