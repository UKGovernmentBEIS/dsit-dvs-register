using DVSRegister.BusinessLogic.Models.CAB;

namespace DVSRegister.Models.CAB.Service
{
    public class ServiceVersionViewModel
    {
        public ServiceDto CurrentServiceVersion { get; set; }
        public List<ServiceDto> ServiceHistoryVersions { get; set; }

    }
}
