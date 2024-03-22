using DVSRegister.CommonUtility.Models;
using DVSRegister.Data.Entities;

namespace DVSRegister.Data.Repositories
{
    public interface IPreRegistrationRepository
    {
        public Task<GenericResponse> SavePreRegistration(PreRegistration preAssessment);
        public Task<List<Country>> GetCountries();
    }
}
