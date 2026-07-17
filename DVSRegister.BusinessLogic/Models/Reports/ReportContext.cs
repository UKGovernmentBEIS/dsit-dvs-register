using DVSRegister.CommonUtility.Models.Enums;

namespace DVSRegister.BusinessLogic.Models.Reports;

public sealed record ReportContext(CsvReportType ReportType, DateTime? FromDate, DateTime? ToDate);