using DVSRegister.BusinessLogic.Abstractions;
using DVSRegister.BusinessLogic.Models.Reports;
using DVSRegister.BusinessLogic.Reports.CurrentRegister;
using DVSRegister.CommonUtility.Models.Enums;
using DVSRegister.Data.Register;
using NSubstitute;

namespace DVSRegister.UnitTests.Reports;

public sealed class CurrentRegisterWithContactsReportTests
{
    [Fact]
    public async Task GenerateAsync_ReturnsCsvFileWithCorrectNameAndRows()
    {
        var fixedTime = new DateTime(2024, 7, 15);
        var clock = Substitute.For<IUtcClock>();
        clock.UtcNow.Returns(fixedTime);

        var data = new[]
        {
            new PublishedServiceForContactsReport(
                "Provider A",
                "Svc 1",
                "Cab X",
                "1.0",
                Array.Empty<string>(),
                "Prim",
                "p@t.com",
                "Sec",
                "s@t.com")
        };

        var report = new CurrentRegisterWithContactsReport(clock);
        var ctx = new ReportContext(CsvReportType.CurrentRegisterWithContacts, null, null);
        var result = await report.GenerateAsync(data, ctx, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal("dvs-register-with-contacts_15072024.csv", result.Value.FileName);
        Assert.Equal("text/csv", result.Value.ContentType);

        result.Value.Data.Position = 0;
        using var reader = new StreamReader(result.Value.Data);
        var csv = await reader.ReadToEndAsync();

        Assert.Contains("Provider", csv);
        Assert.Contains("Provider A", csv);
        Assert.Contains("Primary contact email address", csv);
        Assert.Contains("p@t.com", csv);
    }

    [Fact]
    public async Task GenerateAsync_EmptyRegister_ReturnsHeaderOnly()
    {
        var clock = Substitute.For<IUtcClock>();
        clock.UtcNow.Returns(new DateTime(2024, 1, 1));

        var report = new CurrentRegisterWithContactsReport(clock);
        var ctx = new ReportContext(CsvReportType.CurrentRegisterWithContacts, null, null);
        var result = await report.GenerateAsync(Array.Empty<PublishedServiceForContactsReport>(), ctx,
            CancellationToken.None);

        Assert.True(result.IsSuccess);

        result.Value.Data.Position = 0;
        using var reader = new StreamReader(result.Value.Data);
        var csv = await reader.ReadToEndAsync();
        var lines = csv.Split('\n').Where(l => !string.IsNullOrWhiteSpace(l)).ToList();

        Assert.Single(lines);
        Assert.Contains("Provider", lines[0]);
    }
}