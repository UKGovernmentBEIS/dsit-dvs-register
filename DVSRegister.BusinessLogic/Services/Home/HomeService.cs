using AutoMapper;
using DVSRegister.BusinessLogic.Models;
using DVSRegister.BusinessLogic.Models.CAB;
using DVSRegister.Data;

namespace DVSRegister.BusinessLogic.Services
{
    public class HomeService: IHomeService
    {
        private readonly IHomeRepository homeRepository;
        private readonly IMapper automapper;

        public HomeService(IHomeRepository homeRepository, IMapper automapper)
        {
            this.homeRepository = homeRepository;
            this.automapper = automapper;
        }
        public async Task<PaginatedResult<ServiceDto>> GetDraftApplications(int cabId, int pageNumber, string sort, string sortAction)
        {
            var paginatedServices = await homeRepository.GetDraftApplications(cabId, pageNumber, sort, sortAction);
            var serviceDtos = automapper.Map<List<ServiceDto>>(paginatedServices.Items);

            return new PaginatedResult<ServiceDto>
            {
                Items = serviceDtos,
                TotalCount = paginatedServices.TotalCount
            };
        }
        public async Task<PaginatedResult<ServiceDto>> GetSentBackApplications(int cabId, int pageNumber, string sort, string sortAction)
        {
            var paginatedServices = await homeRepository.GetSentBackApplications(cabId, pageNumber, sort, sortAction);
            var serviceDtos = automapper.Map<List<ServiceDto>>(paginatedServices.Items);

            return new PaginatedResult<ServiceDto>
            {
                Items = serviceDtos,
                TotalCount = paginatedServices.TotalCount
            };
        }
        public async Task<PaginatedResult<ServiceDto>> GetPendingReassignmentRequests(int cabId, int pageNumber, string sort, string sortAction)
        {
            var paginatedServices = await homeRepository.GetPendingReassignmentRequests(cabId, pageNumber, sort, sortAction);
            var serviceDtos = automapper.Map<List<ServiceDto>>(paginatedServices.Items);

            return new PaginatedResult<ServiceDto>
            {
                Items = serviceDtos,
                TotalCount = paginatedServices.TotalCount
            };
        }
        public async Task<Dictionary<string, int>> GetPendingCounts(int cabId)
        {
            return await homeRepository.GetPendingCounts(cabId);
        }
    }
}
