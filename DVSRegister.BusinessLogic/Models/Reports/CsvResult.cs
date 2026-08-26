namespace DVSRegister.BusinessLogic.Models.Reports;

public sealed record CsvResult(Stream Data, string FileName, string ContentType = "text/csv");