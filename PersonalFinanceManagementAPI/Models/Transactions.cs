using System.Text.Json.Serialization;

namespace PersonalFinanceManagementAPI.Models
{
    public class Transactions
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
       [JsonPropertyName("beneficiary-name")]
        public string BeneficiaryName { get; set; }
       [JsonPropertyName("date")]
        public DateTime Date { get; set; }
        [JsonPropertyName("direction")]
        public Direction Direction { get; set; }
       [JsonPropertyName("amount")]
        public double Amount { get; set; }
       [JsonPropertyName("description")]
        public string Description { get; set; }
       [JsonPropertyName("currency")]
        public string Currency { get; set; }
       [JsonPropertyName("mcc")]
        public Mcc? Mcc { get; set; }
        [JsonPropertyName("kind")]
        public TransactionKind Kind { get; set; }
        [JsonPropertyName("catcode")]
        public string Catcode { get; set; }
      
       public List<Splits> Splits { get; set; }




    }
}
