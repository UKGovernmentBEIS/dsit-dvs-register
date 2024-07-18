using DVSRegister.BusinessLogic.Models.PreRegistration;
using DVSRegister.CommonUtility.Models;

namespace DVSRegister.BusinessLogic.Services.PreAssessment
{
    public interface IPreRegistrationService
    {
        public Task<GenericResponse> SavePreRegistration(PreRegistrationDto preRegistration);
        public Task<List<CountryDto>> GetCountries();
    }
}
