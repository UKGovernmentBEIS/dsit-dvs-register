using DVSRegister.BusinessLogic.Abstractions;
using DVSRegister.BusinessLogic.Models.Reports;
using DVSRegister.BusinessLogic.Services.CsvDownload;
using DVSRegister.CommonUtility;
using DVSRegister.CommonUtility.Models.Enums;
using DVSRegister.Data.Register;

namespace DVSRegister.BusinessLogic.Reports.CurrentRegister;

public sealed class CurrentRegisterWithContactsReport(
    IUtcClock clock) : IReportGenerator<IEnumerable<PublishedServiceForContactsReport>>
{
    public async Task<Result<CsvResult>> GenerateAsync(
        IEnumerable<PublishedServiceForContactsReport> input,
        ReportContext ctx,
        CancellationToken ct)
    {
        if (ctx.ReportType != CsvReportType.CurrentRegisterWithContacts)
            throw new InvalidOperationException($"Unexpected report type: {ctx.ReportType}");

        var rows = CurrentRegisterWithContactsProjector.Project(input);
        var stream = await CsvStreamHelper.WriteAsync(rows, new CurrentRegisterWithContactsMap(), ct);
        var name = $"dvs-register-with-contacts_{clock.UtcNow:ddMMyyyy}.csv";

        stream.Position = 0;
        return Result<CsvResult>.Ok(new CsvResult(stream, name));
    }
}