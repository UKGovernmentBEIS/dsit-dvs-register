using DVSRegister.BusinessLogic.Models.CAB;
using DVSRegister.BusinessLogic.Models.Reports;
using DVSRegister.BusinessLogic.Reports.CurrentRegister;
using DVSRegister.CommonUtility.Models.Enums;

namespace DVSRegister.BusinessLogic.Reports;

public sealed class ReportFactory(
    CurrentRegisterReportGenerator currentRegister,
    CurrentRegisterWithContactsReport currentRegisterWithContacts)
{
    public IReportGenerator<IEnumerable<ServiceDto>> GetCurrentRegisterGenerator() => currentRegister;

    public IReport GetReport(CsvReportType reportType)
    {
        return reportType switch
        {
            CsvReportType.CurrentRegister => currentRegister,
            CsvReportType.CurrentRegisterWithContacts => currentRegisterWithContacts,
            _ => throw new ArgumentOutOfRangeException(nameof(reportType), reportType, "Unsupported report type")
        };
    }
}