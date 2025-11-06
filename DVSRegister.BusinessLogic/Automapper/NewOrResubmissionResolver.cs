using AutoMapper;
using DVSRegister.BusinessLogic.Models.CAB;
using DVSRegister.CommonUtility;
using DVSRegister.Data.Entities;

namespace DVSRegister.BusinessLogic.Automapper
{
    public class NewOrResubmissionResolver : IValueResolver<Service, ServiceDto, string>
    {
        public string Resolve(Service source, ServiceDto destination, string newOrResubmission, ResolutionContext context)
        {
            string result = string.Empty;
            if (source.ServiceVersion == 1)
                result = Constants.NewApplication;
            else if (destination.ServiceVersion > 1)
                result = Constants.ReApplication;
            return result;
        }
    }
}
