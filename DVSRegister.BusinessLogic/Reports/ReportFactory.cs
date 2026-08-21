using DVSRegister.BusinessLogic.Models.CAB;
using DVSRegister.BusinessLogic.Models.Reports;

namespace DVSRegister.BusinessLogic.Reports;

public interface IReportFactory
{
    IReportGenerator<IEnumerable<ServiceDto>> GetCurrentRegisterGenerator();
}

public sealed class ReportFactory(
    CurrentRegisterReportGenerator currentRegister) : IReportFactory
{
    public IReportGenerator<IEnumerable<ServiceDto>> GetCurrentRegisterGenerator() => currentRegister;
}