using PersonalFinanceManagementAPI.Commands;
using PersonalFinanceManagementAPI.Database.Entities;
using PersonalFinanceManagementAPI.Models;

namespace PersonalFinanceManagementAPI.Database.Repositories
{
    public interface ITransactionsRepository
    {
        Task<TransactionsEntity> ImportTransactionById(string id);
        Task<TransactionsPagedSortedList<TransactionsEntity>> GetListTransactions(TransactionKind? transactionKind = null, DateTime? startDate = null, DateTime? endDate = null, int page = 1, int pageSize = 10, string? sortBy = null, SortOrder sortOrder = SortOrder.Asc);
        Task<TransactionsEntity> ImportTransactionRepository(TransactionsEntity newTranactionsEntity);
        Task<TransactionsEntity> UpdateTransaction(TransactionsEntity newTranactionsEntity);


        Task<TransactionsEntity> GetTransactionById(string transactionId);
        
        Task<SpendingsByCategoryPagedSortedList<SpendingInCategoryEntity>> GetSpendingsByCategory (string? catcode, DateTime? startDate, DateTime? endDate, Direction? direction);


        //split
       // Task<List<SplitsEntity>> CreateSplit(string transaction_id, List<SplitTransCommand> splits);
        Task DeleteSplitsForTransaction(TransactionsEntity transaction);
        Task<SplitsEntity> AddSplits(SplitsEntity splits);
        Task<bool> AutoCategorizeTransactions();
    }
}
