using CsvHelper.Configuration.Attributes;
using PersonalFinanceManagementAPI.Models;
using System.ComponentModel.DataAnnotations;

namespace PersonalFinanceManagementAPI.Commands
{
    public class CsvLineTransactionsCommand
    {
        [Name("id")]
        [Required]
        public string Id { get; set; }
        [Name("beneficiary-name")]
        public string BeneficiaryName { get; set; }
        [Name("date")]
        [Required]
        public DateTime Date { get; set; }
        [Name("direction")]
        [Required]
        public Direction Direction { get; set; }
        [Name("amount")]
        [Required]
        public double Amount { get; set; }
        [Name("description")]
        public string Description { get; set; }
        [Name("currency")]
        [Required]
        public string Currency { get; set; }
        [Name("mcc")]
        public Mcc? Mcc { get; set; }
        [Name("kind")]
        [Required]
        public TransactionKind Kind { get; set; }
       

    }
}
