using CsvHelper.Configuration.Attributes;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using PersonalFinanceManagementAPI.Models;
using System.Collections.Generic;

namespace PersonalFinanceManagementAPI.Database.Entities
{
    public class TransactionsEntity // pravimo samu bazu, odnosno tabelu
    {
   

        public string Id { get; set; }
        public string BeneficiaryName { get; set; }
        public DateTime Date { get; set; }
        public int Direction { get; set; }
        public double Amount { get; set; }
        public string Description { get; set; }
        public string Currency { get; set; }
        public Mcc? Mcc { get; set; }
        public TransactionKind Kind { get; set; }

        public string? Catcode { get; set; }

       public List<SplitsEntity> Splits { get; set; } 

    }

      
}
