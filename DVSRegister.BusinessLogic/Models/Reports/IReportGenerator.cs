using DVSRegister.CommonUtility;

namespace DVSRegister.BusinessLogic.Models.Reports;

public interface IReport
{
}

public interface IReportGenerator<TInput> : IReport
{
    Task<Result<CsvResult>> GenerateAsync(TInput input, ReportContext ctx, CancellationToken ct);
}