
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

        public DbSet<DataProtectionKey> DataProtectionKeys { get; set; }
        public DbSet<TrustmarkNumber> TrustmarkNumber { get; set; }
        public DbSet<Event> EventLogs { get; set; }
     

        public DbSet<ProviderProfileDraft> ProviderProfileDraft { get; set; }
        public DbSet<ProviderDraftToken> ProviderDraftToken { get; set; }
        public DbSet<ServiceDraft> ServiceDraft { get; set; }
     
        public DbSet<ServiceRoleMappingDraft> ServiceRoleMappingDraft { get; set; }
        public DbSet<ServiceQualityLevelMappingDraft> ServiceQualityLevelMappingDraft { get; set; }
        public DbSet<ServiceIdentityProfileMappingDraft> ServiceIdentityProfileMappingDraft { get; set; }
        public DbSet<ServiceSupSchemeMappingDraft> ServiceSupSchemeMappingDraft { get; set; }

        public DbSet<CabTransferRequest> CabTransferRequest { get; set; }
        public DbSet<RequestManagement> RequestManagement { get; set; }

        public DbSet<ProviderProfileCabMapping> ProviderProfileCabMapping { get; set; }        
        public DbSet<TrustFrameworkVersion> TrustFrameworkVersion { get; set; }
        public DbSet<SchemeGPG44Mapping> SchemeGPG44Mapping { get; set; }
        public DbSet<SchemeGPG45Mapping> SchemeGPG45Mapping { get; set; }
        public DbSet<ManualUnderPinningService> ManualUnderPinningService { get; set; }

        public DbSet<SchemeGPG45MappingDraft> SchemeGPG45MappingDraft { get; set; }
        public DbSet<SchemeGPG44MappingDraft> SchemeGPG44MappingDraft { get; set; }
        public DbSet<ManualUnderPinningServiceDraft> ManualUnderPinningServiceDraft { get; set; }
        public DbSet<ProviderRemovalRequest> ProviderRemovalRequest { get; set; }
        public DbSet<ServiceRemovalRequest> ServiceRemovalRequest { get; set; }

        public DbSet<ActionCategory> ActionCategory { get; set; }
        public DbSet<ActionDetails> ActionDetails { get; set; }
        public DbSet<ActionLogs> ActionLogs { get; set; }

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

            modelBuilder.Entity<ActionCategory>()
            .HasIndex(p => new { p.ActionKey })
            .IsUnique(true);

            modelBuilder.Entity<ActionDetails>()
            .HasIndex(p => new { p.ActionDetailsKey })
            .IsUnique(true);

            modelBuilder.Entity<ActionLogs>(entity =>
            {
                entity.Property(e => e.LogDate).HasColumnType("date");
            });


            modelBuilder.Entity<ServiceDraft>(b =>
            {
                b.HasOne(sd => sd.Service)
                .WithOne(s => s.ServiceDraft)
                .HasForeignKey<ServiceDraft>(sd => sd.ServiceId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

                b.HasOne(sd => sd.User)
                .WithMany()
                .HasForeignKey(sd => sd.RequestedUserId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

                b.Navigation("Service");
                b.Navigation("User");
            });



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
            new Role { Id =3, RoleName = "Orchestration Service Provider (OSP)", Order = 3 },
            new Role { Id = 4, RoleName = "Holder Service Provider (HSP)", Order = 4 },
            new Role { Id = 5, RoleName = "Component Service Provider (CSP)", Order = 5 });

                modelBuilder.Entity<IdentityProfile>().HasData(
                new IdentityProfile { Id =1, IdentityProfileName = "L1A ",IdentityProfileType = IdentityProfileTypeEnum.Low },
                new IdentityProfile { Id =2, IdentityProfileName = "L1B ", IdentityProfileType = IdentityProfileTypeEnum.Low },
                new IdentityProfile { Id =3, IdentityProfileName = "L1C ", IdentityProfileType = IdentityProfileTypeEnum.Low },
                new IdentityProfile { Id =4, IdentityProfileName = "L2A ", IdentityProfileType = IdentityProfileTypeEnum.Low },
                new IdentityProfile { Id =5, IdentityProfileName = "L2B ", IdentityProfileType = IdentityProfileTypeEnum.Low },
                new IdentityProfile { Id =6, IdentityProfileName = "L3A ", IdentityProfileType = IdentityProfileTypeEnum.Low },
                new IdentityProfile { Id =7, IdentityProfileName = "M1A ", IdentityProfileType = IdentityProfileTypeEnum.Medium },
                new IdentityProfile { Id =8, IdentityProfileName = "M1B ", IdentityProfileType = IdentityProfileTypeEnum.Medium },
                new IdentityProfile { Id =9, IdentityProfileName = "M1C ", IdentityProfileType = IdentityProfileTypeEnum.Medium },
                new IdentityProfile { Id =10, IdentityProfileName = "M1D ", IdentityProfileType = IdentityProfileTypeEnum.Medium },
                new IdentityProfile { Id =11, IdentityProfileName = "M2A ", IdentityProfileType = IdentityProfileTypeEnum.Medium },
                new IdentityProfile { Id =12, IdentityProfileName = "M2B ", IdentityProfileType = IdentityProfileTypeEnum.Medium },
                new IdentityProfile { Id =13, IdentityProfileName = "M2C ", IdentityProfileType = IdentityProfileTypeEnum.Medium },
                new IdentityProfile { Id =14, IdentityProfileName = "M3A ", IdentityProfileType = IdentityProfileTypeEnum.Medium },
                new IdentityProfile { Id =15, IdentityProfileName = "H1A ", IdentityProfileType = IdentityProfileTypeEnum.High },
                new IdentityProfile { Id =16, IdentityProfileName = "H1B ", IdentityProfileType = IdentityProfileTypeEnum.High },
                new IdentityProfile { Id =17, IdentityProfileName = "H1C ", IdentityProfileType = IdentityProfileTypeEnum.High },
                new IdentityProfile { Id =18, IdentityProfileName = "H2A ", IdentityProfileType = IdentityProfileTypeEnum.High },
                new IdentityProfile { Id =19, IdentityProfileName = "H2B ", IdentityProfileType = IdentityProfileTypeEnum.High },
                new IdentityProfile { Id =20, IdentityProfileName = "H2C ", IdentityProfileType = IdentityProfileTypeEnum.High },
                new IdentityProfile { Id =21, IdentityProfileName = "H2D ", IdentityProfileType = IdentityProfileTypeEnum.High },
                new IdentityProfile { Id =22, IdentityProfileName = "H2E ", IdentityProfileType = IdentityProfileTypeEnum.High },
                new IdentityProfile { Id =23, IdentityProfileName = "H3A ", IdentityProfileType = IdentityProfileTypeEnum.High },
                new IdentityProfile { Id =24, IdentityProfileName = "V1A ", IdentityProfileType = IdentityProfileTypeEnum.VeryHigh },
                new IdentityProfile { Id =25, IdentityProfileName = "V1B ", IdentityProfileType = IdentityProfileTypeEnum.VeryHigh },
                new IdentityProfile { Id =26, IdentityProfileName = "V1C ", IdentityProfileType = IdentityProfileTypeEnum.VeryHigh },
                new IdentityProfile { Id =27, IdentityProfileName = "V1D ", IdentityProfileType = IdentityProfileTypeEnum.VeryHigh },
                new IdentityProfile { Id =28, IdentityProfileName = "V2A ", IdentityProfileType = IdentityProfileTypeEnum.VeryHigh },
                new IdentityProfile { Id =29, IdentityProfileName = "V2B ", IdentityProfileType = IdentityProfileTypeEnum.VeryHigh },
                new IdentityProfile { Id =30, IdentityProfileName = "V2C ", IdentityProfileType = IdentityProfileTypeEnum.VeryHigh },
                new IdentityProfile { Id =31, IdentityProfileName = "V2D ", IdentityProfileType = IdentityProfileTypeEnum.VeryHigh },
                new IdentityProfile { Id =32, IdentityProfileName = "V3A ", IdentityProfileType = IdentityProfileTypeEnum.VeryHigh });

                modelBuilder.Entity<SupplementaryScheme>().HasData(
                new SupplementaryScheme { Id =1, SchemeName = "Right to Work", Order = 1 },
                new SupplementaryScheme { Id =2, SchemeName = "Right to Rent", Order = 2 },
                new SupplementaryScheme { Id =3, SchemeName = "Disclosure and Barring Service", Order =3 });

                modelBuilder.Entity<TrustFrameworkVersion>().HasData(
                new TrustFrameworkVersion { Id = 1, TrustFrameworkName = "0.4 gamma", Order = 1 },
                new TrustFrameworkVersion { Id = 2, TrustFrameworkName = "0.3 beta", Order = 2 });

                modelBuilder.Entity<ActionCategory>().HasData(
                new ActionCategory { Id = 1, ActionKey = nameof(ActionCategoryEnum.CR), ActionName = "Certificate review" },
                new ActionCategory { Id = 2, ActionKey = nameof(ActionCategoryEnum.PI), ActionName = "Public interest checks" },
                new ActionCategory { Id = 3, ActionKey = nameof(ActionCategoryEnum.ServiceUpdates), ActionName = "Service updates" },
                new ActionCategory { Id = 4, ActionKey = nameof(ActionCategoryEnum.ProviderUpdates), ActionName = "Provider updates" });

                modelBuilder.Entity<ActionDetails>().HasData(
                new ActionDetails { Id = 1, ActionDetailsKey = nameof(ActionDetailsEnum.CR_APR), ActionDescription = "Passed", ActionCategoryId = 1},
                new ActionDetails { Id = 2, ActionDetailsKey = nameof(ActionDetailsEnum.CR_Rej), ActionDescription = "Rejected", ActionCategoryId = 1 },
                new ActionDetails { Id = 3, ActionDetailsKey = nameof(ActionDetailsEnum.CR_Restore), ActionDescription = "Restored", ActionCategoryId = 1 },
                new ActionDetails { Id = 4, ActionDetailsKey = nameof(ActionDetailsEnum.CR_SentBack), ActionDescription = "Sent back to CAB", ActionCategoryId = 1},
                new ActionDetails { Id = 5, ActionDetailsKey = nameof(ActionDetailsEnum.CR_DeclinedByProvider), ActionDescription = "Declined by provider", ActionCategoryId = 1 },
                new ActionDetails { Id = 6, ActionDetailsKey = nameof(ActionDetailsEnum.PI_Primary_Pass), ActionDescription = "Primary check passed", ActionCategoryId = 2 },
                new ActionDetails { Id = 7, ActionDetailsKey = nameof(ActionDetailsEnum.PI_SentBack), ActionDescription = "Sent back by second reviewer", ActionCategoryId = 2 },
                new ActionDetails { Id = 8, ActionDetailsKey = nameof(ActionDetailsEnum.PI_Primary_Fail), ActionDescription = "Primary check failed", ActionCategoryId = 2 },
                new ActionDetails { Id = 9, ActionDetailsKey = nameof(ActionDetailsEnum.PI_Fail), ActionDescription = "Application rejected", ActionCategoryId = 2 },
                new ActionDetails { Id = 10, ActionDetailsKey = nameof(ActionDetailsEnum.PI_ProviderPublish), ActionDescription = "Publication of provider", ActionCategoryId = 2 },
                new ActionDetails { Id = 11, ActionDetailsKey = nameof(ActionDetailsEnum.PI_ServicePublish), ActionDescription = "Service published", ActionCategoryId = 2 },
                new ActionDetails { Id = 12, ActionDetailsKey = nameof(ActionDetailsEnum.PI_ServiceRePublish), ActionDescription = "Service updated", ActionCategoryId = 2 },
                new ActionDetails { Id = 13, ActionDetailsKey = nameof(ActionDetailsEnum.ServiceNameUpdate), ActionDescription = "Service name changed", ActionCategoryId = 3 },
                new ActionDetails { Id = 14, ActionDetailsKey = nameof(ActionDetailsEnum.ServiceUpdates), ActionDescription = "Updates published", ActionCategoryId = 3 },
                new ActionDetails { Id = 15, ActionDetailsKey = nameof(ActionDetailsEnum.ProviderContactUpdate), ActionDescription = "Contact details changed", ActionCategoryId = 4 },
                new ActionDetails { Id = 16, ActionDetailsKey = nameof(ActionDetailsEnum.BusinessDetailsUpdate), ActionDescription = "Business details changed", ActionCategoryId = 4 }             

            );

        }
    }
}
