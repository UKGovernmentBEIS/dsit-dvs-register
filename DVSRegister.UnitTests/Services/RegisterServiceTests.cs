using AutoMapper;
using AutoMapper.Internal;
using DVSRegister.BusinessLogic;
using DVSRegister.BusinessLogic.Services;
using DVSRegister.CommonUtility.Models;
using DVSRegister.CommonUtility.Models.Enums;
using DVSRegister.Data;
using DVSRegister.Data.Entities;
using DVSRegister.Data.Models;
using Microsoft.Extensions.Configuration;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace DVSRegister.UnitTests.Services
{
    public class RegisterServiceTests
    {
        private readonly IRegisterRepository _registerRepository;
        private readonly IMapper _mapper;

        public RegisterServiceTests()
        {
            _registerRepository = Substitute.For<IRegisterRepository>();

            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new AutoMapperProfile());
                cfg.Internal().ForAllMaps((typeMap, _) =>
                {
                    if (typeMap.MaxDepth == 0)
                    {
                        typeMap.MaxDepth = 64;
                    }
                });
            });

            _mapper = config.CreateMapper();
        }

        private static IConfiguration BuildConfiguration(bool useCloudFront = false, string cloudfrontDomain = "cdn.example.com")
        {
            var settings = new Dictionary<string, string?>
            {
                ["UseCloudFront"] = useCloudFront.ToString(),
                ["CloudfrontDomain"] = cloudfrontDomain
            };
            return new ConfigurationBuilder().AddInMemoryCollection(settings).Build();
        }

        private RegisterService CreateService(IConfiguration? configuration = null)
        {
            return new RegisterService(_registerRepository, _mapper, configuration ?? BuildConfiguration());
        }

        private static Service CreateServiceEntity(int id, string name, string? svgLink = null)
        {
            var service = RepositoryTestHelper.CreateService(1, name, 1, ServiceStatusEnum.Published, false, false, false, id);
            service.Id = id;
            if (svgLink != null)
            {
                service.TrustmarkNumber = new TrustmarkNumber
                {
                    TrustMarkNumber = "TM-" + id,
                    SvgLogoLink = svgLink
                };
            }
            return service;
        }

        #region GetProviders

        [Fact]
        public async Task GetProviders_WithProviders_ReturnsMappedPaginatedResult()
        {
            var providers = new List<ProviderProfile>
            {
                RepositoryTestHelper.CreateProviderProfile(1, "Alpha"),
                RepositoryTestHelper.CreateProviderProfile(2, "Beta")
            };
            _registerRepository.GetProviders(Arg.Any<List<int>>(), Arg.Any<List<int>>(), Arg.Any<List<int>>(),
                Arg.Any<int>(), Arg.Any<string>(), Arg.Any<string>())
                .Returns(new PaginatedResult<ProviderProfile> { Items = providers, TotalCount = 2 });

            var service = CreateService();
            var result = await service.GetProviders(new List<int> { 1 }, new List<int> { 2 }, new List<int> { 3 }, 1, "search", "name");

            Assert.Equal(2, result.TotalCount);
            Assert.Equal(2, result.Items.Count);
            Assert.Equal("Alpha", result.Items[0].RegisteredName);
            Assert.Equal("Beta", result.Items[1].RegisteredName);
        }

        [Fact]
        public async Task GetProviders_EmptyResultSet_ReturnsEmptyItemsAndZeroCount()
        {
            _registerRepository.GetProviders(Arg.Any<List<int>>(), Arg.Any<List<int>>(), Arg.Any<List<int>>(),
                Arg.Any<int>(), Arg.Any<string>(), Arg.Any<string>())
                .Returns(new PaginatedResult<ProviderProfile> { Items = new List<ProviderProfile>(), TotalCount = 0 });

            var service = CreateService();
            var result = await service.GetProviders(new List<int>(), new List<int>(), new List<int>(), 1);

            Assert.Empty(result.Items);
            Assert.Equal(0, result.TotalCount);
        }

        [Fact]
        public async Task GetProviders_EmptyFilters_PassesArgumentsToRepository()
        {
            _registerRepository.GetProviders(Arg.Any<List<int>>(), Arg.Any<List<int>>(), Arg.Any<List<int>>(),
                Arg.Any<int>(), Arg.Any<string>(), Arg.Any<string>())
                .Returns(new PaginatedResult<ProviderProfile> { Items = new List<ProviderProfile>(), TotalCount = 0 });

            var roles = new List<int>();
            var schemes = new List<int>();
            var tfVersions = new List<int>();
            var service = CreateService();
            await service.GetProviders(roles, schemes, tfVersions, 1);

            await _registerRepository.Received(1).GetProviders(roles, schemes, tfVersions, 1, "", "");
        }

        [Theory]
        [InlineData(1)]
        [InlineData(0)]
        [InlineData(int.MaxValue)]
        public async Task GetProviders_PaginationBoundaryValues_ForwardsPageNumber(int pageNum)
        {
            _registerRepository.GetProviders(Arg.Any<List<int>>(), Arg.Any<List<int>>(), Arg.Any<List<int>>(),
                Arg.Any<int>(), Arg.Any<string>(), Arg.Any<string>())
                .Returns(new PaginatedResult<ProviderProfile> { Items = new List<ProviderProfile>(), TotalCount = 0 });

            var service = CreateService();
            await service.GetProviders(new List<int>(), new List<int>(), new List<int>(), pageNum, "text", "sort");

            await _registerRepository.Received(1).GetProviders(Arg.Any<List<int>>(), Arg.Any<List<int>>(),
                Arg.Any<List<int>>(), pageNum, "text", "sort");
        }

        [Fact]
        public async Task GetProviders_SortAndFilterCombination_PassesSortByToRepository()
        {
            _registerRepository.GetProviders(Arg.Any<List<int>>(), Arg.Any<List<int>>(), Arg.Any<List<int>>(),
                Arg.Any<int>(), Arg.Any<string>(), Arg.Any<string>())
                .Returns(new PaginatedResult<ProviderProfile> { Items = new List<ProviderProfile>(), TotalCount = 0 });

            var roles = new List<int> { 1, 2 };
            var schemes = new List<int> { 3 };
            var tfVersions = new List<int> { 4 };
            var service = CreateService();
            await service.GetProviders(roles, schemes, tfVersions, 2, "abc", "desc");

            await _registerRepository.Received(1).GetProviders(roles, schemes, tfVersions, 2, "abc", "desc");
        }

        [Fact]
        public async Task GetProviders_RepositoryThrows_PropagatesException()
        {
            _registerRepository.GetProviders(Arg.Any<List<int>>(), Arg.Any<List<int>>(), Arg.Any<List<int>>(),
                Arg.Any<int>(), Arg.Any<string>(), Arg.Any<string>())
                .ThrowsAsync(new InvalidOperationException("db failure"));

            var service = CreateService();
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                service.GetProviders(new List<int>(), new List<int>(), new List<int>(), 1));
        }

        #endregion

        #region GetServices

        [Fact]
        public async Task GetServices_WithServices_ReturnsMappedPaginatedResult()
        {
            var services = new List<Service>
            {
                CreateServiceEntity(1, "Service A"),
                CreateServiceEntity(2, "Service B")
            };
            _registerRepository.GetServices(Arg.Any<List<int>>(), Arg.Any<List<int>>(), Arg.Any<List<int>>(),
                Arg.Any<int>(), Arg.Any<string>(), Arg.Any<string>())
                .Returns(new PaginatedResult<Service> { Items = services, TotalCount = 2 });

            var service = CreateService();
            var result = await service.GetServices(new List<int> { 1 }, new List<int> { 2 }, new List<int> { 3 }, 1, "s", "name");

            Assert.Equal(2, result.TotalCount);
            Assert.Equal(2, result.Items.Count);
        }

        [Fact]
        public async Task GetServices_EmptyResultSet_ReturnsEmptyItems()
        {
            _registerRepository.GetServices(Arg.Any<List<int>>(), Arg.Any<List<int>>(), Arg.Any<List<int>>(),
                Arg.Any<int>(), Arg.Any<string>(), Arg.Any<string>())
                .Returns(new PaginatedResult<Service> { Items = new List<Service>(), TotalCount = 0 });

            var service = CreateService();
            var result = await service.GetServices(new List<int>(), new List<int>(), new List<int>(), 1);

            Assert.Empty(result.Items);
            Assert.Equal(0, result.TotalCount);
        }

        [Fact]
        public async Task GetServices_EmptyFilters_PassesArgumentsToRepository()
        {
            _registerRepository.GetServices(Arg.Any<List<int>>(), Arg.Any<List<int>>(), Arg.Any<List<int>>(),
                Arg.Any<int>(), Arg.Any<string>(), Arg.Any<string>())
                .Returns(new PaginatedResult<Service> { Items = new List<Service>(), TotalCount = 0 });

            var roles = new List<int>();
            var schemes = new List<int>();
            var tfVersions = new List<int>();
            var service = CreateService();
            await service.GetServices(roles, schemes, tfVersions, 1);

            await _registerRepository.Received(1).GetServices(roles, schemes, tfVersions, 1, "", "");
        }

        [Theory]
        [InlineData(1)]
        [InlineData(0)]
        [InlineData(int.MaxValue)]
        public async Task GetServices_PaginationBoundaryValues_ForwardsPageNumber(int pageNum)
        {
            _registerRepository.GetServices(Arg.Any<List<int>>(), Arg.Any<List<int>>(), Arg.Any<List<int>>(),
                Arg.Any<int>(), Arg.Any<string>(), Arg.Any<string>())
                .Returns(new PaginatedResult<Service> { Items = new List<Service>(), TotalCount = 0 });

            var service = CreateService();
            await service.GetServices(new List<int>(), new List<int>(), new List<int>(), pageNum, "text", "sort");

            await _registerRepository.Received(1).GetServices(Arg.Any<List<int>>(), Arg.Any<List<int>>(),
                Arg.Any<List<int>>(), pageNum, "text", "sort");
        }

        [Fact]
        public async Task GetServices_WithTrustmark_AndCloudFrontDisabled_TransformsSvgLinkToDownloadEndpoint()
        {
            var services = new List<Service> { CreateServiceEntity(1, "Service A", "logo/abc.svg") };
            _registerRepository.GetServices(Arg.Any<List<int>>(), Arg.Any<List<int>>(), Arg.Any<List<int>>(),
                Arg.Any<int>(), Arg.Any<string>(), Arg.Any<string>())
                .Returns(new PaginatedResult<Service> { Items = services, TotalCount = 1 });

            var service = CreateService(BuildConfiguration(useCloudFront: false));
            var result = await service.GetServices(new List<int>(), new List<int>(), new List<int>(), 1);

            Assert.Equal("/register/download-logo?logoKey=logo/abc.svg", result.Items[0].TrustmarkNumber.SvgLogoLink);
        }

        [Fact]
        public async Task GetServices_WithTrustmark_AndCloudFrontEnabled_TransformsSvgLinkToCloudFrontUrl()
        {
            var services = new List<Service> { CreateServiceEntity(1, "Service A", "logo/abc.svg") };
            _registerRepository.GetServices(Arg.Any<List<int>>(), Arg.Any<List<int>>(), Arg.Any<List<int>>(),
                Arg.Any<int>(), Arg.Any<string>(), Arg.Any<string>())
                .Returns(new PaginatedResult<Service> { Items = services, TotalCount = 1 });

            var service = CreateService(BuildConfiguration(useCloudFront: true, cloudfrontDomain: "cdn.example.com"));
            var result = await service.GetServices(new List<int>(), new List<int>(), new List<int>(), 1);

            Assert.Equal("https://cdn.example.com/logo/abc.svg", result.Items[0].TrustmarkNumber.SvgLogoLink);
        }

        [Fact]
        public async Task GetServices_WithoutTrustmark_LeavesTrustmarkNull()
        {
            var services = new List<Service> { CreateServiceEntity(1, "Service A") };
            _registerRepository.GetServices(Arg.Any<List<int>>(), Arg.Any<List<int>>(), Arg.Any<List<int>>(),
                Arg.Any<int>(), Arg.Any<string>(), Arg.Any<string>())
                .Returns(new PaginatedResult<Service> { Items = services, TotalCount = 1 });

            var service = CreateService();
            var result = await service.GetServices(new List<int>(), new List<int>(), new List<int>(), 1);

            Assert.Null(result.Items[0].TrustmarkNumber);
        }

        [Fact]
        public async Task GetServices_MixedTrustmarks_TransformsOnlyServicesWithTrustmark()
        {
            var services = new List<Service>
            {
                CreateServiceEntity(1, "Service A", "logo/a.svg"),
                CreateServiceEntity(2, "Service B")
            };
            _registerRepository.GetServices(Arg.Any<List<int>>(), Arg.Any<List<int>>(), Arg.Any<List<int>>(),
                Arg.Any<int>(), Arg.Any<string>(), Arg.Any<string>())
                .Returns(new PaginatedResult<Service> { Items = services, TotalCount = 2 });

            var service = CreateService();
            var result = await service.GetServices(new List<int>(), new List<int>(), new List<int>(), 1);

            Assert.Equal("/register/download-logo?logoKey=logo/a.svg", result.Items[0].TrustmarkNumber.SvgLogoLink);
            Assert.Null(result.Items[1].TrustmarkNumber);
        }

        [Fact]
        public async Task GetServices_SortAndFilterCombination_PassesArgumentsToRepository()
        {
            _registerRepository.GetServices(Arg.Any<List<int>>(), Arg.Any<List<int>>(), Arg.Any<List<int>>(),
                Arg.Any<int>(), Arg.Any<string>(), Arg.Any<string>())
                .Returns(new PaginatedResult<Service> { Items = new List<Service>(), TotalCount = 0 });

            var roles = new List<int> { 1 };
            var schemes = new List<int> { 2 };
            var tfVersions = new List<int> { 3 };
            var service = CreateService();
            await service.GetServices(roles, schemes, tfVersions, 5, "query", "asc");

            await _registerRepository.Received(1).GetServices(roles, schemes, tfVersions, 5, "query", "asc");
        }

        [Fact]
        public async Task GetServices_RepositoryThrows_PropagatesException()
        {
            _registerRepository.GetServices(Arg.Any<List<int>>(), Arg.Any<List<int>>(), Arg.Any<List<int>>(),
                Arg.Any<int>(), Arg.Any<string>(), Arg.Any<string>())
                .ThrowsAsync(new InvalidOperationException("db failure"));

            var service = CreateService();
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                service.GetServices(new List<int>(), new List<int>(), new List<int>(), 1));
        }

        #endregion

        #region GetProviderWithServiceDeatils

        [Fact]
        public async Task GetProviderWithServiceDeatils_WhenProviderExists_ReturnsMappedProviderWithLastUpdated()
        {
            var provider = RepositoryTestHelper.CreateProviderProfile(1, "Alpha");
            var lastUpdated = new DateTime(2024, 1, 1);
            _registerRepository.GetProviderDetails(1).Returns(provider);
            _registerRepository.GetProviderLastUpdatedTime(1).Returns(lastUpdated);

            var service = CreateService();
            var result = await service.GetProviderWithServiceDeatils(1);

            Assert.NotNull(result);
            Assert.Equal("Alpha", result.RegisteredName);
            Assert.Equal(lastUpdated, result.LastUpdated);
        }

        [Fact]
        public async Task GetProviderWithServiceDeatils_WhenProviderNull_ReturnsNull()
        {
            _registerRepository.GetProviderDetails(99).Returns((ProviderProfile)null!);

            var service = CreateService();
            var result = await service.GetProviderWithServiceDeatils(99);

            Assert.Null(result);
            await _registerRepository.DidNotReceive().GetProviderLastUpdatedTime(Arg.Any<int>());
        }

        [Fact]
        public async Task GetProviderWithServiceDeatils_GetProviderDetailsThrows_PropagatesException()
        {
            _registerRepository.GetProviderDetails(1)
                .ThrowsAsync(new InvalidOperationException("db failure"));

            var service = CreateService();

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                service.GetProviderWithServiceDeatils(1));
            await _registerRepository.DidNotReceive().GetProviderLastUpdatedTime(Arg.Any<int>());
        }

        [Fact]
        public async Task GetProviderWithServiceDeatils_GetLastUpdatedTimeThrows_PropagatesException()
        {
            var provider = RepositoryTestHelper.CreateProviderProfile(1, "Alpha");
            _registerRepository.GetProviderDetails(1).Returns(provider);
            _registerRepository.GetProviderLastUpdatedTime(1)
                .ThrowsAsync(new InvalidOperationException("db failure"));

            var service = CreateService();

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                service.GetProviderWithServiceDeatils(1));
        }

        #endregion

        #region GetServiceDetails

        [Fact]
        public async Task GetServiceDetails_WithTrustmark_TransformsSvgLinkAndSetsLastUpdated()
        {
            var entity = CreateServiceEntity(1, "Service A", "logo/a.svg");
            var lastUpdated = new DateTime(2024, 2, 2);
            _registerRepository.GetServiceDetails(1).Returns(entity);
            _registerRepository.GetServiceLastUpdatedTime(1).Returns(lastUpdated);

            var service = CreateService(BuildConfiguration(useCloudFront: false));
            var result = await service.GetServiceDetails(1);

            Assert.Equal("/register/download-logo?logoKey=logo/a.svg", result.TrustmarkNumber.SvgLogoLink);
            Assert.Equal(lastUpdated, result.LastUpdated);
        }

        [Fact]
        public async Task GetServiceDetails_WithoutTrustmark_SetsLastUpdatedAndLeavesTrustmarkNull()
        {
            var entity = CreateServiceEntity(1, "Service A");
            var lastUpdated = new DateTime(2024, 3, 3);
            _registerRepository.GetServiceDetails(1).Returns(entity);
            _registerRepository.GetServiceLastUpdatedTime(1).Returns(lastUpdated);

            var service = CreateService();
            var result = await service.GetServiceDetails(1);

            Assert.Null(result.TrustmarkNumber);
            Assert.Equal(lastUpdated, result.LastUpdated);
        }

        [Fact]
        public async Task GetServiceDetails_GetServiceDetailsThrows_PropagatesException()
        {
            _registerRepository.GetServiceDetails(1)
                .ThrowsAsync(new InvalidOperationException("db failure"));

            var service = CreateService();

            await Assert.ThrowsAsync<InvalidOperationException>(() => service.GetServiceDetails(1));
            await _registerRepository.DidNotReceive().GetServiceLastUpdatedTime(Arg.Any<int>());
        }

        [Fact]
        public async Task GetServiceDetails_GetLastUpdatedTimeThrows_PropagatesException()
        {
            var entity = CreateServiceEntity(1, "Service A");
            _registerRepository.GetServiceDetails(1).Returns(entity);
            _registerRepository.GetServiceLastUpdatedTime(1)
                .ThrowsAsync(new InvalidOperationException("db failure"));

            var service = CreateService();

            await Assert.ThrowsAsync<InvalidOperationException>(() => service.GetServiceDetails(1));
        }

        #endregion

        #region GetUpdateLogs

        [Fact]
        public async Task GetUpdateLogs_WithGroupedLogs_ReturnsMappedGroupedResult()
        {
            var logDate = new DateTime(2024, 5, 5);
            var grouped = new PaginatedResult<GroupedResult<ActionLogs>>
            {
                Items = new List<GroupedResult<ActionLogs>>
                {
                    new()
                    {
                        LogDate = logDate,
                        Items = new List<ActionLogs> { new() { Id = 1 }, new() { Id = 2 } }
                    }
                },
                TotalCount = 2,
                LastUpdated = logDate
            };
            _registerRepository.GetUpdateLogs(1).Returns(grouped);

            var service = CreateService();
            var result = await service.GetUpdateLogs(1);

            Assert.Equal(2, result.TotalCount);
            Assert.Equal(logDate, result.LastUpdated);
            Assert.Single(result.Items);
            Assert.Equal(logDate, result.Items[0].LogDate);
            Assert.Equal(2, result.Items[0].Items.Count);
        }

        [Fact]
        public async Task GetUpdateLogs_EmptyResultSet_ReturnsEmptyItems()
        {
            var grouped = new PaginatedResult<GroupedResult<ActionLogs>>
            {
                Items = new List<GroupedResult<ActionLogs>>(),
                TotalCount = 0,
                LastUpdated = null
            };
            _registerRepository.GetUpdateLogs(1).Returns(grouped);

            var service = CreateService();
            var result = await service.GetUpdateLogs(1);

            Assert.Empty(result.Items);
            Assert.Equal(0, result.TotalCount);
            Assert.Null(result.LastUpdated);
        }

        [Fact]
        public async Task GetUpdateLogs_RepositoryThrows_PropagatesException()
        {
            _registerRepository.GetUpdateLogs(1)
                .ThrowsAsync(new InvalidOperationException("db failure"));

            var service = CreateService();

            await Assert.ThrowsAsync<InvalidOperationException>(() => service.GetUpdateLogs(1));
        }

        #endregion

        #region GetLastUpdatedDate

        [Fact]
        public async Task GetLastUpdatedDate_WhenDatePresent_ReturnsDate()
        {
            var date = new DateTime(2024, 6, 6);
            _registerRepository.GetLastUpdatedDate().Returns(date);

            var service = CreateService();
            var result = await service.GetLastUpdatedDate();

            Assert.Equal(date, result);
        }

        [Fact]
        public async Task GetLastUpdatedDate_WhenNull_ReturnsNull()
        {
            _registerRepository.GetLastUpdatedDate().Returns((DateTime?)null);

            var service = CreateService();
            var result = await service.GetLastUpdatedDate();

            Assert.Null(result);
        }

        [Fact]
        public async Task GetLastUpdatedDate_RepositoryThrows_PropagatesException()
        {
            _registerRepository.GetLastUpdatedDate()
                .ThrowsAsync(new InvalidOperationException("db failure"));

            var service = CreateService();

            await Assert.ThrowsAsync<InvalidOperationException>(() => service.GetLastUpdatedDate());
        }

        #endregion

        #region GetPublishedServices

        [Fact]
        public async Task GetPublishedServices_WithServices_ReturnsMappedList()
        {
            var services = new List<Service>
            {
                CreateServiceEntity(1, "Service A"),
                CreateServiceEntity(2, "Service B")
            };
            _registerRepository.GetPublishedServices().Returns(services);

            var service = CreateService();
            var result = await service.GetPublishedServices();

            Assert.Equal(2, result.Count);
            Assert.Equal("Service A", result[0].ServiceName);
        }

        [Fact]
        public async Task GetPublishedServices_EmptyResultSet_ReturnsEmptyList()
        {
            _registerRepository.GetPublishedServices().Returns(new List<Service>());

            var service = CreateService();
            var result = await service.GetPublishedServices();

            Assert.Empty(result);
        }

        [Fact]
        public async Task GetPublishedServices_RepositoryThrows_PropagatesException()
        {
            _registerRepository.GetPublishedServices().ThrowsAsync(new InvalidOperationException("db failure"));

            var service = CreateService();
            await Assert.ThrowsAsync<InvalidOperationException>(() => service.GetPublishedServices());
        }

        #endregion

        #region GetSVGLogoEndPoint

        [Fact]
        public void GetSVGLogoEndPoint_WhenCloudFrontDisabled_ReturnsDownloadEndpoint()
        {
            var service = CreateService(BuildConfiguration(useCloudFront: false));
            var result = service.GetSVGLogoEndPoint("logo/a.svg");
            Assert.Equal("/register/download-logo?logoKey=logo/a.svg", result);
        }

        [Fact]
        public void GetSVGLogoEndPoint_WhenCloudFrontEnabled_ReturnsCloudFrontUrl()
        {
            var service = CreateService(BuildConfiguration(useCloudFront: true, cloudfrontDomain: "cdn.example.com"));
            var result = service.GetSVGLogoEndPoint("logo/a.svg");
            Assert.Equal("https://cdn.example.com/logo/a.svg", result);
        }

        [Fact]
        public void GetSVGLogoEndPoint_WhenConfigMissing_DefaultsToDownloadEndpoint()
        {
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>()).Build();
            var service = CreateService(configuration);
            var result = service.GetSVGLogoEndPoint("logo/a.svg");
            Assert.Equal("/register/download-logo?logoKey=logo/a.svg", result);
        }

        #endregion
    }
}
