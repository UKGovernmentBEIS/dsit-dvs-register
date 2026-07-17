using DVSRegister.BusinessLogic.Abstractions;
using DVSRegister.BusinessLogic.Models.Reports;
using DVSRegister.BusinessLogic.Reports.CurrentRegister;
using DVSRegister.CommonUtility.Models;
using DVSRegister.CommonUtility.Models.Enums;
using DVSRegister.Data;
using DVSRegister.Data.Entities;
using DVSRegister.Data.Register;
using DVSRegister.UnitTests.Repository;
using Microsoft.EntityFrameworkCore;

namespace DVSRegister.UnitTests.Reports;

[Collection("Postgres Collection")]
public sealed class CurrentRegisterWithContactsIntegrationTests(PostgresTestFixture fixture)
{
    [Fact]
    public async Task GenerateAsync_GeneratesCsvWithContacts_WhenServicesExist()
    {
        await using var context = CreateDbContext();

        var provider = new ProviderProfile
        {
            Id = 100,
            RegisteredName = "Integration Provider",
            IsInRegister = true,
            PrimaryContactFullName = "Primary Contact",
            PrimaryContactEmail = "primary@example.com",
            SecondaryContactFullName = "Secondary Contact",
            SecondaryContactEmail = "secondary@example.com"
        };

        var service = new Service
        {
            Id = 1000,
            ProviderProfileId = provider.Id,
            ServiceName = "Integration Service",
            IsInRegister = true,
            CabUserId = 1,
            TrustFrameworkVersionId = 1,
            ServiceStatus = ServiceStatusEnum.Published
        };

        context.ProviderProfile.Add(provider);
        context.Service.Add(service);
        await context.SaveChangesAsync();

        var query = new PublishedServicesQuery(context);
        var services = await query.GetAsync(CancellationToken.None);
        var report = new CurrentRegisterWithContactsReport(new SystemUtcClock());
        var ctx = new ReportContext(CsvReportType.CurrentRegisterWithContacts, null, null);
        var result = await report.GenerateAsync(services, ctx, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("text/csv", result.Value.ContentType);

        result.Value.Data.Position = 0;
        using var reader = new StreamReader(result.Value.Data);
        var csv = await reader.ReadToEndAsync();

        Assert.Contains("Provider", csv);
        Assert.Contains("Integration Provider", csv);
        Assert.Contains("Integration Service", csv);
        Assert.Contains("Primary contact email address", csv);
        Assert.Contains("primary@example.com", csv);
        Assert.Contains("Secondary contact email address", csv);
        Assert.Contains("secondary@example.com", csv);
    }

    [Fact]
    public async Task GenerateAsync_ExcludesNonRegisteredServices()
    {
        await using var context = CreateDbContext();

        var provider = new ProviderProfile
        {
            Id = 200,
            RegisteredName = "Hidden Provider",
            IsInRegister = true
        };

        var publishedService = new Service
        {
            Id = 2001,
            ProviderProfileId = provider.Id,
            ServiceName = "Published",
            IsInRegister = true,
            CabUserId = 1,
            TrustFrameworkVersionId = 1,
            ServiceStatus = ServiceStatusEnum.Published
        };

        var notPublishedService = new Service
        {
            Id = 2002,
            ProviderProfileId = provider.Id,
            ServiceName = "NotPublished",
            IsInRegister = false,
            CabUserId = 1,
            TrustFrameworkVersionId = 1,
            ServiceStatus = ServiceStatusEnum.Published
        };

        context.ProviderProfile.Add(provider);
        context.Service.Add(publishedService);
        context.Service.Add(notPublishedService);
        await context.SaveChangesAsync();

        var query = new PublishedServicesQuery(context);
        var services = await query.GetAsync(CancellationToken.None);
        var report = new CurrentRegisterWithContactsReport(new SystemUtcClock());
        var ctx = new ReportContext(CsvReportType.CurrentRegisterWithContacts, null, null);
        var result = await report.GenerateAsync(services, ctx, CancellationToken.None);

        Assert.True(result.IsSuccess);

        result.Value.Data.Position = 0;
        using var reader = new StreamReader(result.Value.Data);
        var csv = await reader.ReadToEndAsync();

        Assert.Contains("Published", csv);
        Assert.DoesNotContain("NotPublished", csv);
    }

    private DVSRegisterDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<DVSRegisterDbContext>()
            .UseNpgsql(fixture.GetConnectionString())
            .Options;

        return new DVSRegisterDbContext(options);
    }
}