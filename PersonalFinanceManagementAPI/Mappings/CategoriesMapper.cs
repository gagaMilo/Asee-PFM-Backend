using CsvHelper.Configuration;
using PersonalFinanceManagementAPI.Commands;

namespace PersonalFinanceManagementAPI.Mappings
{
    public class CategoriesMapper : ClassMap<CsvLineCategoriesCommand>
    {
        public CategoriesMapper()
        {
            Map(m => m.Code).Name("code");
            
            Map(m => m.ParentCode).Name("parent-code");

            Map(m => m.Name).Name("name");
        }
    }
}
