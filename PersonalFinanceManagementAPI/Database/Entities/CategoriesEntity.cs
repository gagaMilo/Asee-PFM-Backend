using PersonalFinanceManagementAPI.Models;

namespace PersonalFinanceManagementAPI.Database.Entities
{
    public class CategoriesEntity
    {

        public string Code { get; set; }
        public string ParentCode { get; set; }
        public string Name { get; set; }
        public List<SplitsEntity> Splits { get; set; }

    }
}
