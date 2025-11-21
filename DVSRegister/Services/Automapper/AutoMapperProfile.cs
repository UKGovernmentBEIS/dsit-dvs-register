using AutoMapper;
using DVSRegister.BusinessLogic.Models.CAB;
using DVSRegister.Models.CAB;

namespace DVSRegister.Services
{

    //For mapping view models to to dto that repeats across controllers
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<ServiceSummaryViewModel, ServiceDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ServiceId))
            .ForMember(dest => dest.WebSiteAddress, opt => opt.MapFrom(src => src.ServiceURL))
            .ForMember(dest => dest.TrustFrameworkVersionId, opt => opt.MapFrom(src => src.TFVersionViewModel.SelectedTFVersion.Id))
           .ForMember(dest => dest.ServiceRoleMapping, opt => opt.MapFrom(src =>
            src.RoleViewModel.SelectedRoles.Select(role => new ServiceRoleMappingDto { RoleId = role.Id })))
           .ForMember(dest => dest.ServiceQualityLevelMapping, opt => opt.MapFrom(src =>
            src.QualityLevelViewModel.SelectedQualityofAuthenticators.Select(q => new ServiceQualityLevelMappingDto { QualityLevelId = q.Id })
            .Concat(src.QualityLevelViewModel.SelectedLevelOfProtections.Select(q => new ServiceQualityLevelMappingDto { QualityLevelId = q.Id }))))
           .ForMember(dest => dest.ServiceIdentityProfileMapping, opt => opt.MapFrom(src =>
            src.IdentityProfileViewModel.SelectedIdentityProfiles.Select(profile => new ServiceIdentityProfileMappingDto { IdentityProfileId = profile.Id })))
           .ForMember(dest => dest.ServiceSupSchemeMapping, opt => opt.MapFrom(src =>
            src.SupplementarySchemeViewModel.SelectedSupplementarySchemes.Select(scheme => new ServiceSupSchemeMappingDto { SupplementarySchemeId = scheme.Id })))
           .ForMember(dest => dest.FileSizeInKb, opt => opt.MapFrom(src => src.FileSizeInKb ?? 0))            
           .ForMember(dest => dest.ConformityIssueDate, opt => opt.MapFrom(src => Convert.ToDateTime(src.ConformityIssueDate)))
           .ForMember(dest => dest.ConformityExpiryDate, opt => opt.MapFrom(src => Convert.ToDateTime(src.ConformityExpiryDate)))
            .ForMember(dest => dest.ServiceType, opt => opt.MapFrom(src => src.ServiceType));


            CreateMap<ProfileSummaryViewModel, ProviderProfileDto>()
           
                .ForMember(dest => dest.RegisteredName, opt => opt.MapFrom(src => src.RegisteredName))
                .ForMember(dest => dest.TradingName, opt => opt.MapFrom(src => src.TradingName))
                .ForMember(dest => dest.HasRegistrationNumber, opt => opt.MapFrom(src => src.HasRegistrationNumber))
                .ForMember(dest => dest.CompanyRegistrationNumber, opt => opt.MapFrom(src => src.CompanyRegistrationNumber))
                .ForMember(dest => dest.DUNSNumber, opt => opt.MapFrom(src => src.DUNSNumber))
                .ForMember(dest => dest.HasParentCompany, opt => opt.MapFrom(src => src.HasParentCompany))
                .ForMember(dest => dest.ParentCompanyRegisteredName, opt => opt.MapFrom(src => src.ParentCompanyRegisteredName))
                .ForMember(dest => dest.ParentCompanyLocation, opt => opt.MapFrom(src => src.ParentCompanyLocation))
                .ForMember(dest => dest.PrimaryContactFullName, opt => opt.MapFrom(src => src.PrimaryContact.PrimaryContactFullName))
                .ForMember(dest => dest.PrimaryContactJobTitle, opt => opt.MapFrom(src => src.PrimaryContact.PrimaryContactJobTitle))
                .ForMember(dest => dest.PrimaryContactEmail, opt => opt.MapFrom(src => src.PrimaryContact.PrimaryContactEmail))
                .ForMember(dest => dest.PrimaryContactTelephoneNumber, opt => opt.MapFrom(src => src.PrimaryContact.PrimaryContactTelephoneNumber))
                .ForMember(dest => dest.SecondaryContactFullName, opt => opt.MapFrom(src => src.SecondaryContact.SecondaryContactFullName))
                .ForMember(dest => dest.SecondaryContactJobTitle, opt => opt.MapFrom(src => src.SecondaryContact.SecondaryContactJobTitle))
                .ForMember(dest => dest.SecondaryContactEmail, opt => opt.MapFrom(src => src.SecondaryContact.SecondaryContactEmail))
                .ForMember(dest => dest.SecondaryContactTelephoneNumber, opt => opt.MapFrom(src => src.SecondaryContact.SecondaryContactTelephoneNumber))
                .ForMember(dest => dest.PublicContactEmail, opt => opt.MapFrom(src => src.PublicContactEmail))
                .ForMember(dest => dest.ProviderTelephoneNumber, opt => opt.MapFrom(src => src.ProviderTelephoneNumber))
                .ForMember(dest => dest.ProviderWebsiteAddress, opt => opt.MapFrom(src => src.ProviderWebsiteAddress))
                .ForMember(dest => dest.LinkToContactPage, opt => opt.MapFrom(src => src.LinkToContactPage))                  
               .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ProviderId));
        }
    }


   
}
