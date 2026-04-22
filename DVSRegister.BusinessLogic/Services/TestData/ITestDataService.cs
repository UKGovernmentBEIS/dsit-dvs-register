using DVSRegister.BusinessLogic.Models.CAB;

namespace DVSRegister.BusinessLogic.Services.TestData
{
    public interface ITestDataService
    {
        public Task CreateTestProvider(string loggedInUserEmail, bool allowTestDataCreation, ProviderProfileDto providerProfileDto);
        public Task CreateTestServices(string loggedInUserEmail, bool allowTestDataCreation, int providerId, List<ServiceDto> services);
    }
}
