using DVSRegister.BusinessLogic.Reports.CurrentRegister;
using DVSRegister.Data.Register;

namespace DVSRegister.UnitTests.Reports;

public sealed class CurrentRegisterWithContactsProjectorTests
{
    [Fact]
    public void Project_MapsAllFieldsCorrectly()
    {
        var input = new[]
        {
            new PublishedServiceForContactsReport(
                "Provider A",
                "Svc 1",
                "Cab X",
                "1.0",
                new List<string> { "Scheme 1" },
                "Prim Name",
                "prim@test.com",
                "Sec Name",
                "sec@test.com")
        };

        var rows = CurrentRegisterWithContactsProjector.Project(input);

        Assert.Single(rows);
        var row = rows[0];
        Assert.Equal("Provider A", row.Provider);
        Assert.Equal("Svc 1", row.ServiceName);
        Assert.Equal("Cab X", row.Cab);
        Assert.Equal("1.0", row.TrustFrameworkVersion);
        Assert.Equal("Scheme 1", row.SupplementaryCodes);
        Assert.Equal("Prim Name", row.PrimaryContactFullName);
        Assert.Equal("prim@test.com", row.PrimaryContactEmail);
        Assert.Equal("Sec Name", row.SecondaryContactFullName);
        Assert.Equal("sec@test.com", row.SecondaryContactEmail);
    }

    [Fact]
    public void Project_JoinsMultipleSchemesWithComma()
    {
        var input = new[]
        {
            new PublishedServiceForContactsReport(
                "P",
                "S",
                "C",
                "1.0",
                new List<string> { "A", "B" },
                null, null, null, null)
        };

        var rows = CurrentRegisterWithContactsProjector.Project(input);

        Assert.Equal("A, B", rows[0].SupplementaryCodes);
    }

    [Fact]
    public void Project_EmptySchemesResultsInNull()
    {
        var input = new[]
        {
            new PublishedServiceForContactsReport(
                "P",
                "S",
                "C",
                "1.0",
                Array.Empty<string>(),
                null, null, null, null)
        };

        var rows = CurrentRegisterWithContactsProjector.Project(input);

        Assert.Null(rows[0].SupplementaryCodes);
    }

    [Fact]
    public void Project_NullContactsArePreserved()
    {
        var input = new[]
        {
            new PublishedServiceForContactsReport(
                "P",
                "S",
                "C",
                "1.0",
                Array.Empty<string>(),
                null, null, null, null)
        };

        var rows = CurrentRegisterWithContactsProjector.Project(input);

        Assert.Null(rows[0].PrimaryContactFullName);
        Assert.Null(rows[0].SecondaryContactEmail);
    }

    [Fact]
    public void Project_MultipleServicesFromSameProviderProducesMultipleRows()
    {
        var input = new[]
        {
            new PublishedServiceForContactsReport("Same Provider", "Svc1", "C", "1.0",
                Array.Empty<string>(), null, null, null, null),
            new PublishedServiceForContactsReport("Same Provider", "Svc2", "C", "1.0",
                Array.Empty<string>(), null, null, null, null)
        };

        var rows = CurrentRegisterWithContactsProjector.Project(input);

        Assert.Equal(2, rows.Count);
        Assert.Equal("Svc1", rows[0].ServiceName);
        Assert.Equal("Svc2", rows[1].ServiceName);
    }

    [Fact]
    public void Project_EmptyInputReturnsEmptyList()
    {
        var rows = CurrentRegisterWithContactsProjector.Project(Array.Empty<PublishedServiceForContactsReport>());

        Assert.Empty(rows);
    }
}