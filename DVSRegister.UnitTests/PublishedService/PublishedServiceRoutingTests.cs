using DVSRegister.CommonUtility;
using DVSRegister.Middleware;
using DVSRegister.PublishedService;
using Microsoft.AspNetCore.Http;
using NSubstitute;

namespace DVSRegister.UnitTests.PublishedService
{
    public class PublishedServiceRoutingTests
    {
        private static readonly Guid ProviderGuid = Guid.NewGuid();
        private static readonly Guid ServiceGuid = Guid.NewGuid();

        [Theory]
        [InlineData("/service-details")]
        [InlineData("/register/service-details")]
        public async Task Middleware_LegacyPath_RedirectsToStableUrl(string path)
        {
            var resolver = Substitute.For<IResolveLegacyServiceLink>();
            resolver.HandleAsync(42, Arg.Any<CancellationToken>())
                .Returns(Result<string>.Ok(ServiceLinks.Latest(ProviderGuid, ServiceGuid)));
            var context = CreateContext(path, "?serviceId=42");
            var nextCalled = false;

            await new LegacyServiceLinkRedirectMiddleware(_ => { nextCalled = true; return Task.CompletedTask; })
                .InvokeAsync(context, resolver);

            Assert.False(nextCalled);
            Assert.Equal(StatusCodes.Status302Found, context.Response.StatusCode);
            Assert.Equal(ServiceLinks.Latest(ProviderGuid, ServiceGuid), context.Response.Headers.Location);
        }

        [Theory]
        [InlineData("?serviceId=abc")]
        [InlineData("")]
        public async Task Middleware_InvalidServiceId_RedirectsToNotFound(string query)
        {
            var resolver = Substitute.For<IResolveLegacyServiceLink>();
            var context = CreateContext("/service-details", query);

            await new LegacyServiceLinkRedirectMiddleware(_ => Task.CompletedTask).InvokeAsync(context, resolver);

            Assert.Equal("/register/page-not-found", context.Response.Headers.Location);
            await resolver.DidNotReceive().HandleAsync(Arg.Any<int>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Middleware_UnpublishedService_RedirectsToNotFound()
        {
            var resolver = Substitute.For<IResolveLegacyServiceLink>();
            resolver.HandleAsync(7, Arg.Any<CancellationToken>())
                .Returns(Result<string>.Fail(Error.Custom(PublishedServiceErrorCodes.NotFound, "missing")));
            var context = CreateContext("/register/service-details", "?serviceId=7");

            await new LegacyServiceLinkRedirectMiddleware(_ => Task.CompletedTask).InvokeAsync(context, resolver);

            Assert.Equal("/register/page-not-found", context.Response.Headers.Location);
        }

        [Theory]
        [InlineData("/register/all-services", "GET")]
        [InlineData("/service-details", "POST")]
        public async Task Middleware_OtherRequests_PassThrough(string path, string method)
        {
            var resolver = Substitute.For<IResolveLegacyServiceLink>();
            var context = CreateContext(path, "?serviceId=1", method);
            var nextCalled = false;

            await new LegacyServiceLinkRedirectMiddleware(_ => { nextCalled = true; return Task.CompletedTask; })
                .InvokeAsync(context, resolver);

            Assert.True(nextCalled);
            await resolver.DidNotReceive().HandleAsync(Arg.Any<int>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public void PreviousServiceUrl_ServicePageReferer_ReturnsPath()
        {
            var otherPath = ServiceLinks.Latest(Guid.NewGuid(), Guid.NewGuid());
            var request = CreateContext(ServiceLinks.Latest(ProviderGuid, ServiceGuid), "", referer: $"https://localhost{otherPath}").Request;

            Assert.Equal(otherPath, PublishedServiceNavigation.GetPreviousServiceUrl(request));
        }

        [Theory]
        [InlineData("https://evil.example/organisation/x/service/y/version/latest")]
        [InlineData("https://localhost/register/all-services")]
        [InlineData("not a url")]
        [InlineData("")]
        public void PreviousServiceUrl_OtherHostOrPage_ReturnsNull(string referer)
        {
            var request = CreateContext(ServiceLinks.Latest(ProviderGuid, ServiceGuid), "", referer: referer).Request;

            Assert.Null(PublishedServiceNavigation.GetPreviousServiceUrl(request));
        }

        [Fact]
        public void PreviousServiceUrl_RefererIsSamePage_ReturnsNull()
        {
            var path = ServiceLinks.Latest(ProviderGuid, ServiceGuid);
            var request = CreateContext(path, "", referer: $"https://localhost{path}").Request;

            Assert.Null(PublishedServiceNavigation.GetPreviousServiceUrl(request));
        }

        [Fact]
        public void BackUrl_RefererIsOwnUnderpinningService_ReturnsNull()
        {
            var underpinning = new UnderpinningServiceViewModel(Guid.NewGuid(), Guid.NewGuid(), "Underpinning", "Provider", "CAB", DateTime.UtcNow);
            var underpinningPath = ServiceLinks.Latest(underpinning.ProviderGuid, underpinning.ServiceGuid);
            var request = CreateContext(ServiceLinks.Latest(ProviderGuid, ServiceGuid), "", referer: $"https://localhost{underpinningPath}").Request;

            Assert.Null(PublishedServiceNavigation.GetBackUrl(request, underpinning));
        }

        [Fact]
        public void BackUrl_RefererIsAnotherService_ReturnsReferer()
        {
            var underpinning = new UnderpinningServiceViewModel(Guid.NewGuid(), Guid.NewGuid(), "Underpinning", "Provider", "CAB", DateTime.UtcNow);
            var otherPath = ServiceLinks.Latest(Guid.NewGuid(), Guid.NewGuid());
            var request = CreateContext(ServiceLinks.Latest(ProviderGuid, ServiceGuid), "", referer: $"https://localhost{otherPath}").Request;

            Assert.Equal(otherPath, PublishedServiceNavigation.GetBackUrl(request, underpinning));
        }

        private static DefaultHttpContext CreateContext(string path, string query, string method = "GET", string? referer = null)
        {
            var context = new DefaultHttpContext();
            context.Request.Method = method;
            context.Request.Path = path;
            context.Request.QueryString = new QueryString(query);
            context.Request.Host = new HostString("localhost");
            if (referer is not null)
                context.Request.Headers.Referer = referer;
            return context;
        }
    }
}
