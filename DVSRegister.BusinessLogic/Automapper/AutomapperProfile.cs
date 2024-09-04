using AutoMapper;
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
            CreateMap<ServiceSupSchemeMapping, ServiceSupSchemeMappingDto>();
            CreateMap<ServiceSupSchemeMappingDto, ServiceSupSchemeMapping>();
            CreateMap<ServiceQualityLevelMapping, ServiceQualityLevelMappingDto>();
            CreateMap<ServiceQualityLevelMappingDto, ServiceQualityLevelMapping>();         
          

            CreateMap<Provider, ProviderDto>();
            CreateMap<ProviderDto, Provider>();
           

            CreateMap<RegisterPublishLog, RegisterPublishLogDto>();
            CreateMap<RegisterPublishLogDto, RegisterPublishLog>();


            CreateMap<ProviderProfile, ProviderProfileDto>()
            .ForMember(dest => dest.Services, opt => opt.MapFrom(src => src.Services))
            .ForMember(dest => dest.LastModified, opt => opt.MapFrom<LastModifiedDateResolver>());
            CreateMap<ProviderProfileDto, ProviderProfile>()
            .ForMember(dest => dest.Services, opt => opt.MapFrom(src => src.Services));

            CreateMap<Cab, CabDto>();
            CreateMap<CabDto, Cab>();
            CreateMap<CabUser, CabUserDto>();
            CreateMap<CabUserDto, CabUser>();

           
            CreateMap<CertificateReview, CertificateReviewDto>();
            CreateMap<CertificateReviewDto, CertificateReview>();

            CreateMap<Service, ServiceDto>()
            .ForMember(dest => dest.ServiceQualityLevelMapping, opt => opt.MapFrom(src => src.ServiceQualityLevelMapping))
            .ForMember(dest => dest.ServiceRoleMapping, opt => opt.MapFrom(src => src.ServiceRoleMapping))
            .ForMember(dest => dest.ServiceIdentityProfileMapping, opt => opt.MapFrom(src => src.ServiceIdentityProfileMapping))
            .ForMember(dest => dest.ServiceSupSchemeMapping, opt => opt.MapFrom(src => src.ServiceSupSchemeMapping))
            .ForMember(dest => dest.Provider, opt => opt.MapFrom(src => src.Provider))
            .ForMember(dest => dest.CertificateReview, opt => opt.MapFrom(src => src.CertificateReview));

            CreateMap<ServiceDto, Service>()
           .ForMember(dest => dest.ServiceQualityLevelMapping, opt => opt.MapFrom(src => src.ServiceQualityLevelMapping))
           .ForMember(dest => dest.ServiceRoleMapping, opt => opt.MapFrom(src => src.ServiceRoleMapping))
           .ForMember(dest => dest.ServiceIdentityProfileMapping, opt => opt.MapFrom(src => src.ServiceIdentityProfileMapping))
           .ForMember(dest => dest.ServiceSupSchemeMapping, opt => opt.MapFrom(src => src.ServiceSupSchemeMapping))
           .ForMember(dest => dest.Provider, opt => opt.MapFrom(src => src.Provider))
           .ForMember(dest => dest.CertificateReview, opt => opt.MapFrom(src => src.CertificateReview));
        }
    }
}
