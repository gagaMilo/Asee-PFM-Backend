using System.Text.Json.Serialization;

namespace PersonalFinanceManagementAPI.Models
{
    public class ValidationErrors
    {
        [JsonPropertyName("tag")]
        public string Tag { get; set; }
        [JsonPropertyName("error")]
        public Error Error { get; set; }
        [JsonPropertyName("message")]
        public string Message { get; set; }


    }
}
