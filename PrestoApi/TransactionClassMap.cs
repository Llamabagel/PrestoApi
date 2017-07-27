using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using PrestoApi.Models.Presto;

namespace PrestoApi
{
    public sealed class TransactionClassMap : CsvClassMap<TransactionResponse>
    {
        public TransactionClassMap()
        {
            Map(m => m.Date).Name("Date").TypeConverter<DateConverter>();
            Map(m => m.Agency).Name("Transit Agency");
            Map(m => m.Location).Name("Location");
            Map(m => m.Type).Name("Type ");
            Map(m => m.ServiceClass).Name("Service Class");
            Map(m => m.Discount).Name("Discount");
            Map(m => m.Amount).Name("Amount");
            Map(m => m.Balance).Name("Balance");
        }
    }
}