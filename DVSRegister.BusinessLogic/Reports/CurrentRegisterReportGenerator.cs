using DVSRegister.BusinessLogic.Models.CAB;
using DVSRegister.BusinessLogic.Models.Reports;
using DVSRegister.BusinessLogic.Services;
using DVSRegister.BusinessLogic.Services.CsvDownload;
using DVSRegister.CommonUtility;

namespace DVSRegister.BusinessLogic.Reports;

public sealed class CurrentRegisterReportGenerator : IReportGenerator<IEnumerable<ServiceDto>>
{
    public async Task<Result<CsvResult>> GenerateAsync(IEnumerable<ServiceDto> input, ReportContext ctx,
        CancellationToken ct)
    {
        var result = await CsvStreamHelper.ToCsvAsync(input, null, csv => csv.RegisterClassMap<PfrCsvMap>(),
            $"dvs-register_{DateTime.UtcNow:ddMMyyyy}.csv", ct);
        return result;
    }
}