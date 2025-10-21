using CsvHelper;
using CsvHelper.Configuration;
using DVSRegister.BusinessLogic.Models;
using DVSRegister.Data;
using DVSRegister.Data.Entities;
using Microsoft.Extensions.Logging;
using System.Globalization;

namespace DVSRegister.BusinessLogic.Services
{
    public sealed class PfrCsvMap : ClassMap<Service>
    {
        public PfrCsvMap()
        {
            //Maps Service table columns to Register fields
            Map(s => s.Provider.RegisteredName).Name("Provider");
            Map(s => s.CabUser.Cab.CabName).Name("CAB");
            Map(s => s.ServiceName).Name("Service name");
            Map(m => m.ServiceSupSchemeMapping).Name("Schemes")
            .Convert(row => string.Join(", " , row.Value.ServiceSupSchemeMapping
            ?.Where(ssm => ssm.SupplementaryScheme != null)
            ?.Select(ssm => ssm.SupplementaryScheme.SchemeName)
            ?? Enumerable.Empty<string>()));
            Map(s => s.PublishedTime).Name("Published on").TypeConverterOption.Format("dd/MM/yyyy");
            Map(s => s.ConformityIssueDate).Name("Certificate Issue Date").TypeConverterOption.Format("dd/MM/yyyy"); ;
            Map(s => s.ConformityExpiryDate).Name("Certificate Expiry Date").TypeConverterOption.Format("dd/MM/yyyy");
        }
    }
    public class CsvDownloadService : ICsvDownloadService
    {
        private readonly IRegisterRepository registerRepository;
     
        private readonly ILogger<CsvDownloadService> logger;

        public CsvDownloadService(IRegisterRepository registerRepository, ILogger<CsvDownloadService> logger)
        {
            this.registerRepository = registerRepository;
            this.logger = logger;
        }

        public async Task<CsvDownload> DownloadAsync()
        {
            try
            {
                var services = await registerRepository.GetPublishedServices();
                
                foreach (var service in services.Take(1))  // Just look at first record
                {
                    logger.LogInformation("Service {ServiceName} has {SchemeCount} schemes", 
                        service.ServiceName,
                        service.ServiceSupSchemeMapping?.Count ?? 0);
    
                    if (service.ServiceSupSchemeMapping != null)
                    {
                        foreach (var scheme in service.ServiceSupSchemeMapping)
                        {
                            logger.LogInformation("Scheme: {SchemeName}", 
                                scheme.SupplementaryScheme?.SchemeName ?? "null");
                        }
                    }
                }

                if (!services.Any())
                {
                    throw new InvalidOperationException("No service data available for export");
                }

                // Generate the CSV in memory
                using var memoryStream = new MemoryStream();
                using var writer = new StreamWriter(memoryStream);
                using var csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    Delimiter = ",",
                    HasHeaderRecord = true,
                    Quote = '"',
                    //Quote everything in the first row/header, and any value that has a comma
                    ShouldQuote = args => args.Row.Index == 0 || args.Field.Contains(",")
                });

                //Maps Service table columns to Register fields
                csv.Context.RegisterClassMap<PfrCsvMap>();

                await csv.WriteRecordsAsync(services);
                await writer.FlushAsync();

                logger.LogInformation("Successfully generated Register export with {Count} services", 
                    services.Count);

                // Return everything needed for the file download
                return new CsvDownload
                {
                    FileContent = memoryStream.ToArray(),
                    ContentType = "text/csv",
                    FileName = $"dvs-register_{DateTime.Now:ddMMyyyy}.csv"
                };
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Could not generate Register export");
                throw;
            }
        }
    }
}