using CsvHelper.Configuration;
using PersonalFinanceManagementAPI.Commands;

namespace PersonalFinanceManagementAPI.Mappings
{
    public class TransactionsMapper : ClassMap<CsvLineTransactionsCommand>
    {
        public TransactionsMapper()
        {
            Map(m => m.Id).Name("id");
            Map(m => m.BeneficiaryName).Name("beneficiary-name");
            Map(m => m.Date).Name("date");
            Map(m => m.Direction).Name("direction");
            Map(m => m.Amount).Name("amount");
            Map(m => m.Description).Name("description");
            Map(m => m.Currency).Name("currency");
            Map(m => m.Mcc).Name("mcc");
            Map(m => m.Kind).Name("kind");
          


        }

    }
}
