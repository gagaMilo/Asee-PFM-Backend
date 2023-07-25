using CsvHelper.Configuration.Attributes;
using System.ComponentModel.DataAnnotations;

namespace PersonalFinanceManagementAPI.Commands
{
    public class CsvLineCategoriesCommand
    {
        [Name("code")]
        [Required]
        public string Code { get; set; }

        [Name("name")]
        [Required]
        public string Name { get; set; }

        [Name("parent-code")]
        public string ParentCode { get; set; }
       
    }
}
