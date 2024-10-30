using AutoMapper;
using DVSRegister.BusinessLogic.Models.CAB;
using DVSRegister.CommonUtility;
using DVSRegister.Data.Entities;

namespace DVSRegister.BusinessLogic
{
    public class LastModifiedDateResolver : IValueResolver<ProviderProfile, ProviderProfileDto, DateTimeInfoDto>
    {
        public DateTimeInfoDto Resolve(ProviderProfile source, ProviderProfileDto destination, DateTimeInfoDto destMember, ResolutionContext context)
        {
            DateTime? lastModifiedTime = null;
            if (source.Services != null && source.Services.Any())
            {
                lastModifiedTime = source.Services.Max(s => s.ModifiedTime ?? s.CreatedTime);
            }
            else
            {
                lastModifiedTime = source.ModifiedTime == null ? source.CreatedTime : source.ModifiedTime;
            }

            return new DateTimeInfoDto
            {
                LastModifiedTime = lastModifiedTime ?? DateTime.MinValue 
            };
        }
    }

}

