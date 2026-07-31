using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using DVSRegister.CommonUtility;
using DVSRegister.CommonUtility.Models;
using DVSRegister.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace DVSRegister.Data.Reports.RegisterHistory;

public enum RegisterHistoryActivityKind
{
    Publication = 1,
    ServiceUpdated = 2,
    ProviderUpdated = 3,
    Removed = 4,
    CabTransferred = 5,
    CustomDisplayUpdated = 6,
    CertificateExpired = 7
}

public static class PublishedRegisterEntryRevisionRecorder
{
    private const int FormatVersion = 1;
    private const string RightToWork = "Right to Work";
    private const string RightToRent = "Right to Rent";
    private const string Dbs = "DBS";

    public static string CreateSourceId(string operation, int entityId, params string?[] values)
    {
        var content = string.Join("\u001f", values.Select(value => value ?? string.Empty));
        var hash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(content)))[..24];
        return $"{operation}:{entityId}:{hash}";
    }

    public static async Task RecordAsync(DVSRegisterDbContext context, IEnumerable<int> serviceIds,
        RegisterHistoryActivityKind activityKind, string sourceType, string sourceId, string additionalInformation)
    {
        var ids = await CollectAffectedServiceIdsAsync(context, serviceIds);
        if (ids.Count > 0)
        {
            await RecordAffectedServicesAsync(context, ids, activityKind, sourceType, sourceId, additionalInformation);
        }
    }

    private static async Task RecordAffectedServicesAsync(DVSRegisterDbContext context, IReadOnlyCollection<int> serviceIds,
        RegisterHistoryActivityKind activityKind, string sourceType, string sourceId, string additionalInformation)
    {
        var services = await Query(context).Where(service => serviceIds.Contains(service.Id)).ToListAsync();
        var serviceKeys = services.Select(service => service.ServiceKey).Distinct().ToArray();
        var alreadyRecorded = await context.PublishedRegisterEntryRevisions
            .Where(revision => revision.SourceType == sourceType && revision.SourceId == sourceId &&
                               revision.ActivityKind == (int)activityKind && serviceKeys.Contains(revision.ServiceKey))
            .Select(revision => revision.ServiceKey)
            .ToListAsync();

        var now = DateTimeOffset.UtcNow;
        var servicesToRecord = services
            .Where(service => !alreadyRecorded.Contains(service.ServiceKey))
            .ToList();

        if (servicesToRecord.Count > 0)
        {
            var serviceKeysToRecord = servicesToRecord.Select(service => service.ServiceKey).ToArray();
            var previousRevisions = await context.PublishedRegisterEntryRevisions
                .AsNoTracking()
                .Where(revision => serviceKeysToRecord.Contains(revision.ServiceKey))
                .OrderByDescending(revision => revision.EffectiveAtUtc)
                .ThenByDescending(revision => revision.Id)
                .ToListAsync();

            var latestByServiceKey = previousRevisions
                .GroupBy(revision => revision.ServiceKey)
                .ToDictionary(group => group.Key, group => group.First());

            var revisions = servicesToRecord
                .Select(service => Map(service, activityKind, sourceType, sourceId,
                    AdditionalInformationFor(service, activityKind, sourceType, sourceId, additionalInformation, now,
                        latestByServiceKey), now))
                .ToList();

            context.PublishedRegisterEntryRevisions.AddRange(revisions);
            await context.SaveChangesAsync();
        }
    }

    private static string AdditionalInformationFor(
        Service service,
        RegisterHistoryActivityKind activityKind,
        string sourceType,
        string sourceId,
        string additionalInformation,
        DateTimeOffset now,
        IReadOnlyDictionary<int, PublishedRegisterEntryRevision> latestByServiceKey)
    {
        return IsFieldChangeActivity(activityKind)
            ? CreateChangeSummary(
                latestByServiceKey.GetValueOrDefault(service.ServiceKey),
                Map(service, activityKind, sourceType, sourceId, additionalInformation, now))
            : additionalInformation;
    }

    private static bool IsFieldChangeActivity(RegisterHistoryActivityKind activityKind) =>
        activityKind is RegisterHistoryActivityKind.ServiceUpdated or RegisterHistoryActivityKind.ProviderUpdated
            or RegisterHistoryActivityKind.CabTransferred;

    public static Task<List<int>> GetPublishedServiceIdsForProviderAsync(DVSRegisterDbContext context, int providerId) =>
        context.Service.Where(service => service.IsInRegister &&
                (service.ProviderProfileId == providerId ||
                 service.UnderPinningService.ProviderProfileId == providerId))
            .Select(service => service.Id).ToListAsync();

    private static async Task<List<int>> CollectAffectedServiceIdsAsync(DVSRegisterDbContext context,
        IEnumerable<int> serviceIds)
    {
        var roots = serviceIds.Distinct().ToArray();
        var dependents = roots.Length > 0
            ? await context.Service
                .Where(service => service.UnderPinningServiceId.HasValue &&
                                  roots.Contains(service.UnderPinningServiceId.Value) && service.IsInRegister)
                .Select(service => service.Id)
                .ToListAsync()
            : [];

        return roots.Concat(dependents).Distinct().ToList();
    }

    private static IQueryable<Service> Query(DVSRegisterDbContext context) =>
        context.Service.AsNoTracking().AsSplitQuery()
            .Include(service => service.Provider)
            .Include(service => service.CabUser).ThenInclude(cabUser => cabUser.Cab)
            .Include(service => service.TrustFrameworkVersion)
            .Include(service => service.ServiceRoleMapping!).ThenInclude(mapping => mapping.Role)
            .Include(service => service.ServiceIdentityProfileMapping!).ThenInclude(mapping => mapping.IdentityProfile)
            .Include(service => service.ServiceQualityLevelMapping!).ThenInclude(mapping => mapping.QualityLevel)
            .Include(service => service.ServiceSupSchemeMapping!.Where(mapping => mapping.ServiceSupSchemeCustomDisplayId == null))
                .ThenInclude(mapping => mapping.SupplementaryScheme)
            .Include(service => service.ServiceSupSchemeMapping!.Where(mapping => mapping.ServiceSupSchemeCustomDisplayId == null))
                .ThenInclude(mapping => mapping.SchemeGPG44Mapping!).ThenInclude(mapping => mapping.QualityLevel)
            .Include(service => service.ServiceSupSchemeMapping!.Where(mapping => mapping.ServiceSupSchemeCustomDisplayId == null))
                .ThenInclude(mapping => mapping.SchemeGPG45Mapping!).ThenInclude(mapping => mapping.IdentityProfile)
            .Include(service => service.UnderPinningService).ThenInclude(underpinning => underpinning.Provider)
            .Include(service => service.UnderPinningService).ThenInclude(underpinning => underpinning.CabUser)
                .ThenInclude(cabUser => cabUser.Cab)
            .Include(service => service.ManualUnderPinningService).ThenInclude(underpinning => underpinning.Cab);

    private static PublishedRegisterEntryRevision Map(Service service, RegisterHistoryActivityKind activityKind,
        string sourceType, string sourceId, string additionalInformation, DateTimeOffset now)
    {
        var rightToWork = FindScheme(service, RightToWork);
        var rightToRent = FindScheme(service, RightToRent);
        var dbs = FindScheme(service, Dbs);
        var linked = service.UnderPinningService;
        var manual = service.ManualUnderPinningService;

        return new PublishedRegisterEntryRevision
        {
            ServiceKey = service.ServiceKey, ProviderProfileId = service.ProviderProfileId, ServiceId = service.Id,
            ServiceVersion = service.ServiceVersion, EffectiveAtUtc = now, IsInRegister = service.IsInRegister,
            ActivityKind = (int)activityKind, SourceType = sourceType, SourceId = sourceId,
            AdditionalInformation = additionalInformation.Length <= 4000 ? additionalInformation : additionalInformation[..4000],
            FormatVersion = FormatVersion, CreatedAtUtc = now,
            Provider = service.Provider.RegisteredName, Cab = service.CabUser.Cab?.CabName ?? string.Empty,
            ServiceName = service.ServiceName ?? string.Empty,
            PublicationType = service.ServiceVersion == 1 ? Constants.NewApplication : Constants.ReApplication,
            CompanyAddress = service.CompanyAddress, PublicEmailAddress = service.Provider.PublicContactEmail,
            PublicTelephoneNumber = service.Provider.ProviderTelephoneNumber, WebsiteAddress = service.WebSiteAddress,
            SubmittedOn = service.ResubmissionTime ?? service.CreatedTime, PublishedOn = service.PublishedTime,
            RemovedOn = service.RemovedTime, TrustFrameworkVersion = service.TrustFrameworkVersion.Version.ToString("0.0", CultureInfo.InvariantCulture),
            RolesCertifiedAgainst = Join(service.ServiceRoleMapping?.OrderBy(x => x.Role.Order).Select(x => x.Role.RoleName)),
            IsUnderpinningOrWhiteLabelledService = service.ServiceType.HasValue,
            IsCertifiedAgainstGpg45Profiles = service.HasGPG45,
            IdentityProfiles = Join(service.ServiceIdentityProfileMapping?.OrderBy(x => x.IdentityProfile.IdentityProfileName).Select(x => x.IdentityProfile.IdentityProfileName)),
            IsCertifiedAgainstGpg44 = service.HasGPG44,
            Gpg44AuthenticationQualities = JoinQualityLevels(service.ServiceQualityLevelMapping, QualityTypeEnum.Authentication),
            Gpg44ProtectionQualities = JoinQualityLevels(service.ServiceQualityLevelMapping, QualityTypeEnum.Protection),
            IsCertifiedAgainstSupplementaryCodes = service.HasSupplementarySchemes,
            SupplementaryCodes = Join(service.ServiceSupSchemeMapping?.OrderBy(x => x.SupplementaryScheme?.Order).Select(x => x.SupplementaryScheme?.SchemeName)),
            CertificateIssueDate = service.ConformityIssueDate, CertificateExpiryDate = service.ConformityExpiryDate,
            RightToWorkIdentityProfiles = JoinSchemeIdentityProfiles(rightToWork),
            IsRightToWorkCertifiedAgainstGpg44 = rightToWork?.HasGpg44Mapping,
            RightToWorkAuthenticationQualities = JoinSchemeQualityLevels(rightToWork, QualityTypeEnum.Authentication),
            RightToWorkProtectionQualities = JoinSchemeQualityLevels(rightToWork, QualityTypeEnum.Protection),
            RightToRentIdentityProfiles = JoinSchemeIdentityProfiles(rightToRent),
            IsRightToRentCertifiedAgainstGpg44 = rightToRent?.HasGpg44Mapping,
            RightToRentAuthenticationQualities = JoinSchemeQualityLevels(rightToRent, QualityTypeEnum.Authentication),
            RightToRentProtectionQualities = JoinSchemeQualityLevels(rightToRent, QualityTypeEnum.Protection),
            DbsIdentityProfiles = JoinSchemeIdentityProfiles(dbs), IsDbsCertifiedAgainstGpg44 = dbs?.HasGpg44Mapping,
            DbsAuthenticationQualities = JoinSchemeQualityLevels(dbs, QualityTypeEnum.Authentication),
            DbsProtectionQualities = JoinSchemeQualityLevels(dbs, QualityTypeEnum.Protection),
            UnderpinningServiceName = linked.ServiceName ?? manual.ServiceName,
            UnderpinningProviderName = linked.Provider.RegisteredName,
            UnderpinningCab = linked.CabUser.Cab?.CabName ?? manual.Cab.CabName,
            UnderpinningCertificateExpiryDate = linked.ConformityExpiryDate ?? manual.CertificateExpiryDate
        };
    }

    private static ServiceSupSchemeMapping? FindScheme(Service service, string name) =>
        service.ServiceSupSchemeMapping?.SingleOrDefault(x => string.Equals(x.SupplementaryScheme?.SchemeName, name, StringComparison.OrdinalIgnoreCase));

    private static string CreateChangeSummary(PublishedRegisterEntryRevision? previous,
        PublishedRegisterEntryRevision current)
    {
        var changes = new[]
            {
                Change("Provider", previous?.Provider, current.Provider),
                Change("CAB", previous?.Cab, current.Cab),
                Change("Service name", previous?.ServiceName, current.ServiceName),
                Change("Publication type", previous?.PublicationType, current.PublicationType),
                Change("Company address", previous?.CompanyAddress, current.CompanyAddress),
                Change("Public email", previous?.PublicEmailAddress, current.PublicEmailAddress),
                Change("Public telephone", previous?.PublicTelephoneNumber, current.PublicTelephoneNumber),
                Change("Website", previous?.WebsiteAddress, current.WebsiteAddress),
                Change("Submitted on", previous?.SubmittedOn, current.SubmittedOn),
                Change("Published on", previous?.PublishedOn, current.PublishedOn),
                Change("Removed on", previous?.RemovedOn, current.RemovedOn),
                Change("Trust framework version", previous?.TrustFrameworkVersion, current.TrustFrameworkVersion),
                Change("Roles certified against", previous?.RolesCertifiedAgainst, current.RolesCertifiedAgainst),
                Change("Underpinning or white-labelled service", previous?.IsUnderpinningOrWhiteLabelledService, current.IsUnderpinningOrWhiteLabelledService),
                Change("Certified against GPG 45 profiles", previous?.IsCertifiedAgainstGpg45Profiles, current.IsCertifiedAgainstGpg45Profiles),
                Change("Identity profiles", previous?.IdentityProfiles, current.IdentityProfiles),
                Change("Certified against GPG 44", previous?.IsCertifiedAgainstGpg44, current.IsCertifiedAgainstGpg44),
                Change("GPG 44 authentication qualities", previous?.Gpg44AuthenticationQualities, current.Gpg44AuthenticationQualities),
                Change("GPG 44 protection qualities", previous?.Gpg44ProtectionQualities, current.Gpg44ProtectionQualities),
                Change("Certified against supplementary codes", previous?.IsCertifiedAgainstSupplementaryCodes, current.IsCertifiedAgainstSupplementaryCodes),
                Change("Supplementary codes", previous?.SupplementaryCodes, current.SupplementaryCodes),
                Change("Certificate issue date", previous?.CertificateIssueDate, current.CertificateIssueDate),
                Change("Certificate expiry date", previous?.CertificateExpiryDate, current.CertificateExpiryDate),
                Change("Right to Work identity profiles", previous?.RightToWorkIdentityProfiles, current.RightToWorkIdentityProfiles),
                Change("Right to Work GPG 44", previous?.IsRightToWorkCertifiedAgainstGpg44, current.IsRightToWorkCertifiedAgainstGpg44),
                Change("Right to Work authentication qualities", previous?.RightToWorkAuthenticationQualities, current.RightToWorkAuthenticationQualities),
                Change("Right to Work protection qualities", previous?.RightToWorkProtectionQualities, current.RightToWorkProtectionQualities),
                Change("Right to Rent identity profiles", previous?.RightToRentIdentityProfiles, current.RightToRentIdentityProfiles),
                Change("Right to Rent GPG 44", previous?.IsRightToRentCertifiedAgainstGpg44, current.IsRightToRentCertifiedAgainstGpg44),
                Change("Right to Rent authentication qualities", previous?.RightToRentAuthenticationQualities, current.RightToRentAuthenticationQualities),
                Change("Right to Rent protection qualities", previous?.RightToRentProtectionQualities, current.RightToRentProtectionQualities),
                Change("DBS identity profiles", previous?.DbsIdentityProfiles, current.DbsIdentityProfiles),
                Change("DBS GPG 44", previous?.IsDbsCertifiedAgainstGpg44, current.IsDbsCertifiedAgainstGpg44),
                Change("DBS authentication qualities", previous?.DbsAuthenticationQualities, current.DbsAuthenticationQualities),
                Change("DBS protection qualities", previous?.DbsProtectionQualities, current.DbsProtectionQualities),
                Change("Underpinning service", previous?.UnderpinningServiceName, current.UnderpinningServiceName),
                Change("Underpinning provider", previous?.UnderpinningProviderName, current.UnderpinningProviderName),
                Change("Underpinning CAB", previous?.UnderpinningCab, current.UnderpinningCab),
                Change("Underpinning certificate expiry date", previous?.UnderpinningCertificateExpiryDate, current.UnderpinningCertificateExpiryDate),
                Change("In register", previous?.IsInRegister, current.IsInRegister)
            }
            .OfType<string>();

        return Truncate(ChangeSummary(previous, changes));
    }

    private static string? Change(string label, object? previous, object? current) =>
        string.Equals(Display(previous), Display(current), StringComparison.Ordinal)
            ? null
            : $"{label}: {Display(previous)} → {Display(current)}";

    private static string ChangeSummary(PublishedRegisterEntryRevision? previous, IEnumerable<string> changes)
    {
        var changeList = changes.ToList();
        var summaryPrefix = previous is null ? "Previously unrecorded state" : "Changes";

        return changeList.Any()
            ? $"{summaryPrefix}: {string.Join("; ", changeList)}"
            : "Changes: no visible report fields changed.";
    }

    private static string Truncate(string summary) => summary.Length <= 4000 ? summary : summary[..4000];

    private static string Display(object? value) => value switch
    {
        null => "(not recorded)",
        DateOnly date => date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
        DateTime date => date.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture),
        bool boolean => boolean ? "Yes" : "No",
        _ => value.ToString() ?? "(not recorded)"
    };

    private static string Join(IEnumerable<string?>? values) => string.Join("; ", values?.Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x!) ?? []);
    private static string JoinQualityLevels(IEnumerable<ServiceQualityLevelMapping>? mappings, QualityTypeEnum type) =>
        Join(mappings?.Where(x => x.QualityLevel.QualityType == type).OrderBy(x => x.QualityLevel.Level).Select(x => x.QualityLevel.Level));
    private static string JoinSchemeIdentityProfiles(ServiceSupSchemeMapping? scheme) =>
        Join(scheme?.SchemeGPG45Mapping?.OrderBy(x => x.IdentityProfile.IdentityProfileName).Select(x => x.IdentityProfile.IdentityProfileName));
    private static string JoinSchemeQualityLevels(ServiceSupSchemeMapping? scheme, QualityTypeEnum type) =>
        Join(scheme?.SchemeGPG44Mapping?.Where(x => x.QualityLevel.QualityType == type).OrderBy(x => x.QualityLevel.Level).Select(x => x.QualityLevel.Level));
}
