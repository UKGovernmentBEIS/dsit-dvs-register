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
           .ForMember(dest => dest.ServiceRoleMapping, opt => opt.MapFrom(src =>
               src.RoleViewModel.SelectedRoles.Select(role => new ServiceRoleMappingDto { RoleId = role.Id })))
           .ForMember(dest => dest.ServiceQualityLevelMapping, opt => opt.MapFrom(src =>
               src.QualityLevelViewModel.SelectedQualityofAuthenticators.Select(q => new ServiceQualityLevelMappingDto { QualityLevelId = q.Id })
               .Concat(src.QualityLevelViewModel.SelectedLevelOfProtections.Select(q => new ServiceQualityLevelMappingDto { QualityLevelId = q.Id })))
           )
           .ForMember(dest => dest.ServiceIdentityProfileMapping, opt => opt.MapFrom(src =>
               src.IdentityProfileViewModel.SelectedIdentityProfiles.Select(profile => new ServiceIdentityProfileMappingDto { IdentityProfileId = profile.Id })))
           .ForMember(dest => dest.ServiceSupSchemeMapping, opt => opt.MapFrom(src =>
               src.SupplementarySchemeViewModel.SelectedSupplementarySchemes.Select(scheme => new ServiceSupSchemeMappingDto { SupplementarySchemeId = scheme.Id })))
           .ForMember(dest => dest.FileSizeInKb, opt => opt.MapFrom(src => src.FileSizeInKb ?? 0))
           .ForMember(dest => dest.ConformityIssueDate, opt => opt.MapFrom(src => Convert.ToDateTime(src.ConformityIssueDate)))
           .ForMember(dest => dest.ConformityExpiryDate, opt => opt.MapFrom(src => Convert.ToDateTime(src.ConformityExpiryDate)));
        }
    }


   
}
