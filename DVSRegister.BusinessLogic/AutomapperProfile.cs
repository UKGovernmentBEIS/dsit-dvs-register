using AutoMapper;
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
            CreateMap<CertificateInfoIdentityProfileMapping, CertificateInfoIdentityProfileMappingDto>();
            CreateMap<CertificateInfoIdentityProfileMappingDto, CertificateInfoIdentityProfileMapping>();
            CreateMap<CertificateInfoRoleMapping, CertificateInfoRoleMappingDto>();
            CreateMap<CertificateInfoRoleMappingDto, CertificateInfoRoleMapping>();
            CreateMap<CertificateInfoSupSchemeMapping, CertificateInfoSupSchemeMappingDto>();
            CreateMap<CertificateInfoSupSchemeMappingDto, CertificateInfoSupSchemeMapping>();

            CreateMap<Provider, ProviderDto>()
           .ForMember(dest => dest.CertificateInformation, opt => opt.MapFrom(src => src.CertificateInformation));
            CreateMap<ProviderDto, Provider>()
            .ForMember(dest => dest.CertificateInformation, opt => opt.MapFrom(src => src.CertificateInformation));
            CreateMap<CertificateInformation, CertificateInfoDto>()
            .ForMember(dest => dest.CertificateInfoRoleMappings, opt => opt.MapFrom(src => src.CertificateInfoRoleMappings))
            .ForMember(dest => dest.CertificateInfoIdentityProfileMappings, opt => opt.MapFrom(src => src.CertificateInfoIdentityProfileMappings))
            .ForMember(dest => dest.CertificateInfoSupSchemeMappings, opt => opt.MapFrom(src => src.CertificateInfoSupSchemeMappings))
              .ForMember(dest => dest.Provider, opt => opt.MapFrom(src => src.Provider));
            CreateMap<CertificateInfoDto, CertificateInformation>()
            .ForMember(dest => dest.CertificateInfoRoleMappings, opt => opt.MapFrom(src => src.CertificateInfoRoleMappings))
            .ForMember(dest => dest.CertificateInfoIdentityProfileMappings, opt => opt.MapFrom(src => src.CertificateInfoIdentityProfileMappings))
            .ForMember(dest => dest.CertificateInfoSupSchemeMappings, opt => opt.MapFrom(src => src.CertificateInfoSupSchemeMappings))
             .ForMember(dest => dest.Provider, opt => opt.MapFrom(src => src.Provider));

            CreateMap<RegisterPublishLog, RegisterPublishLogDto>();
            CreateMap<RegisterPublishLogDto, RegisterPublishLog>();
        }
    }
}
