using AutoMapper;
using DVSRegister.BusinessLogic.Automapper;
using DVSRegister.BusinessLogic.Models;
using DVSRegister.BusinessLogic.Models.CAB;
using DVSRegister.BusinessLogic.Models.Register;
using DVSRegister.BusinessLogic.Models.Remove2i;
using DVSRegister.BusinessLogic.Remove2i;
using DVSRegister.BusinessLogic.Users;
using DVSRegister.Data.Entities;

namespace DVSRegister.BusinessLogic
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
          
            CreateMap<Role, RoleDto>();
            CreateMap<RoleDto, Role>();
            CreateMap<IdentityProfile, IdentityProfileDto>();
            CreateMap<IdentityProfileDto, IdentityProfile>();
            CreateMap<SupplementaryScheme, SupplementarySchemeDto>();
            CreateMap<SupplementarySchemeDto, SupplementaryScheme>();
            CreateMap<QualityLevel, QualityLevelDto>();
            CreateMap<QualityLevelDto, QualityLevel>();

            CreateMap<ServiceIdentityProfileMapping, ServiceIdentityProfileMappingDto>();
            CreateMap<ServiceIdentityProfileMappingDto, ServiceIdentityProfileMapping>();
            CreateMap<ServiceRoleMapping, ServiceRoleMappingDto>();
            CreateMap<ServiceRoleMappingDto, ServiceRoleMapping>();

            CreateMap<SchemeGPG44Mapping, SchemeGPG44MappingDto>();
            CreateMap<SchemeGPG44MappingDto, SchemeGPG44Mapping>();

            CreateMap<SchemeGPG45Mapping, SchemeGPG45MappingDto>();
            CreateMap<SchemeGPG45MappingDto, SchemeGPG45Mapping>();

            CreateMap<ServiceSupSchemeMapping, ServiceSupSchemeMappingDto>()
            .ForMember(dest => dest.SchemeGPG44Mapping, opt => opt.MapFrom(src =>src.SchemeGPG44Mapping));
            CreateMap<ServiceSupSchemeMappingDto, ServiceSupSchemeMapping>()
            .ForMember(dest => dest.SchemeGPG45Mapping, opt => opt.MapFrom(src => src.SchemeGPG45Mapping));
            CreateMap<ServiceQualityLevelMapping, ServiceQualityLevelMappingDto>();
            CreateMap<ServiceQualityLevelMappingDto, ServiceQualityLevelMapping>();         
          

           
           

            CreateMap<RegisterPublishLog, RegisterPublishLogDto>();
            CreateMap<RegisterPublishLogDto, RegisterPublishLog>();

            CreateMap<ProviderProfileCabMapping, ProviderProfileCabMappingDto>();
            CreateMap<ProviderProfileCabMappingDto, ProviderProfileCabMapping>();


            CreateMap<ProviderProfile, ProviderProfileDto>()
            .ForMember(dest => dest.Services, opt => opt.MapFrom(src => src.Services))
            .ForMember(dest => dest.ProviderProfileCabMapping, opt => opt.MapFrom(src => src.ProviderProfileCabMapping))
            .ForMember(dest => dest.LastUpdatedInfo, opt => opt.MapFrom<LastModifiedDateResolver>());
            CreateMap<ProviderProfileDto, ProviderProfile>()
            .ForMember(dest => dest.ProviderProfileCabMapping, opt => opt.MapFrom(src => src.ProviderProfileCabMapping))
            .ForMember(dest => dest.Services, opt => opt.MapFrom(src => src.Services));

            CreateMap<Cab, CabDto>();
            CreateMap<CabDto, Cab>();
            CreateMap<CabUser, CabUserDto>();
            CreateMap<CabUserDto, CabUser>();

            CreateMap<PublicInterestCheck, PublicInterestCheckDto>();
            CreateMap<PublicInterestCheckDto, PublicInterestCheck>();

            CreateMap<CertificateReview, CertificateReviewDto>();
            CreateMap<CertificateReviewDto, CertificateReview>();

            CreateMap<Service, ServiceDto>()
            .ForMember(dest => dest.ServiceQualityLevelMapping, opt => opt.MapFrom(src => src.ServiceQualityLevelMapping))
            .ForMember(dest => dest.ServiceRoleMapping, opt => opt.MapFrom(src => src.ServiceRoleMapping))
            .ForMember(dest => dest.ServiceIdentityProfileMapping, opt => opt.MapFrom(src => src.ServiceIdentityProfileMapping))
            .ForMember(dest => dest.ServiceSupSchemeMapping, opt => opt.MapFrom(src => src.ServiceSupSchemeMapping))
            .ForMember(dest => dest.ServiceQualityLevelMapping, opt => opt.MapFrom(src => src.ServiceQualityLevelMapping))
            .ForMember(dest => dest.Provider, opt => opt.MapFrom(src => src.Provider))
            .ForMember(dest => dest.CertificateReview, opt => opt.MapFrom(src => src.CertificateReview))
            .ForMember(dest => dest.PublicInterestCheck, opt => opt.MapFrom(src => src.PublicInterestCheck))
            .ForMember(dest => dest.CabTransferRequest, opt => opt.MapFrom(src => src.CabTransferRequest))
            .ForMember(dest => dest.CreatedTime, opt => opt.MapFrom<CreatedTimeResolver>())
            .ForMember(dest => dest.NewOrResubmission, opt => opt.MapFrom<NewOrResubmissionResolver>())
            .ForMember(dest => dest.TrustFrameworkVersion, opt => opt.MapFrom(src => src.TrustFrameworkVersion));

            CreateMap<ServiceDto, Service>()
           .ForMember(dest => dest.ServiceQualityLevelMapping, opt => opt.MapFrom(src => src.ServiceQualityLevelMapping))
           .ForMember(dest => dest.ServiceRoleMapping, opt => opt.MapFrom(src => src.ServiceRoleMapping))
           .ForMember(dest => dest.ServiceIdentityProfileMapping, opt => opt.MapFrom(src => src.ServiceIdentityProfileMapping))
           .ForMember(dest => dest.ServiceSupSchemeMapping, opt => opt.MapFrom(src => src.ServiceSupSchemeMapping))
           .ForMember(dest => dest.ServiceQualityLevelMapping, opt => opt.MapFrom(src => src.ServiceQualityLevelMapping))
           .ForMember(dest => dest.Provider, opt => opt.MapFrom(src => src.Provider))
           .ForMember(dest => dest.CertificateReview, opt => opt.MapFrom(src => src.CertificateReview))
           .ForMember(dest => dest.PublicInterestCheck, opt => opt.MapFrom(src => src.PublicInterestCheck))
           .ForMember(dest => dest.CabTransferRequest, opt => opt.MapFrom(src => src.CabTransferRequest))
           .ForMember(dest => dest.TrustFrameworkVersion, opt => opt.MapFrom(src => src.TrustFrameworkVersion));


            CreateMap<ProceedApplicationConsentToken, ProceedApplicationConsentTokenDto>()
           .ForMember(dest => dest.Service, opt => opt.MapFrom(src => src.Service));
            CreateMap<ProceedApplicationConsentTokenDto, ProceedApplicationConsentToken>()
            .ForMember(dest => dest.Service, opt => opt.MapFrom(src => src.Service));

            CreateMap<UserDto, User>();
            CreateMap<User, UserDto>();

            CreateMap<RequestManagement, RequestManagementDto>();
            CreateMap<RequestManagementDto, RequestManagement>();

            CreateMap<CabTransferRequest, CabTransferRequestDto>()
          .ForMember(dest => dest.RequestManagement, opt => opt.MapFrom(src => src.RequestManagement));
            CreateMap<CabTransferRequestDto, CabTransferRequest>()
            .ForMember(dest => dest.RequestManagement, opt => opt.MapFrom(src => src.RequestManagement));

            CreateMap<TrustFrameworkVersion, TrustFrameworkVersionDto>();
            CreateMap<TrustFrameworkVersionDto, TrustFrameworkVersion>();
            CreateMap<ManualUnderPinningService, ManualUnderPinningServiceDto>();
            CreateMap<ManualUnderPinningServiceDto, ManualUnderPinningService>();

           
            CreateMap<ProviderRemovalRequest, ProviderRemovalRequestDto>();
            CreateMap<ProviderRemovalRequestDto, ProviderRemovalRequest>();

            CreateMap<ServiceDraft, ServiceDraftDto>();
            CreateMap<ServiceDraftDto, ServiceDraft>();

            CreateMap<ServiceRemovalRequest, ServiceRemovalRequestDto>();
            CreateMap<ServiceRemovalRequestDto, ServiceRemovalRequest>();

            CreateMap<ActionLogs, ActionLogsDto>();
            CreateMap<ActionLogsDto, ActionLogs>();

            CreateMap<ActionDetailsDto, ActionDetails>();
            CreateMap<ActionDetails, ActionDetailsDto>();

            CreateMap<ActionCategoryDto, ActionCategory>();
            CreateMap<ActionCategoryDto, ActionCategory>();

            CreateMap<ProviderProfileDraft, ProviderProfileDraftDto>();
            CreateMap<ProviderProfileDraftDto, ProviderProfileDraft>();
            CreateMap<ProviderRemovalRequestServiceMapping, ProviderRemovalRequestServiceMappingDto>();
            CreateMap<ProviderRemovalRequestServiceMappingDto, ProviderRemovalRequestServiceMapping>();


        }
    }
}
