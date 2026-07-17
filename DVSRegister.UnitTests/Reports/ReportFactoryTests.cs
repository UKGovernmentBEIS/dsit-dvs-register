using DVSRegister.BusinessLogic.Abstractions;
using DVSRegister.BusinessLogic.Reports;
using DVSRegister.BusinessLogic.Reports.CurrentRegister;
using NSubstitute;

namespace DVSRegister.UnitTests.Reports;

public class ReportFactoryTests
{
    [Fact]
    public void GetCurrentRegisterGenerator_Returns_Instance()
    {
        var gen = Substitute.For<CurrentRegisterReportGenerator>();
        var contacts = new CurrentRegisterWithContactsReport(Substitute.For<IUtcClock>());
        var factory = new ReportFactory(gen, contacts);

        var result = factory.GetCurrentRegisterGenerator();

        Assert.Same(gen, result);
    }
}