using AutoMapper;
using DVSRegister.BusinessLogic.Models.CAB;
using DVSRegister.CommonUtility;
using DVSRegister.Data.Entities;
using System;
using System.Globalization;

namespace DVSRegister.BusinessLogic
{
    public class LastModifiedDateResolver : IValueResolver<ProviderProfile, ProviderProfileDto, DateTime>
    {
        public DateTime Resolve(ProviderProfile source, ProviderProfileDto destination, DateTime destMember, ResolutionContext context)
        {
            DateTime? dateTime=null;
            if (source.Services != null && source.Services.Any())
            {
                 dateTime = source.Services.Max(s => s.ModifiedTime ?? s.CreatedTime);
            }
            else
            {
                dateTime = source.ModifiedTime == null ? source.CreatedTime : source.ModifiedTime;
            }

            if (dateTime.HasValue)
            {
                return dateTime.Value;
            }

            return DateTime.MinValue;
        }

    }
}
