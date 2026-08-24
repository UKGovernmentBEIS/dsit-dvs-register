using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using DVSRegister.BusinessLogic.Models.Reports;
using DVSRegister.CommonUtility;

namespace DVSRegister.BusinessLogic.Services.CsvDownload;

public static class CsvStreamHelper
{
    public static async Task<Result<CsvResult>> ToCsvAsync<T>(
        IEnumerable<T> rows,
        Action<CsvConfiguration>? configureCsv,
        Action<CsvContext> configureMap,
        string fileName,
        CancellationToken ct)
    {
        try
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture);
            configureCsv?.Invoke(config);

            var stream = new MemoryStream();
            await using var writer = new StreamWriter(stream, leaveOpen: true);
            await using var csv = new CsvWriter(writer, config, leaveOpen: true);
            configureMap(csv.Context);

            await csv.WriteRecordsAsync(rows, ct);
            await writer.FlushAsync(ct);
            stream.Position = 0;

            return Result<CsvResult>.Ok(new CsvResult(stream, fileName));
        }
        catch (Exception ex)
        {
            return Result<CsvResult>.Fail(Error.Unexpected($"CSV generation failed: {ex.Message}"));
        }
    }

    public static async Task<Stream> WriteAsync<T>(
        IEnumerable<T> rows,
        ClassMap<T> map,
        CancellationToken ct)
    {
        var config = new CsvConfiguration(CultureInfo.InvariantCulture);

        var stream = new MemoryStream();
        await using var writer = new StreamWriter(stream, leaveOpen: true);
        await using var csv = new CsvWriter(writer, config, leaveOpen: true);

        csv.Context.RegisterClassMap(map);
        await csv.WriteRecordsAsync(rows, ct);
        await writer.FlushAsync(ct);
        stream.Position = 0;

        return stream;
    }
}