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
            return Result<CsvResult>.Fail(
                Error.Validation("Unexpected report type for CurrentRegisterWithContactsReport"));

        var rows = CurrentRegisterWithContactsProjector.Project(input);
        var name = $"dvs-register-with-contacts_{clock.UtcNow:ddMMyyyy}.csv";

        var csvResult = await CsvStreamHelper.ToCsvAsync(
            rows,
            configureCsv: null,
            configureMap: mapCtx => mapCtx.RegisterClassMap(new CurrentRegisterWithContactsMap()),
            name,
            ct);

        if (!csvResult.IsSuccess)
            return csvResult;

        csvResult.Value.Data.Position = 0;
        return Result<CsvResult>.Ok(csvResult.Value);
    }
}