using AutoMapper;
using DVSRegister.BusinessLogic.Models;
using DVSRegister.BusinessLogic.Models.CAB;
using DVSRegister.BusinessLogic.Services;
using DVSRegister.Data;
using DVSRegister.Data.Entities;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace DVSRegister.UnitTests.Services
{
    public class TrustFrameworkServiceTests
    {
        private readonly ITrustFrameworkRepository _trustFrameworkRepository;
        private readonly ICommonRepository _commonRepository;
        private readonly IMapper _mapper;
        private readonly TrustFrameworkService _service;

        public TrustFrameworkServiceTests()
        {
            _trustFrameworkRepository = Substitute.For<ITrustFrameworkRepository>();
            _commonRepository = Substitute.For<ICommonRepository>();
            _mapper = Substitute.For<IMapper>();
            _service = new TrustFrameworkService(
                _trustFrameworkRepository,
                _commonRepository,
                _mapper);
        }

        [Fact]
        public async Task GetActiveTrustFrameworkVersions_RepositoryReturnsVersions_ReturnsMappedVersions()
        {
            var versions = new List<TrustFrameworkVersion>
            {
                new() { Id = 1, TrustFrameworkName = "Version one" },
                new() { Id = 2, TrustFrameworkName = "Version two" }
            };
            var expected = new List<TrustFrameworkVersionDto>
            {
                new() { Id = 1, TrustFrameworkName = "Version one" },
                new() { Id = 2, TrustFrameworkName = "Version two" }
            };
            _commonRepository.GetActiveTfVersion().Returns(versions);
            _mapper.Map<List<TrustFrameworkVersionDto>>(versions).Returns(expected);

            var result = await _service.GetActiveTrustFrameworkVersions();

            Assert.Same(expected, result);
            await _commonRepository.Received(1).GetActiveTfVersion();
            _mapper.Received(1).Map<List<TrustFrameworkVersionDto>>(versions);
        }

        [Fact]
        public async Task GetActiveTrustFrameworkVersions_RepositoryReturnsEmptyList_ReturnsMappedEmptyList()
        {
            var versions = new List<TrustFrameworkVersion>();
            var expected = new List<TrustFrameworkVersionDto>();
            _commonRepository.GetActiveTfVersion().Returns(versions);
            _mapper.Map<List<TrustFrameworkVersionDto>>(versions).Returns(expected);

            var result = await _service.GetActiveTrustFrameworkVersions();

            Assert.Same(expected, result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetActiveTrustFrameworkVersions_RepositoryReturnsNull_MapsNullAndReturnsMapperResult()
        {
            var expected = new List<TrustFrameworkVersionDto>();
            _commonRepository.GetActiveTfVersion()
                .Returns((List<TrustFrameworkVersion>)null!);
            _mapper.Map<List<TrustFrameworkVersionDto>>(null!).Returns(expected);

            var result = await _service.GetActiveTrustFrameworkVersions();

            Assert.Same(expected, result);
            _mapper.Received(1).Map<List<TrustFrameworkVersionDto>>(null!);
        }

        [Fact]
        public async Task GetActiveTrustFrameworkVersions_RepositoryThrows_PropagatesFailureWithoutMapping()
        {
            _commonRepository.GetActiveTfVersion()
                .ThrowsAsync(new InvalidOperationException("version failure"));

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _service.GetActiveTrustFrameworkVersions());

            Assert.Equal("version failure", exception.Message);
            _mapper.DidNotReceive().Map<List<TrustFrameworkVersionDto>>(Arg.Any<object>());
        }

        [Fact]
        public async Task GetActiveTrustFrameworkVersions_MapperThrows_PropagatesFailure()
        {
            var versions = new List<TrustFrameworkVersion> { new() { Id = 1 } };
            _commonRepository.GetActiveTfVersion().Returns(versions);
            _mapper.Map<List<TrustFrameworkVersionDto>>(versions)
                .Throws(new AutoMapperMappingException("mapping failure"));

            var exception = await Assert.ThrowsAsync<AutoMapperMappingException>(() =>
                _service.GetActiveTrustFrameworkVersions());

            Assert.Equal("mapping failure", exception.Message);
        }

        [Fact]
        public async Task GetCabs_RepositoryReturnsCabs_ReturnsMappedCabs()
        {
            var cabs = new List<Cab>
            {
                new() { Id = 1, CabName = "CAB one" },
                new() { Id = 2, CabName = "CAB two" }
            };
            var expected = new List<CabDto>
            {
                new() { Id = 1, CabName = "CAB one" },
                new() { Id = 2, CabName = "CAB two" }
            };
            _trustFrameworkRepository.GetCabs().Returns(cabs);
            _mapper.Map<List<CabDto>>(cabs).Returns(expected);

            var result = await _service.GetCabs();

            Assert.Same(expected, result);
            await _trustFrameworkRepository.Received(1).GetCabs();
            _mapper.Received(1).Map<List<CabDto>>(cabs);
        }

        [Fact]
        public async Task GetCabs_RepositoryReturnsEmptyList_ReturnsMappedEmptyList()
        {
            var cabs = new List<Cab>();
            var expected = new List<CabDto>();
            _trustFrameworkRepository.GetCabs().Returns(cabs);
            _mapper.Map<List<CabDto>>(cabs).Returns(expected);

            var result = await _service.GetCabs();

            Assert.Same(expected, result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetCabs_RepositoryReturnsNull_MapsNullAndReturnsMapperResult()
        {
            var expected = new List<CabDto>();
            _trustFrameworkRepository.GetCabs().Returns((List<Cab>)null!);
            _mapper.Map<List<CabDto>>(null!).Returns(expected);

            var result = await _service.GetCabs();

            Assert.Same(expected, result);
            _mapper.Received(1).Map<List<CabDto>>(null!);
        }

        [Fact]
        public async Task GetCabs_RepositoryThrows_PropagatesFailureWithoutMapping()
        {
            _trustFrameworkRepository.GetCabs()
                .ThrowsAsync(new InvalidOperationException("cab failure"));

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _service.GetCabs());

            Assert.Equal("cab failure", exception.Message);
            _mapper.DidNotReceive().Map<List<CabDto>>(Arg.Any<object>());
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("Provider & Service")]
        public async Task GetPublishedUnderpinningServices_SearchTextProvided_ForwardsSearchTextUnchanged(
            string searchText)
        {
            var services = new List<Service>();
            var expected = new List<ServiceDto>();
            _trustFrameworkRepository.GetPublishedUnderpinningServices(searchText).Returns(services);
            _mapper.Map<List<ServiceDto>>(services).Returns(expected);

            var result = await _service.GetPublishedUnderpinningServices(searchText);

            Assert.Same(expected, result);
            await _trustFrameworkRepository.Received(1)
                .GetPublishedUnderpinningServices(searchText);
        }

        [Fact]
        public async Task GetPublishedUnderpinningServices_NullSearchText_ForwardsNullUnchanged()
        {
            var services = new List<Service>();
            var expected = new List<ServiceDto>();
            _trustFrameworkRepository.GetPublishedUnderpinningServices(null!).Returns(services);
            _mapper.Map<List<ServiceDto>>(services).Returns(expected);

            var result = await _service.GetPublishedUnderpinningServices(null!);

            Assert.Same(expected, result);
            await _trustFrameworkRepository.Received(1)
                .GetPublishedUnderpinningServices(null!);
        }

        [Fact]
        public async Task GetPublishedUnderpinningServices_RepositoryReturnsServices_ReturnsMappedServices()
        {
            var services = new List<Service>
            {
                new() { Id = 11, ServiceName = "Service one" },
                new() { Id = 12, ServiceName = "Service two" }
            };
            var expected = new List<ServiceDto>
            {
                new() { Id = 11, ServiceName = "Service one" },
                new() { Id = 12, ServiceName = "Service two" }
            };
            _trustFrameworkRepository.GetPublishedUnderpinningServices("service").Returns(services);
            _mapper.Map<List<ServiceDto>>(services).Returns(expected);

            var result = await _service.GetPublishedUnderpinningServices("service");

            Assert.Same(expected, result);
            _mapper.Received(1).Map<List<ServiceDto>>(services);
        }

        [Fact]
        public async Task GetPublishedUnderpinningServices_RepositoryReturnsNull_MapsNullAndReturnsMapperResult()
        {
            var expected = new List<ServiceDto>();
            _trustFrameworkRepository.GetPublishedUnderpinningServices("search")
                .Returns((List<Service>)null!);
            _mapper.Map<List<ServiceDto>>(null!).Returns(expected);

            var result = await _service.GetPublishedUnderpinningServices("search");

            Assert.Same(expected, result);
            _mapper.Received(1).Map<List<ServiceDto>>(null!);
        }

        [Fact]
        public async Task GetPublishedUnderpinningServices_RepositoryThrows_PropagatesFailureWithoutMapping()
        {
            _trustFrameworkRepository.GetPublishedUnderpinningServices("search")
                .ThrowsAsync(new InvalidOperationException("published service failure"));

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _service.GetPublishedUnderpinningServices("search"));

            Assert.Equal("published service failure", exception.Message);
            _mapper.DidNotReceive().Map<List<ServiceDto>>(Arg.Any<object>());
        }

        [Fact]
        public async Task GetPublishedUnderpinningServices_MapperThrows_PropagatesFailure()
        {
            var services = new List<Service> { new() { Id = 1 } };
            _trustFrameworkRepository.GetPublishedUnderpinningServices("search").Returns(services);
            _mapper.Map<List<ServiceDto>>(services)
                .Throws(new AutoMapperMappingException("mapping failure"));

            var exception = await Assert.ThrowsAsync<AutoMapperMappingException>(() =>
                _service.GetPublishedUnderpinningServices("search"));

            Assert.Equal("mapping failure", exception.Message);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("Manual/service?")]
        public async Task GetServicesWithManualUnderinningService_SearchTextProvided_ForwardsSearchTextUnchanged(
            string searchText)
        {
            var services = new List<Service>();
            var expected = new List<ServiceDto>();
            _trustFrameworkRepository.GetServicesWithManualUnderinningService(searchText).Returns(services);
            _mapper.Map<List<ServiceDto>>(services).Returns(expected);

            var result = await _service.GetServicesWithManualUnderinningService(searchText);

            Assert.Same(expected, result);
            await _trustFrameworkRepository.Received(1)
                .GetServicesWithManualUnderinningService(searchText);
        }

        [Fact]
        public async Task GetServicesWithManualUnderinningService_NullSearchText_ForwardsNullUnchanged()
        {
            var services = new List<Service>();
            var expected = new List<ServiceDto>();
            _trustFrameworkRepository.GetServicesWithManualUnderinningService(null!).Returns(services);
            _mapper.Map<List<ServiceDto>>(services).Returns(expected);

            var result = await _service.GetServicesWithManualUnderinningService(null!);

            Assert.Same(expected, result);
            await _trustFrameworkRepository.Received(1)
                .GetServicesWithManualUnderinningService(null!);
        }

        [Fact]
        public async Task GetServicesWithManualUnderinningService_RepositoryReturnsServices_ReturnsMappedServices()
        {
            var services = new List<Service>
            {
                new() { Id = 21, ServiceName = "Manual one" },
                new() { Id = 22, ServiceName = "Manual two" }
            };
            var expected = new List<ServiceDto>
            {
                new() { Id = 21, ServiceName = "Manual one" },
                new() { Id = 22, ServiceName = "Manual two" }
            };
            _trustFrameworkRepository.GetServicesWithManualUnderinningService("manual").Returns(services);
            _mapper.Map<List<ServiceDto>>(services).Returns(expected);

            var result = await _service.GetServicesWithManualUnderinningService("manual");

            Assert.Same(expected, result);
            _mapper.Received(1).Map<List<ServiceDto>>(services);
        }

        [Fact]
        public async Task GetServicesWithManualUnderinningService_RepositoryReturnsNull_MapsNullAndReturnsMapperResult()
        {
            var expected = new List<ServiceDto>();
            _trustFrameworkRepository.GetServicesWithManualUnderinningService("search")
                .Returns((List<Service>)null!);
            _mapper.Map<List<ServiceDto>>(null!).Returns(expected);

            var result = await _service.GetServicesWithManualUnderinningService("search");

            Assert.Same(expected, result);
            _mapper.Received(1).Map<List<ServiceDto>>(null!);
        }

        [Fact]
        public async Task GetServicesWithManualUnderinningService_RepositoryThrows_PropagatesFailureWithoutMapping()
        {
            _trustFrameworkRepository.GetServicesWithManualUnderinningService("search")
                .ThrowsAsync(new InvalidOperationException("manual service failure"));

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _service.GetServicesWithManualUnderinningService("search"));

            Assert.Equal("manual service failure", exception.Message);
            _mapper.DidNotReceive().Map<List<ServiceDto>>(Arg.Any<object>());
        }

        [Fact]
        public async Task GetServicesWithManualUnderinningService_MapperThrows_PropagatesFailure()
        {
            var services = new List<Service> { new() { Id = 1 } };
            _trustFrameworkRepository.GetServicesWithManualUnderinningService("search").Returns(services);
            _mapper.Map<List<ServiceDto>>(services)
                .Throws(new AutoMapperMappingException("mapping failure"));

            var exception = await Assert.ThrowsAsync<AutoMapperMappingException>(() =>
                _service.GetServicesWithManualUnderinningService("search"));

            Assert.Equal("mapping failure", exception.Message);
        }

        [Fact]
        public async Task GetServiceDetails_RepositoryReturnsService_ReturnsMappedService()
        {
            var service = new Service { Id = 31, ServiceName = "Detailed service" };
            var expected = new ServiceDto { Id = 31, ServiceName = "Detailed service" };
            _trustFrameworkRepository.GetServiceDetails(31).Returns(service);
            _mapper.Map<ServiceDto>(service).Returns(expected);

            var result = await _service.GetServiceDetails(31);

            Assert.Same(expected, result);
            await _trustFrameworkRepository.Received(1).GetServiceDetails(31);
            _mapper.Received(1).Map<ServiceDto>(service);
        }

        [Fact]
        public async Task GetServiceDetails_RepositoryReturnsNull_MapsNullAndReturnsMapperResult()
        {
            var expected = new ServiceDto { Id = 0 };
            _trustFrameworkRepository.GetServiceDetails(999).Returns((Service)null!);
            _mapper.Map<ServiceDto>(null!).Returns(expected);

            var result = await _service.GetServiceDetails(999);

            Assert.Same(expected, result);
            _mapper.Received(1).Map<ServiceDto>(null!);
        }

        [Fact]
        public async Task GetServiceDetails_RepositoryThrows_PropagatesFailureWithoutMapping()
        {
            _trustFrameworkRepository.GetServiceDetails(31)
                .ThrowsAsync(new InvalidOperationException("service failure"));

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _service.GetServiceDetails(31));

            Assert.Equal("service failure", exception.Message);
            _mapper.DidNotReceive().Map<ServiceDto>(Arg.Any<object>());
        }

        [Fact]
        public async Task GetServiceDetails_MapperThrows_PropagatesFailure()
        {
            var service = new Service { Id = 31 };
            _trustFrameworkRepository.GetServiceDetails(31).Returns(service);
            _mapper.Map<ServiceDto>(service)
                .Throws(new AutoMapperMappingException("mapping failure"));

            var exception = await Assert.ThrowsAsync<AutoMapperMappingException>(() =>
                _service.GetServiceDetails(31));

            Assert.Equal("mapping failure", exception.Message);
        }
    }
}
