using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using DVSRegister.BusinessLogic.Reports.CurrentRegister;

namespace DVSRegister.UnitTests.Reports;

public sealed class CurrentRegisterWithContactsMapTests
{
    private static string BuildCsv(IEnumerable<CurrentRegisterWithContactsRow> rows)
    {
        var config = new CsvConfiguration(CultureInfo.InvariantCulture);
        using var writer = new StringWriter();
        using var csv = new CsvWriter(writer, config);

        csv.Context.RegisterClassMap(new CurrentRegisterWithContactsMap());
        csv.WriteRecords(rows);
        return writer.ToString();
    }

    [Fact]
    public void Headers_AreExactly9AndInOrder()
    {
        var rows = new[]
        {
            new CurrentRegisterWithContactsRow(
                "Provider A",
                "Service 1",
                "CAB X",
                "1.0",
                "Scheme 1",
                "Prim Name",
                "prim@test.com",
                "Sec Name",
                "sec@test.com")
        };

        var csv = BuildCsv(rows);
        var headerLine = csv.Split('\n')[0].TrimEnd('\r');
        var headers = headerLine.Split(',');

        Assert.Equal(9, headers.Length);
        Assert.Equal("Provider", headers[0]);
        Assert.Equal("Service name", headers[1]);
        Assert.Equal("CAB", headers[2]);
        Assert.Equal("Trust framework version", headers[3]);
        Assert.Equal("Supplementary codes", headers[4]);
        Assert.Equal("Primary contact full name", headers[5]);
        Assert.Equal("Primary contact email address", headers[6]);
        Assert.Equal("Secondary contact full name", headers[7]);
        Assert.Equal("Secondary contact email address", headers[8]);
    }

    [Fact]
    public void Csv_EscapesValuesContainingCommas()
    {
        var rows = new[]
        {
            new CurrentRegisterWithContactsRow(
                "Provider, Ltd",
                "Service, name",
                "CAB X",
                "1.0",
                "A, B",
                "Prim Name",
                "prim@test.com",
                "Sec Name",
                "sec@test.com")
        };

        var csv = BuildCsv(rows);

        Assert.Contains("\"Provider, Ltd\"", csv);
        Assert.Contains("\"Service, name\"", csv);
        Assert.Contains("\"A, B\"", csv);
    }
}