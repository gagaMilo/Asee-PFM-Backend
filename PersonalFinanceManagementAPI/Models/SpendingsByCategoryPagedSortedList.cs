using System.Text.Json.Serialization;

namespace PersonalFinanceManagementAPI.Models
{
    public class SpendingsByCategoryPagedSortedList<T>
    {

        [JsonPropertyName("groups")]
        public List<T> Groups { get; set; }
    }
}
