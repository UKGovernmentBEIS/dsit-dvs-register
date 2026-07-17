using System.Text;
using DVSRegister.BusinessLogic.Models.Reports;
using DVSRegister.BusinessLogic.Reports;
using DVSRegister.BusinessLogic.Services.CAB;
using DVSRegister.BusinessLogic.Services.Register;
using DVSRegister.CommonUtility;
using DVSRegister.CommonUtility.Models;
using DVSRegister.CommonUtility.Models.Enums;
using DVSRegister.Controllers;
using DVSRegister.Data.Register;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace DVSRegister.UnitTests.Controllers;

public sealed class DownloadRegisterWithContactsEndpointTests
{
    [Fact]
    public async Task DownloadRegisterWithContacts_Returns200WithCsvHeaderAndContent()
    {
        var generator = Substitute.For<IReportGenerator<IEnumerable<PublishedServiceForContactsReport>>>();

        var stream = new MemoryStream("Provider,CAB\r\nTest Provider,TestCAB"u8.ToArray());
        generator.GenerateAsync(
                Arg.Any<IEnumerable<PublishedServiceForContactsReport>>(),
                Arg.Any<ReportContext>(),
                Arg.Any<CancellationToken>())
            .Returns(Result<CsvResult>.Ok(new CsvResult(stream, "test.csv")));

        var reportFactory = Substitute.For<ReportFactory>();
        reportFactory
            .GetReport(CsvReportType.CurrentRegisterWithContacts)
            .Returns(generator);

        var query = Substitute.For<IPublishedServicesQuery>();
        query.GetAsync(Arg.Any<CancellationToken>())
            .Returns(new[]
            {
                new PublishedServiceForContactsReport(
                    "Test Provider", "Svc", "Cab", "1.0",
                    Array.Empty<string>(), "P", "p@t.com", "S", "s@t.com")
            });

        var controller = CreateController(reportFactory, query);
        var result = await controller.DownloadRegisterWithContacts(CancellationToken.None);

        var fileResult = Assert.IsType<FileContentResult>(result);
        Assert.Equal("text/csv", fileResult.ContentType);

        var content = Encoding.UTF8.GetString(fileResult.FileContents);
        Assert.Contains("Test Provider", content);
    }

    [Fact]
    public async Task DownloadRegisterWithContacts_UsesFileNameFromReport()
    {
        var generator = Substitute.For<IReportGenerator<IEnumerable<PublishedServiceForContactsReport>>>();

        var stream = new MemoryStream();
        generator.GenerateAsync(
                Arg.Any<IEnumerable<PublishedServiceForContactsReport>>(),
                Arg.Any<ReportContext>(),
                Arg.Any<CancellationToken>())
            .Returns(Result<CsvResult>.Ok(new CsvResult(stream, "dvs-register-with-contacts_01012025.csv")));

        var reportFactory = Substitute.For<ReportFactory>();
        reportFactory
            .GetReport(CsvReportType.CurrentRegisterWithContacts)
            .Returns(generator);

        var query = Substitute.For<IPublishedServicesQuery>();
        query.GetAsync(Arg.Any<CancellationToken>())
            .Returns(new[]
            {
                new PublishedServiceForContactsReport(
                    "P", "S", "C", "1.0",
                    Array.Empty<string>(), "P", "p@t.com", "S", "s@t.com")
            });

        var controller = CreateController(reportFactory, query);
        var result = await controller.DownloadRegisterWithContacts(CancellationToken.None);

        var fileResult = Assert.IsType<FileResult>(result);
        Assert.Equal("dvs-register-with-contacts_01012025.csv", fileResult.FileDownloadName);
    }

    private RegisterController CreateController(ReportFactory reportFactory,
        IPublishedServicesQuery publishedServicesQuery)
    {
        var registerService = Substitute.For<IRegisterService>();
        var cabService = Substitute.For<ICabService>();
        var bucketService = Substitute.For<IBucketService>();
        var logger = Substitute.For<ILogger<RegisterController>>();
        var config = Options.Create(new S3Configuration
            { BucketName = "test-bucket", LogoBucketName = "test-logo-bucket" });

        var controller = new RegisterController(
            registerService,
            cabService,
            bucketService,
            config,
            logger,
            reportFactory,
            publishedServicesQuery);

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                RequestServices = Substitute.For<IServiceProvider>()
            }
        };

        return controller;
    }
}