using System.Text.Json.Serialization;

namespace PersonalFinanceManagementAPI.Models
{
    public class SpendingInCategory


    {
  
        [JsonPropertyName("catcode")]
        public string? Catcode { get; set; }
        [JsonPropertyName("amount")]
        public double Amount { get; set; }
        [JsonPropertyName("count")]
        public int Count { get; set; }


    
    }
}
