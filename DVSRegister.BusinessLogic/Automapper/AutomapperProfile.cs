﻿using AutoMapper;
using DVSRegister.BusinessLogic.Models;
using DVSRegister.BusinessLogic.Models.CAB;
using DVSRegister.BusinessLogic.Models.Register;
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

            CreateMap<ProceedPublishConsentToken, ProceedPublishConsentTokenDto>()
            .ForMember(dest => dest.Service, opt => opt.MapFrom(src => src.Service));
            CreateMap<ProceedPublishConsentTokenDto, ProceedPublishConsentToken>()
            .ForMember(dest => dest.Service, opt => opt.MapFrom(src => src.Service));

            CreateMap<ProviderProfileDraft, ProviderProfileDraftDto>();
            CreateMap<ProviderProfileDraftDto, ProviderProfileDraft>();


            CreateMap<ProviderDraftToken, ProviderDraftTokenDto>();
            CreateMap<ProviderDraftTokenDto, ProviderDraftToken>();


            CreateMap<ServiceIdentityProfileMappingDraft, ServiceIdentityProfileMappingDraftDto>();
            CreateMap<ServiceIdentityProfileMappingDraftDto, ServiceIdentityProfileMappingDraft>().ForMember(x => x.IdentityProfile, opt => opt.Ignore());
            CreateMap<ServiceRoleMappingDraft, ServiceRoleMappingDraftDto>();
            CreateMap<ServiceRoleMappingDraftDto, ServiceRoleMappingDraft>().ForMember(x => x.Role, opt => opt.Ignore());


            CreateMap<ServiceSupSchemeMappingDraft, ServiceSupSchemeMappingDraftDto>();
            CreateMap<ServiceSupSchemeMappingDraftDto, ServiceSupSchemeMappingDraft>().ForMember(x => x.SupplementaryScheme, opt => opt.Ignore());
            CreateMap<ServiceQualityLevelMappingDraft, ServiceQualityLevelMappingDraftDto>();
            CreateMap<ServiceQualityLevelMappingDraftDto, ServiceQualityLevelMappingDraft>().ForMember(x => x.QualityLevel, opt => opt.Ignore());

            CreateMap<ServiceDraft, ServiceDraftDto>()
            .ForMember(dest => dest.ServiceQualityLevelMappingDraft, opt => opt.MapFrom(src => src.ServiceQualityLevelMappingDraft))
            .ForMember(dest => dest.ServiceRoleMappingDraft, opt => opt.MapFrom(src => src.ServiceRoleMappingDraft))
            .ForMember(dest => dest.ServiceIdentityProfileMappingDraft, opt => opt.MapFrom(src => src.ServiceIdentityProfileMappingDraft))
            .ForMember(dest => dest.ServiceSupSchemeMappingDraft, opt => opt.MapFrom(src => src.ServiceSupSchemeMappingDraft))
            .ForMember(dest => dest.Provider, opt => opt.MapFrom(src => src.Provider));

            CreateMap<ServiceDraftDto, ServiceDraft>()
            .ForMember(dest => dest.ServiceQualityLevelMappingDraft, opt => opt.MapFrom(src => src.ServiceQualityLevelMappingDraft))
            .ForMember(dest => dest.ServiceRoleMappingDraft, opt => opt.MapFrom(src => src.ServiceRoleMappingDraft))
            .ForMember(dest => dest.ServiceIdentityProfileMappingDraft, opt => opt.MapFrom(src => src.ServiceIdentityProfileMappingDraft))
            .ForMember(dest => dest.ServiceSupSchemeMappingDraft, opt => opt.MapFrom(src => src.ServiceSupSchemeMappingDraft))
            .ForMember(dest => dest.Provider, opt => opt.MapFrom(src => src.Provider));

            CreateMap<ServiceDraftToken, ServiceDraftTokenDto>();
            CreateMap<ServiceDraftTokenDto, ServiceDraftToken>();
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
        }
    }
}
