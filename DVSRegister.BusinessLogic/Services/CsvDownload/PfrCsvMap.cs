using CsvHelper.Configuration;
using DVSRegister.BusinessLogic.Models.CAB;

namespace DVSRegister.BusinessLogic.Services
{
    public sealed class PfrCsvMap : ClassMap<ServiceDto>
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
  
}