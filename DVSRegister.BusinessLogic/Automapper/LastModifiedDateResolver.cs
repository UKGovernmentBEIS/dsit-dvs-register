using AutoMapper;
using DVSRegister.BusinessLogic.Models.CAB;
using DVSRegister.CommonUtility;
using DVSRegister.Data.Entities;

namespace DVSRegister.BusinessLogic
{
    public class LastModifiedDateResolver : IValueResolver<ProviderProfile, ProviderProfileDto, string>
    {
        public string Resolve(ProviderProfile source, ProviderProfileDto destination, string lastModifiedDate, ResolutionContext context)
        {
            DateTime? dateTime = null;
          
            if (source.Services!= null && source.Services.Any())
            {
                dateTime =   source.Services.Max(s => s.ModifiedTime ?? s.CreatedTime);
                return Helper.GetLocalDateTime(dateTime, "dd MMM yyyy ; hh:mm tt");
            }
            else
            {
                dateTime =  source.ModifiedTime == null ? source.CreatedTime : source.ModifiedTime;
                return Helper.GetLocalDateTime(dateTime, "dd MMM yyyy ; hh:mm tt");                
            }
        }
    }
}
