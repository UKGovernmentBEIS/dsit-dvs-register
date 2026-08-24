using DVSRegister.BusinessLogic.Reports;

namespace DVSRegister.UnitTests.Reports;

public class ReportFactoryTests
{
    [Fact]
    public void GetCurrentRegisterGenerator_Returns_Instance()
    {
        var gen = new CurrentRegisterReportGenerator();
        var factory = new ReportFactory(gen);

        var result = factory.GetCurrentRegisterGenerator();

        Assert.Same(gen, result);
    }
}