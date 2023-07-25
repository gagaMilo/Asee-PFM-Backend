using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PersonalFinanceManagementAPI.Database.Entities
{
    public class SplitsEntity
    {
        public SplitsEntity() { }

        public SplitsEntity(string transactionId, string catcode, double amount, TransactionsEntity transaction, CategoriesEntity category)
        { 
            TransactionId = transactionId;
            Catcode = catcode;
            Amount = amount;
            this.category = category;
            this.transaction = transaction;
        }

        [Required]
        public string TransactionId { get; set; }

        [Required]
        public string Catcode { get; set; }

        [Required]
        public double Amount { get; set; }

        public TransactionsEntity transaction { get; set; }

        public CategoriesEntity category { get; set; }

    }
}

