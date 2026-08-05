using AutoMapper;
using DVSRegister.BusinessLogic.Models.CAB;
using DVSRegister.BusinessLogic.Services;
using DVSRegister.Data;
using DVSRegister.Data.Entities;
using DVSRegister.Data.Models;
using NSubstitute;

namespace DVSRegister.UnitTests.Services
{
    public class HomeServiceTests
    {
        private readonly IHomeRepository _homeRepository;
        private readonly IMapper _mapper;
        private readonly HomeService _service;

        public HomeServiceTests()
        {
            _homeRepository = Substitute.For<IHomeRepository>();
            _mapper = Substitute.For<IMapper>();
            _service = new HomeService(_homeRepository, _mapper);
        }

        [Fact]
        public async Task GetDraftApplications_ReturnsMappedPaginatedServices()
        {
            // Arrange
            var services = new List<Service>
            {
                new() { Id = 1 },
                new() { Id = 2 }
            };
            var serviceDtos = new List<ServiceDto>
            {
                new() { Id = 1 },
                new() { Id = 2 }
            };
            var page = new PaginatedResult<Service>
            {
                Items = services,
                TotalCount = 12
            };

            _homeRepository
                .GetDraftApplications(3, 2, "serviceName", "ascending")
                .Returns(page);
            _mapper.Map<List<ServiceDto>>(services).Returns(serviceDtos);

            // Act
            var result = await _service.GetDraftApplications(3, 2, "serviceName", "ascending");

            // Assert
            Assert.Same(serviceDtos, result.Items);
            Assert.Equal(12, result.TotalCount);
            await _homeRepository.Received(1)
                .GetDraftApplications(3, 2, "serviceName", "ascending");
            _mapper.Received(1).Map<List<ServiceDto>>(services);
        }

        [Fact]
        public async Task GetSentBackApplications_ReturnsMappedPaginatedServices()
        {
            // Arrange
            var services = new List<Service>
            {
                new() { Id = 4 }
            };
            var serviceDtos = new List<ServiceDto>
            {
                new() { Id = 4 }
            };
            var page = new PaginatedResult<Service>
            {
                Items = services,
                TotalCount = 7
            };

            _homeRepository
                .GetSentBackApplications(5, 3, "providerName", "descending")
                .Returns(page);
            _mapper.Map<List<ServiceDto>>(services).Returns(serviceDtos);

            // Act
            var result = await _service.GetSentBackApplications(5, 3, "providerName", "descending");

            // Assert
            Assert.Same(serviceDtos, result.Items);
            Assert.Equal(7, result.TotalCount);
            await _homeRepository.Received(1)
                .GetSentBackApplications(5, 3, "providerName", "descending");
            _mapper.Received(1).Map<List<ServiceDto>>(services);
        }

        [Fact]
        public async Task GetPendingReassignmentRequests_ReturnsMappedPaginatedServices()
        {
            // Arrange
            var services = new List<Service>();
            var serviceDtos = new List<ServiceDto>();
            var page = new PaginatedResult<Service>
            {
                Items = services,
                TotalCount = 0
            };

            _homeRepository
                .GetPendingReassignmentRequests(8, 1, "submittedDate", "ascending")
                .Returns(page);
            _mapper.Map<List<ServiceDto>>(services).Returns(serviceDtos);

            // Act
            var result = await _service.GetPendingReassignmentRequests(8, 1, "submittedDate", "ascending");

            // Assert
            Assert.Same(serviceDtos, result.Items);
            Assert.Equal(0, result.TotalCount);
            await _homeRepository.Received(1)
                .GetPendingReassignmentRequests(8, 1, "submittedDate", "ascending");
            _mapper.Received(1).Map<List<ServiceDto>>(services);
        }

        [Fact]
        public async Task GetPendingCounts_ReturnsRepositoryCountsWithoutMapping()
        {
            // Arrange
            var counts = new Dictionary<string, int>
            {
                ["Drafts"] = 2,
                ["SentBack"] = 3,
                ["Reassignments"] = 1
            };

            _homeRepository.GetPendingCounts(13).Returns(counts);

            // Act
            var result = await _service.GetPendingCounts(13);

            // Assert
            Assert.Same(counts, result);
            await _homeRepository.Received(1).GetPendingCounts(13);
            Assert.Empty(_mapper.ReceivedCalls());
        }

        [Fact]
        public async Task GetAllProviders_ReturnsMappedPaginatedProviders()
        {
            // Arrange
            var providers = new List<ProviderProfile>
            {
                new() { Id = 21, RegisteredName = "Provider One" },
                new() { Id = 22, RegisteredName = "Provider Two" }
            };
            var providerDtos = new List<ProviderProfileDto>
            {
                new() { Id = 21, RegisteredName = "Provider One" },
                new() { Id = 22, RegisteredName = "Provider Two" }
            };
            var page = new PaginatedResult<ProviderProfile>
            {
                Items = providers,
                TotalCount = 25
            };

            _homeRepository
                .GetAllProviders(11, 4, "registeredName", "descending", "Provider")
                .Returns(page);
            _mapper.Map<List<ProviderProfileDto>>(providers).Returns(providerDtos);

            // Act
            var result = await _service.GetAllProviders(11, 4, "registeredName", "descending", "Provider");

            // Assert
            Assert.Same(providerDtos, result.Items);
            Assert.Equal(25, result.TotalCount);
            await _homeRepository.Received(1)
                .GetAllProviders(11, 4, "registeredName", "descending", "Provider");
            _mapper.Received(1).Map<List<ProviderProfileDto>>(providers);
        }
    }
}
