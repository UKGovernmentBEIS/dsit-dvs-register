
using DVSRegister.CommonUtility.Models;
using DVSRegister.CommonUtility.Models.Enums;
using DVSRegister.Data.Entities;
using DVSRegister.Data.Models;
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
        public DbSet<Event> EventLogs { get; set; }
        public DbSet<RemoveProviderToken> RemoveProviderToken { get; set; }
        public DbSet<RemoveTokenServiceMapping> RemoveTokenServiceMapping { get; set; }
        public virtual async Task<int> SaveChangesAsync(TeamEnum team = TeamEnum.NA, EventTypeEnum eventType = EventTypeEnum.NA, string actorId = null)
        {
            if (actorId !=null)
            {
                OnBeforeSaveChanges(team, eventType, actorId);
            }
            var result = await base.SaveChangesAsync();
            return result;
        }
        private void OnBeforeSaveChanges(TeamEnum team, EventTypeEnum eventType, string actorId)
        {
            ChangeTracker.DetectChanges();
            var eventEntries = new List<EventEntry>();
            foreach (var entry in ChangeTracker.Entries())
            {
                if (entry.Entity is Event || entry.State == EntityState.Detached || entry.State == EntityState.Unchanged)
                    continue;
                var eventEnty = new EventEntry(entry);
                eventEnty.TableName = entry.Entity.GetType().Name;
                eventEnty.ActorId = actorId;
                eventEnty.Team = team;
                eventEnty.EventType = eventType;
                eventEntries.Add(eventEnty);
                foreach (var property in entry.Properties)
                {
                    string propertyName = property.Metadata.Name;
                    if (property.Metadata.IsPrimaryKey())
                    {
                        eventEnty.KeyValues[propertyName] = property.CurrentValue;
                        continue;
                    }

                    switch (entry.State)
                    {
                        case EntityState.Added:
                            eventEnty.Action = ActionEnum.Create;
                            eventEnty.NewValues[propertyName] = property.CurrentValue;
                            break;

                        case EntityState.Deleted:
                            eventEnty.Action = ActionEnum.Delete;
                            eventEnty.OldValues[propertyName] = property.OriginalValue;
                            break;

                        case EntityState.Modified:
                            if (property.IsModified)
                            {
                                eventEnty.ChangedColumns.Add(propertyName);
                                eventEnty.Action = ActionEnum.Update;
                                eventEnty.OldValues[propertyName] = property.OriginalValue;
                                eventEnty.NewValues[propertyName] = property.CurrentValue;
                            }
                            break;
                    }
                }
            }
            foreach (var eventEntry in eventEntries)
            {
                EventLogs.Add(eventEntry.ToEventLogs());
            }
        }

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
               new Cab { Id =1, CabName = "EY", CreatedTime = new DateTime(2024, 9, 16, 0, 0, 0, DateTimeKind.Utc) },
               new Cab { Id =2, CabName = "DSIT", CreatedTime = new DateTime(2024, 9, 16, 0, 0, 0, DateTimeKind.Utc) },
               new Cab { Id =3, CabName = "ACCS", CreatedTime = new DateTime(2024, 9, 16, 0, 0, 0, DateTimeKind.Utc) },
               new Cab { Id =4, CabName = "Kantara", CreatedTime = new DateTime(2024, 9, 16, 0, 0, 0, DateTimeKind.Utc) },           
               new Cab { Id =6, CabName = "NQA", CreatedTime = new DateTime(2024, 9, 16, 0, 0, 0, DateTimeKind.Utc) },
               new Cab { Id = 7, CabName = "BSI", CreatedTime = new DateTime(2024, 9, 16, 0, 0, 0, DateTimeKind.Utc) });

            }
            else
            {
              modelBuilder.Entity<Cab>().HasData(
               new Cab { Id =1, CabName = "ACCS", CreatedTime = new DateTime(2024, 9, 16, 0, 0, 0, DateTimeKind.Utc) },
               new Cab { Id =2, CabName = "Kantara", CreatedTime = new DateTime(2024, 9, 16, 0, 0, 0, DateTimeKind.Utc) },
               new Cab { Id =3, CabName = "NQA", CreatedTime = new DateTime(2024, 9, 16, 0, 0, 0, DateTimeKind.Utc) },
               new Cab { Id =4, CabName = "BSI", CreatedTime = new DateTime(2024, 9, 16, 0, 0, 0, DateTimeKind.Utc) },
               new Cab { Id =5, CabName = "DSIT", CreatedTime = new DateTime(2024, 9, 16, 0, 0, 0, DateTimeKind.Utc) });
            }

           

            modelBuilder.Entity<CertificateReviewRejectionReason>().HasData(
            new CertificateReviewRejectionReason { Id =1, Reason = "Information is missing from the certificate" },
            new CertificateReviewRejectionReason { Id =2, Reason = "The certificate contains invalid information" },
            new CertificateReviewRejectionReason { Id =3, Reason = "The information submitted does not match the information on the certificate" },
            new CertificateReviewRejectionReason { Id =4, Reason = "The certificate or information submitted contains errors" },
            new CertificateReviewRejectionReason { Id =5, Reason = "Other" });

            modelBuilder.Entity<Role>().HasData(
            new Role { Id =1, RoleName = "Identity Service Provider (IDSP)",Order = 1 },
            new Role { Id =2, RoleName = "Attribute Service Provider (ASP)", Order = 2 },
            new Role { Id =3, RoleName = "Orchestration Service Provider (OSP)", Order = 3 });

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
                new SupplementaryScheme { Id =1, SchemeName = "Right to Work", Order = 2 },
                new SupplementaryScheme { Id =2, SchemeName = "Right to Rent", Order =1 },
                new SupplementaryScheme { Id =3, SchemeName = "Disclosure and Barring Service", Order =3 });

        }
    }
}
