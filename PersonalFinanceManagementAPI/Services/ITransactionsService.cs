using Microsoft.AspNetCore.Mvc;
using PersonalFinanceManagementAPI.Commands;
using PersonalFinanceManagementAPI.Controllers;
using PersonalFinanceManagementAPI.Models;

namespace PersonalFinanceManagementAPI.Services
{
    public interface ITransactionsService
    {
  
        Task<bool> AutoCategorizeTransactions();
        Task<Transactions> CategorizeTransaction(string transactionId, CategorizeTransCommand categorizeTransCommand);
        Task<TransactionsPagedSortedList<Transactions>> GetListTransactions(TransactionKind? transactionKind, DateTime? startDate, DateTime? endDate, int page, int pageSize, string? sortBy, SortOrder sortOrder);
        Task<SpendingsByCategoryPagedSortedList<SpendingInCategory>> GetSpendingAnalytics(string catcode, DateTime? startDate, DateTime? endDate, Direction? direction);
        Task<Transactions> ImportCsvTransaction(CsvLineTransactionsCommand csvLine);
        Task<Transactions> SplitTransaction(string id, SplitTransCommand splits);

       


    }
}
