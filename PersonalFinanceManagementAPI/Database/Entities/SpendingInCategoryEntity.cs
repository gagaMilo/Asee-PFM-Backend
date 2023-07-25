using System.Text.Json.Serialization;

namespace PersonalFinanceManagementAPI.Database.Entities
{
    public class SpendingInCategoryEntity
    {
      
            public string? Catcode { get; set; }
            public double Amount { get; set; }
            public int Count { get; set; }

            public SpendingInCategoryEntity()
            {

            }

            public SpendingInCategoryEntity(string? catCode, double amount, int count)
            {
                Catcode = catCode;
                Amount = amount;
                Count = count;
            }
        }

    
}
