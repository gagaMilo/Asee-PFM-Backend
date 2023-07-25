using System.Text.Json.Serialization;

namespace PersonalFinanceManagementAPI.Models
{
    public class ValidationProblemPagedSortedList<T>
    {
        [JsonPropertyName("errors")]
        public List<T> Errors { get; set; }

    }
}
