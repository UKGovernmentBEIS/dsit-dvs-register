using AutoMapper;
using DVSRegister.BusinessLogic.Models;
using DVSRegister.BusinessLogic.Models.CAB;
using DVSRegister.BusinessLogic.Models.PreRegistration;
using DVSRegister.BusinessLogic.Models.Register;
using DVSRegister.Data.Entities;

namespace DVSRegister.BusinessLogic
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<PreRegistrationCountryMapping, PreRegistrationCountryMappingDto>();
            CreateMap<PreRegistrationCountryMappingDto, PreRegistrationCountryMapping>();
            CreateMap<Country, CountryDto>();
            CreateMap<CountryDto, Country>();
            CreateMap<PreRegistration, PreRegistrationDto>().ForMember(dest => dest.PreRegistrationCountryMappings, opt => opt.MapFrom(src => src.PreRegistrationCountryMappings));
            CreateMap<PreRegistrationDto, PreRegistration>().ForMember(dest => dest.PreRegistrationCountryMappings, opt => opt.MapFrom(src => src.PreRegistrationCountryMappings));
            CreateMap<URNDto, UniqueReferenceNumber>();
            CreateMap<UniqueReferenceNumber, URNDto>();

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


            CreateMap<CertificateInfoIdentityProfileMapping, CertificateInfoIdentityProfileMappingDto>();
            CreateMap<CertificateInfoIdentityProfileMappingDto, CertificateInfoIdentityProfileMapping>();
         
            CreateMap<CertificateInfoSupSchemeMapping, CertificateInfoSupSchemeMappingDto>();
            CreateMap<CertificateInfoSupSchemeMappingDto, CertificateInfoSupSchemeMapping>();

            CreateMap<Provider, ProviderDto>()
           .ForMember(dest => dest.CertificateInformation, opt => opt.MapFrom(src => src.CertificateInformation));
            CreateMap<ProviderDto, Provider>()
            .ForMember(dest => dest.CertificateInformation, opt => opt.MapFrom(src => src.CertificateInformation));
           

            CreateMap<RegisterPublishLog, RegisterPublishLogDto>();
            CreateMap<RegisterPublishLogDto, RegisterPublishLog>();


            CreateMap<ProviderProfile, ProviderProfileDto>()
            .ForMember(dest => dest.Services, opt => opt.MapFrom(src => src.Services));
            CreateMap<ProviderProfileDto, ProviderProfile>()
            .ForMember(dest => dest.Services, opt => opt.MapFrom(src => src.Services));

            CreateMap<Cab, CabDto>();
            CreateMap<CabDto, Cab>();
            CreateMap<CabUser, CabUserDto>();
            CreateMap<CabUserDto, CabUser>();

            CreateMap<Service, ServiceDto>()
            .ForMember(dest => dest.ServiceQualityLevelMapping, opt => opt.MapFrom(src => src.ServiceQualityLevelMapping))
            .ForMember(dest => dest.ServiceRoleMapping, opt => opt.MapFrom(src => src.ServiceRoleMapping))
            .ForMember(dest => dest.ServiceIdentityProfileMapping, opt => opt.MapFrom(src => src.ServiceIdentityProfileMapping))
            .ForMember(dest => dest.ServiceSupSchemeMapping, opt => opt.MapFrom(src => src.ServiceSupSchemeMapping))
            .ForMember(dest => dest.Provider, opt => opt.MapFrom(src => src.Provider));

            CreateMap<ServiceDto, Service>()
           .ForMember(dest => dest.ServiceQualityLevelMapping, opt => opt.MapFrom(src => src.ServiceQualityLevelMapping))
           .ForMember(dest => dest.ServiceRoleMapping, opt => opt.MapFrom(src => src.ServiceRoleMapping))
           .ForMember(dest => dest.ServiceIdentityProfileMapping, opt => opt.MapFrom(src => src.ServiceIdentityProfileMapping))
           .ForMember(dest => dest.ServiceSupSchemeMapping, opt => opt.MapFrom(src => src.ServiceSupSchemeMapping))
           .ForMember(dest => dest.Provider, opt => opt.MapFrom(src => src.Provider));
        }
    }
}
