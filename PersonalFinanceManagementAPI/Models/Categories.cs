using Newtonsoft.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PersonalFinanceManagementAPI.Models
{
    public class Categories
    {
        [JsonPropertyName("code")]
        public string Code { get; set; }
        [JsonPropertyName("parent-code")]
        public string ParentCode { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }



    }
}
