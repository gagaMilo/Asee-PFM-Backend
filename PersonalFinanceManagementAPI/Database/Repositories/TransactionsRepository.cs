using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using System.Text.Json;
using PersonalFinanceManagementAPI.Database.Entities;
using PersonalFinanceManagementAPI.Models;
using System.Data;

using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using SortOrder = PersonalFinanceManagementAPI.Models.SortOrder;
using LinqKit;

namespace PersonalFinanceManagementAPI.Database.Repositories
{
    public class TransactionsRepository : ITransactionsRepository
    {
        PFMDbContext _dbContext;
        public TransactionsRepository(PFMDbContext dbContext) {
            _dbContext = dbContext; 
        }
        public async Task<TransactionsEntity> ImportTransactionById(string id)
        {
            return await _dbContext.Transactions.FirstOrDefaultAsync(x => x.Id.Equals(id));
        }


        public async Task<TransactionsEntity> ImportTransactionRepository(TransactionsEntity newTranactionsEntity)
        {
            _dbContext.Transactions.Add(newTranactionsEntity);
            await _dbContext.SaveChangesAsync();
            return newTranactionsEntity;
        }

        public async Task<TransactionsPagedSortedList<TransactionsEntity>> GetListTransactions(TransactionKind? transactionKind = null, DateTime? startDate = null, DateTime? endDate = null, int page = 1, int pageSize = 10, string? sortBy = null, SortOrder sortOrder = SortOrder.Asc)
        {
            var query = _dbContext.Transactions.Include(x => x.Splits).AsQueryable();
            var totalCount = query.Count();
            var totalPages = (int)Math.Ceiling((double)totalCount /pageSize);

            
            //sortiranje
            if (!String.IsNullOrEmpty(sortBy) )
            {
                switch (sortBy)
                {
                    case "id":
                            query = sortOrder == SortOrder.Asc ? query.OrderBy(x => x.Id) : query.OrderByDescending(x => x.Id);
                            break;
                    case "beneficiary-name":
                        query = sortOrder == SortOrder.Asc ? query.OrderBy(x => x.BeneficiaryName) : query.OrderByDescending(x => x.BeneficiaryName);
                        break;
                    case "date":
                        query = sortOrder == SortOrder.Asc ? query.OrderBy(x => x.Date) : query.OrderByDescending(x => x.Date);
                        break;
                    case "direction":
                        query = sortOrder == SortOrder.Asc ? query.OrderBy(x => x.Direction) : query.OrderByDescending(x => x.Direction);
                        break;
                    case "amount":
                        query = sortOrder == SortOrder.Asc ? query.OrderBy(x => x.Amount) : query.OrderByDescending(x => x.Amount);
                        break;
                    case "description":
                        query = sortOrder == SortOrder.Asc ? query.OrderBy(x => x.Description) : query.OrderByDescending(x => x.Description);
                        break;
                    case "currency":
                        query = sortOrder == SortOrder.Asc ? query.OrderBy(x => x.Currency) : query.OrderByDescending(x => x.Currency);
                        break;
                    case "mcc":
                        query = sortOrder == SortOrder.Asc ? query.OrderBy(x => x.Mcc) : query.OrderByDescending(x => x.Mcc);
                        break;
                    case "kind":
                        query = sortOrder == SortOrder.Asc ? query.OrderBy(x => x.Kind) : query.OrderByDescending(x => x.Kind);
                        break;


                }

            }
            else
            {
                query = query.OrderBy(x => x.Date);
            }


           
            if (transactionKind.HasValue)
            {
                query = query.Where(x => x.Kind == transactionKind);
            }

            if (startDate.HasValue)
            {
                query = query.Where(x => x.Date >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                query = query.Where(x => x.Date <= endDate.Value);  
            }


            //optimizovanje poziva
            query = query.Skip((page-1) * pageSize).Take(pageSize);

            var transactions = await query.ToListAsync();

            return new TransactionsPagedSortedList<TransactionsEntity>
            {
                TotalCount = totalCount,
                PageSize = pageSize,
                Page = page,
                TotalPages = totalPages,
                SortOrder = sortOrder,
                SortBy = sortBy,
                Items = transactions


            };
        }



        public async Task<TransactionsEntity> GetTransactionById(string transactionId)
        {
            return await _dbContext.Transactions.FirstOrDefaultAsync(t => t.Id == transactionId);
        }

        public async void SaveChanges()
        {
            await _dbContext.SaveChangesAsync();
        }

        public async Task<TransactionsEntity> UpdateTransaction(TransactionsEntity updateTranactionsEntity)
        {
            _dbContext.Transactions.Update(updateTranactionsEntity);
            await _dbContext.SaveChangesAsync();
            return updateTranactionsEntity;
        }

        public async Task<SpendingsByCategoryPagedSortedList<SpendingInCategoryEntity>> GetSpendingsByCategory(string? catcode, DateTime? startDate, DateTime? endDate, Direction? direction)
        {
            
            var query = _dbContext.Transactions.AsQueryable();

            if (!string.IsNullOrEmpty(catcode))
            {
                
                query = query.Where(x => x.Catcode == catcode);
            }
            else
            {
                query = query.Where(x => string.IsNullOrEmpty(x.Catcode));
            }

            if (startDate.HasValue)
            {
                query = query.Where(x => x.Date >= startDate.Value);
            }
            else
            {
                if (await query.AnyAsync())
                {
                    DateTime minDate = await query.MinAsync(x => x.Date);
                    startDate = minDate;
                    query = query.Where(x => x.Date >= startDate.Value);
                }
            }


            if (endDate.HasValue)
            {
                query = query.Where(x => x.Date <= endDate.Value);
            }
            else
            {

                if (await query.AnyAsync())
                {
                    DateTime maxDate = await query.MaxAsync(x => x.Date);
                    endDate = maxDate;
                    query = query.Where(x => x.Date >= endDate.Value);
                }
            }
            int dbDirection;
            if (direction.HasValue)
            {
               
                dbDirection = direction.Value == Direction.d ? 0 : 1;
                query = query.Where(x => x.Direction == dbDirection);
            }
            else
            {
                // Ako vrednost smera nije postavljena, pretpostavljam vrednost 0 (debit)
                dbDirection = 0;
                query = query.Where(x => x.Direction == dbDirection);
            }

            var result = new SpendingsByCategoryPagedSortedList<SpendingInCategoryEntity>
            {
               
            };
            if (catcode != null)
            {
                
                var All = new List<string>();

                All.Add(catcode);

                
                

                var query1 = _dbContext.Categories.AsQueryable();
                var categories = await query1.ToListAsync();


                var universal = catcode;
                var fleg = 0;
                
                //sve podkategorije - ako postoje
                universal = catcode;
                fleg = 0;

                while (true)
                {
                    if (fleg == 1)
                    {
                        break;
                    }
                    var fleg1 = 0;
                    foreach (var category in categories)
                    {
                        if (category.ParentCode == universal)
                        {

                            var Code = category.Code;
                            fleg1 = 1;
                            if (Code != null && !Code.Equals(""))
                            {
                                All.Add(Code);
                                universal = Code;
                                
                                break;
                            }
                            else
                            {
                                fleg = 1;
                                break;
                            }
                        }
                        


                    }

                   if(fleg1 == 0)
                    {
                        break; // nema podkategoriju
                    }

                }

                //sad imam listu stringova od svih kategorija i podkategorija 
                // Kreiram praznu listu za rezultate
                var newListCatcode = new List<SpendingInCategoryEntity>();
                query = _dbContext.Transactions.AsQueryable();
                var spendings1 = await query.ToListAsync();

                foreach (var catcodeValue in All)
                {

                    if (catcodeValue != null )
                    {
                        int count_i = 0;
                        double amount_i = 0;
                        int flag = 0;

                        // Provera da li smo vec obradili ovu kategoriju
                        var existingCategory = newListCatcode.FirstOrDefault(c => c.Catcode == catcodeValue);

                        if (existingCategory == null)
                        {

                            foreach (var transaction_j in spendings1)
                            {
                                if (catcodeValue.Equals(transaction_j.Catcode))
                                {

                                    flag = 1;
                                    amount_i += transaction_j.Amount;
                                    count_i++;

                                }
                            }

                            if (flag == 1)
                            {
                                SpendingInCategoryEntity spendinInCategory_i = new SpendingInCategoryEntity(catcodeValue, amount_i, count_i);
                                newListCatcode.Add(spendinInCategory_i);
                            }
                        }


                    }
                }
                result.Groups = newListCatcode;



            }

            if (string.IsNullOrEmpty(catcode))
            {
                
                var newList = new List<SpendingInCategoryEntity>();
                query = _dbContext.Transactions.AsQueryable();
                var spendings = await query.ToListAsync();

                foreach (var transaction in spendings)
                {
                    if (transaction != null)
                    {
                        string catcode_i = transaction.Catcode;
                        string subCategory_i = new String(catcode_i);

                        if (catcode_i != null)
                        {
                            int count_i = 0;
                            double amount_i = 0;

                            // ako je podkategorija
                            if (subCategory_i.All(c => char.IsDigit(c)))
                            {
                                var query1 = _dbContext.Categories.AsQueryable();
                                var categories = await query1.ToListAsync();
                                catcode_i = CheckCategoryForSubcategory(subCategory_i, categories);


                            }
                            // optimizovati 
                            // ako je kategorija
                                if (catcode_i != null && catcode_i.All(c => char.IsLetter(c)))
                                { 
  
                                var existingCategory = newList.FirstOrDefault(c => c.Catcode.Equals(catcode_i));

                                if (existingCategory == null) // ne postoji
                                {
                                    
                                    foreach (var transaction_j in spendings)
                                    {
                                        string catcode_j = transaction_j.Catcode;
                                        string subCategory_j = new String(catcode_j);


                                        if (catcode_j != null && catcode_i.Equals(catcode_j))
                                        {

                                                amount_i += transaction_j.Amount;
                                                count_i++;
                                             
                                           
                                        }
                                        else
                                        {
                                            if (subCategory_j != null && subCategory_j.All(c => char.IsDigit(c))) 
                                            {
                                                var query1 = _dbContext.Categories.AsQueryable();
                                                var categories = await query1.ToListAsync();
                                                catcode_j = CheckCategoryForSubcategory(subCategory_j, categories);
                                                if (catcode_i.Equals(catcode_j))
                                                {
                                                    amount_i += transaction_j.Amount;
                                                    count_i++;

                                                }

                                            }

                                        }


                                        
                                    }


                                    SpendingInCategoryEntity spendinInCategory_i = new SpendingInCategoryEntity(catcode_i, amount_i, count_i);
                                    newList.Add(spendinInCategory_i);
                                }
                                else
                                {

                                    continue;

                                   
                                }
                            }
                        }
                    }
                }
                result.Groups = newList;
            }


            return result;
        }

        private  string? CheckCategoryForSubcategory(string catcode_i, List<CategoriesEntity> categories)
        {
            
            foreach (var category in categories)
            {
                if (category.Code == catcode_i)
                {

                    var parentCode = category.ParentCode;

                    if (parentCode != null && !parentCode.Equals(""))
                    {
                        return parentCode;
                    }
                    else
                    {
                        return null;
                    }
                }

            }
            return null;
            
        }
        
        public async Task DeleteSplitsForTransaction(TransactionsEntity transaction)
        {

            
            var existingSplits = _dbContext.SplitsTransactions.Where(s => s.TransactionId == transaction.Id);
            _dbContext.SplitsTransactions.RemoveRange(existingSplits);
            await _dbContext.SaveChangesAsync();


        }

        public async Task<SplitsEntity> AddSplits(SplitsEntity splits)
        {
            _dbContext.SplitsTransactions.AddRange(splits);
            await _dbContext.SaveChangesAsync();
            return splits;
        }

        public Mcc ConvertToMccEnum(string mccValue)
        {
         
            switch (mccValue)
            {
                case "Mcc = '8299'": return Mcc.MCC237;
                case "Mcc = '5499'": return Mcc.MCC55;
                case "Mcc = '7523'": return Mcc.MCC187;
                case "Mcc = '5733'": return Mcc.MCC87;

                // ...
                default: return Mcc.MCC1; 
            }
        }

        public async Task<bool> AutoCategorizeTransactions()
        {


            string rulesJson = File.ReadAllText("rules.json");

            Configuration config = JsonSerializer.Deserialize<Configuration>(rulesJson);



            foreach (var rule in config.Rules)
            {
                string predicate = rule.Predicate;
                List<TransactionsEntity> transactions_category = new List<TransactionsEntity>();


                
                   // Pravim SQL upit za filtriranje 
                   transactions_category = await _dbContext.Transactions
                             .FromSql($"SELECT * FROM public.transactions WHERE {predicate}")
                             .ToListAsync();
                   
                /*
                
                if (rule.Predicate.Contains("Mcc"))
                {
                   
                   

                    var mccValue = rule.Predicate.Replace("'Mcc'", "Mcc"); // Uklonim apostrofe oko Mcc
                    var mccEnumValue = ConvertToMccEnum(mccValue);
                    transactions_category = await _dbContext.Transactions
                        .Where(t => t.Mcc == mccEnumValue)
                        .ToListAsync();


                }

                
               if (rule.Predicate.Contains("BeneficiaryName"))
                {
                    /*
                    var beneficiaryNameValue = rule.Predicate.Replace("'BeneficiaryName'", "BeneficiaryName"); // Uklonim apostrofe oko BeneficiaryName
                    
                    transactions_category = await _dbContext.Transactions
                        .Where(t => EF.Functions.Like(t.BeneficiaryName, beneficiaryNameValue))
                        .ToListAsync();*/
                    /*
                    transactions_category = await _dbContext.Transactions
                              .FromSqlInterpolated($"SELECT * FROM public.transactions WHERE {rule.Predicate}")
                              .ToListAsync();
                    
                    var beneficiaryNameValue = rule.Predicate.Replace("'BeneficiaryName'", "BeneficiaryName");
                    List<string> names = convertToNames(beneficiaryNameValue);
                    transactions_category = await _dbContext.Transactions
                        .Where(t => t.BeneficiaryName == names[0] || t.BeneficiaryName == names[1])
                        .ToListAsync();

                }
            */
                if (transactions_category != null)
                {
                    foreach (var transaction in transactions_category)
                    {
                        transaction.Catcode = rule.Catcode;
                        _dbContext.Update(transaction);
                    }
                }
            }

            await _dbContext.SaveChangesAsync();
      

            return true;
        }

        private List<string> convertToNames(string beneficiaryNameValue)
        {
            List<string> searchTerms = new List<string>();

            string[] parts = beneficiaryNameValue.Split(new string[] { " OR " }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string part in parts)
            {
                string searchTerm = part.Replace("BeneficiaryName LIKE '%", "").Replace("%'", "");
                searchTerms.Add(searchTerm);
            }

            return searchTerms;
        }
    }
   

}


