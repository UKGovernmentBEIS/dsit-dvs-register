using DVSRegister.CommonUtility.Models;
using DVSRegister.Data;
using DVSRegister.Data.Entities;
using DVSRegister.Data.Register;
using Microsoft.EntityFrameworkCore;

namespace DVSRegister.UnitTests.Repository;

[Collection("Postgres Collection")]
public sealed class PublishedServicesQueryTests(PostgresTestFixture fixture)
{
    [Fact]
    public async Task GetAsync_OnlyReturnsServicesWithIsInRegisterTrue()
    {
        await using var context = CreateDbContext();

        var provider = new ProviderProfile
        {
            Id = 1,
            RegisteredName = "Alpha Provider",
            IsInRegister = true,
            PrimaryContactFullName = "P1",
            PrimaryContactEmail = "p1@a.com",
            SecondaryContactFullName = "S1",
            SecondaryContactEmail = "s1@a.com"
        };

        var inService = new Service
        {
            Id = 1,
            ProviderProfileId = provider.Id,
            ServiceName = "In Register",
            IsInRegister = true,
            CabUserId = 1,
            TrustFrameworkVersionId = 1,
            ServiceStatus = ServiceStatusEnum.Published
        };

        var outService = new Service
        {
            Id = 2,
            ProviderProfileId = provider.Id,
            ServiceName = "Out Register",
            IsInRegister = false,
            CabUserId = 1,
            TrustFrameworkVersionId = 1,
            ServiceStatus = ServiceStatusEnum.Published
        };

        context.ProviderProfile.Add(provider);
        context.Service.Add(inService);
        context.Service.Add(outService);
        await context.SaveChangesAsync();

        var query = new PublishedServicesQuery(context);
        var results = await query.GetAsync(CancellationToken.None);

        Assert.Single(results);
        Assert.Equal("In Register", results[0].ServiceName);
    }

    [Fact]
    public async Task GetAsync_OrdersByProviderRegisteredName()
    {
        await using var context = CreateDbContext();

        var providerZ = new ProviderProfile
        {
            Id = 10,
            RegisteredName = "Zebra Provider",
            IsInRegister = true
        };

        var providerA = new ProviderProfile
        {
            Id = 20,
            RegisteredName = "Alpha Provider",
            IsInRegister = true
        };

        context.ProviderProfile.AddRange(providerZ, providerA);
        await context.SaveChangesAsync();

        var serviceZ = new Service
        {
            Id = 1,
            ProviderProfileId = providerZ.Id,
            ServiceName = "Svc Z",
            IsInRegister = true,
            CabUserId = 1,
            TrustFrameworkVersionId = 1,
            ServiceStatus = ServiceStatusEnum.Published
        };

        var serviceA = new Service
        {
            Id = 2,
            ProviderProfileId = providerA.Id,
            ServiceName = "Svc A",
            IsInRegister = true,
            CabUserId = 1,
            TrustFrameworkVersionId = 1,
            ServiceStatus = ServiceStatusEnum.Published
        };

        context.Service.AddRange(serviceZ, serviceA);
        await context.SaveChangesAsync();

        var query = new PublishedServicesQuery(context);
        var results = await query.GetAsync(CancellationToken.None);

        Assert.Equal(2, results.Count);
        Assert.Equal("Alpha Provider", results[0].ProviderRegisteredName);
        Assert.Equal("Zebra Provider", results[1].ProviderRegisteredName);
    }

    [Fact]
    public async Task GetAsync_IncludesSchemesWithoutCustomDisplay()
    {
        await using var context = CreateDbContext();

        var provider = new ProviderProfile
        {
            Id = 30,
            RegisteredName = "Scheme Provider",
            IsInRegister = true
        };
        context.ProviderProfile.Add(provider);
        await context.SaveChangesAsync();

        var scheme = new SupplementaryScheme
        {
            Id = 100,
            SchemeName = "Test Scheme",
            TrustFrameworkVersionId = 1
        };
        context.SupplementaryScheme.Add(scheme);
        await context.SaveChangesAsync();

        var service = new Service
        {
            Id = 3,
            ProviderProfileId = provider.Id,
            ServiceName = "With Schemes",
            IsInRegister = true,
            CabUserId = 1,
            TrustFrameworkVersionId = 1,
            ServiceStatus = ServiceStatusEnum.Published,
            ServiceSupSchemeMapping = new List<ServiceSupSchemeMapping>
            {
                new()
                {
                    SupplementarySchemeId = scheme.Id,
                    ServiceSupSchemeCustomDisplayId = null
                }
            }
        };

        context.Service.Add(service);
        await context.SaveChangesAsync();

        var query = new PublishedServicesQuery(context);
        var results = await query.GetAsync(CancellationToken.None);

        var row = results.First(r => r.ServiceName == "With Schemes");
        Assert.Contains("Test Scheme", row.SchemeNames);
    }

    [Fact]
    public async Task GetAsync_IncludesProviderContactsAndCabName()
    {
        await using var context = CreateDbContext();

        var provider = new ProviderProfile
        {
            Id = 40,
            RegisteredName = "Contact Provider",
            IsInRegister = true,
            PrimaryContactFullName = "Primary One",
            PrimaryContactEmail = "primary@one.com",
            SecondaryContactFullName = "Secondary Two",
            SecondaryContactEmail = "secondary@two.com"
        };

        context.ProviderProfile.Add(provider);
        await context.SaveChangesAsync();

        var service = new Service
        {
            Id = 4,
            ProviderProfileId = provider.Id,
            ServiceName = "Contacts Svc",
            IsInRegister = true,
            CabUserId = 1,
            TrustFrameworkVersionId = 1,
            ServiceStatus = ServiceStatusEnum.Published
        };

        context.Service.Add(service);
        await context.SaveChangesAsync();

        var query = new PublishedServicesQuery(context);
        var results = await query.GetAsync(CancellationToken.None);

        var row = results.First();
        Assert.Equal("Primary One", row.PrimaryContactFullName);
        Assert.Equal("primary@one.com", row.PrimaryContactEmail);
        Assert.Equal("Secondary Two", row.SecondaryContactFullName);
        Assert.Equal("secondary@two.com", row.SecondaryContactEmail);
    }

    private DVSRegisterDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<DVSRegisterDbContext>()
            .UseNpgsql(fixture.GetConnectionString())
            .Options;

        return new DVSRegisterDbContext(options);
    }
}