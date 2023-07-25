using AutoMapper;
using Newtonsoft.Json;
using PersonalFinanceManagementAPI.Commands;
using PersonalFinanceManagementAPI.Database.Entities;
using PersonalFinanceManagementAPI.Database.Repositories;
using PersonalFinanceManagementAPI.Models;

namespace PersonalFinanceManagementAPI.Services
{
    public class TransactionsService : ITransactionsService
    {
        private readonly ITransactionsRepository _repository;
        private readonly ICategoriesRepository _repository1;

        private readonly IMapper _mapper;

        public TransactionsService(ITransactionsRepository repository, ICategoriesRepository repository1, IMapper mapper) {
            _repository = repository;
            _repository1 = repository1;
            _mapper = mapper;
        }

        public async Task<Transactions> ImportCsvTransaction(CsvLineTransactionsCommand csvLine)
        {

            var checkIfProductExists = await CheckIfProductExistsAsync(csvLine.Id);
            if (!checkIfProductExists)
            {
                var newTransactionEntity = _mapper.Map<TransactionsEntity>(csvLine);
                await _repository.ImportTransactionRepository(newTransactionEntity);
                return _mapper.Map<Transactions>(newTransactionEntity);
            }
            return null;
        
        }

        private async Task<bool> CheckIfProductExistsAsync(string id)
        {
           
                var transaction = await _repository.ImportTransactionById(id);
                if (transaction == null)
                {
                    return false;

                }
                return true;
            

        }



        public async Task<TransactionsPagedSortedList<Transactions>> GetListTransactions(TransactionKind? transactionKind, DateTime? startDate, DateTime? endDate, int page, int pageSize, string? sortBy, SortOrder sortOrder)
        {
            var transactions = await _repository.GetListTransactions(transactionKind,startDate, endDate, page, pageSize, sortBy, sortOrder);
            
            return _mapper.Map<TransactionsPagedSortedList<Transactions>>(transactions);
        }

   

    

        public async Task<Transactions> CategorizeTransaction(string transactionId, CategorizeTransCommand categorizeTransCommand)
        {
            // Proverim da li postoji transakcija sa datim ID-jem u bazi
            var transaction = await _repository.GetTransactionById(transactionId);
            if (transaction == null)
            {
                return null;
            }

            // Proverim da li postoji kategorija sa datim kodom u bazi
            var category = await _repository1.GetCategoryByCode(categorizeTransCommand.catCode);
            if (category == null)
            {
                return null;
            }

            // kategorizacija transakcije
            transaction.Catcode = categorizeTransCommand.catCode;

            await _repository.UpdateTransaction(transaction);

            return _mapper.Map<Transactions>(transaction);



            
        }

        public async Task<SpendingsByCategoryPagedSortedList<SpendingInCategory>> GetSpendingAnalytics(string? catcode, DateTime? startDate, DateTime? endDate, Direction? direction)
        {

            var spendings = await _repository.GetSpendingsByCategory(catcode, startDate, endDate, direction);

       
            return _mapper.Map<SpendingsByCategoryPagedSortedList<SpendingInCategory>>(spendings);



        }


        public async Task<Transactions> SplitTransaction(string id, SplitTransCommand splits)
        {

            var transaction = await _repository.GetTransactionById(id);
            if (transaction == null)
            {
                return null;
            }

            //za svaki split prolazim
            foreach(var splitTransaction in splits.Splits)
            {
               var category = await _repository1.GetCategoryByCode(splitTransaction.Catcode);
            if(category == null)
                {
                    return null;
                }


            }

            //proverim amount
            double totalAmount = 0;
            for (int i = 0; i < splits.Splits.Count; i++) totalAmount += splits.Splits[i].Amount;
          


            await _repository.DeleteSplitsForTransaction(transaction);



            if (totalAmount != transaction.Amount)
            {
                throw new BadHttpRequestException("Error : Total amount of splits does not match the transaction amount!");
            }


            foreach ( var splitTransaction in splits.Splits)
            { 
                var category = await _repository1.GetCategoryByCode(splitTransaction.Catcode);
                var splitEntity = new SplitsEntity(id, splitTransaction.Catcode, splitTransaction.Amount, transaction, category);
                await _repository.AddSplits(splitEntity);

            }

            

           return _mapper.Map<Transactions>(transaction);
           
           

        }

        public async Task<bool> AutoCategorizeTransactions()
        {
            var rulesJson = File.ReadAllText("rules.json");

            var config = JsonConvert.DeserializeObject<Configuration>(rulesJson);

            //List<Models.Rule> rules = config.Rules;
            return await _repository.AutoCategorizeTransactions();

          
          
        }
    }
}

