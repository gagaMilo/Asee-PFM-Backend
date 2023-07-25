using PersonalFinanceManagementAPI.Database.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PersonalFinanceManagementAPI.Models
{
    public class Splits
    {

       // public string TransactionId { get; set; }

        public string Catcode { get; set; }

        public double Amount { get; set; }

    }
}
