using System.ComponentModel.DataAnnotations;

namespace DVSRegister.Data.Entities;

public sealed class PublishedRegisterEntryRevision
{
    public long Id { get; set; }
    public int ServiceKey { get; set; }
    public int ProviderProfileId { get; set; }
    public int ServiceId { get; set; }
    public int ServiceVersion { get; set; }
    public DateTimeOffset EffectiveAtUtc { get; set; }
    public bool IsInRegister { get; set; }
    public int ActivityKind { get; set; }
    [MaxLength(64)]
    public string SourceType { get; set; } = string.Empty;
    [MaxLength(128)]
    public string SourceId { get; set; } = string.Empty;
    [MaxLength(4000)]
    public string AdditionalInformation { get; set; } = string.Empty;
    public int FormatVersion { get; set; }
    public DateTimeOffset CreatedAtUtc { get; set; }

    [MaxLength(512)]
    public string Provider { get; set; } = string.Empty;
    [MaxLength(512)]
    public string Cab { get; set; } = string.Empty;
    [MaxLength(512)]
    public string ServiceName { get; set; } = string.Empty;
    [MaxLength(128)]
    public string? PublicationType { get; set; }
    [MaxLength(2000)]
    public string? CompanyAddress { get; set; }
    [MaxLength(320)]
    public string? PublicEmailAddress { get; set; }
    [MaxLength(64)]
    public string? PublicTelephoneNumber { get; set; }
    [MaxLength(2048)]
    public string? WebsiteAddress { get; set; }
    public DateOnly? SubmittedOn { get; set; }
    public DateOnly? PublishedOn { get; set; }
    public DateOnly? RemovedOn { get; set; }
    [MaxLength(64)]
    public string? TrustFrameworkVersion { get; set; }
    [MaxLength(4000)]
    public string RolesCertifiedAgainst { get; set; } = string.Empty;
    public bool? IsUnderpinningOrWhiteLabelledService { get; set; }
    public bool? IsCertifiedAgainstGpg45Profiles { get; set; }
    [MaxLength(4000)]
    public string IdentityProfiles { get; set; } = string.Empty;
    public bool? IsCertifiedAgainstGpg44 { get; set; }
    [MaxLength(4000)]
    public string Gpg44AuthenticationQualities { get; set; } = string.Empty;
    [MaxLength(4000)]
    public string Gpg44ProtectionQualities { get; set; } = string.Empty;
    public bool? IsCertifiedAgainstSupplementaryCodes { get; set; }
    [MaxLength(4000)]
    public string SupplementaryCodes { get; set; } = string.Empty;
    public DateOnly? CertificateIssueDate { get; set; }
    public DateOnly? CertificateExpiryDate { get; set; }
    [MaxLength(4000)]
    public string RightToWorkIdentityProfiles { get; set; } = string.Empty;
    public bool? IsRightToWorkCertifiedAgainstGpg44 { get; set; }
    [MaxLength(4000)]
    public string RightToWorkAuthenticationQualities { get; set; } = string.Empty;
    [MaxLength(4000)]
    public string RightToWorkProtectionQualities { get; set; } = string.Empty;
    [MaxLength(4000)]
    public string RightToRentIdentityProfiles { get; set; } = string.Empty;
    public bool? IsRightToRentCertifiedAgainstGpg44 { get; set; }
    [MaxLength(4000)]
    public string RightToRentAuthenticationQualities { get; set; } = string.Empty;
    [MaxLength(4000)]
    public string RightToRentProtectionQualities { get; set; } = string.Empty;
    [MaxLength(4000)]
    public string DbsIdentityProfiles { get; set; } = string.Empty;
    public bool? IsDbsCertifiedAgainstGpg44 { get; set; }
    [MaxLength(4000)]
    public string DbsAuthenticationQualities { get; set; } = string.Empty;
    [MaxLength(4000)]
    public string DbsProtectionQualities { get; set; } = string.Empty;
    [MaxLength(512)]
    public string? UnderpinningServiceName { get; set; }
    [MaxLength(512)]
    public string? UnderpinningProviderName { get; set; }
    [MaxLength(512)]
    public string? UnderpinningCab { get; set; }
    public DateOnly? UnderpinningCertificateExpiryDate { get; set; }
}
