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
using System.Diagnostics.Metrics;

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
                        {
                            query = sortOrder == SortOrder.Asc ? query.OrderBy(x => x.Id) : query.OrderByDescending(x => x.Id);
                            break;
                        }
                    case "beneficiary-name":
                        {
                            query = sortOrder == SortOrder.Asc ? query.OrderBy(x => x.BeneficiaryName) : query.OrderByDescending(x => x.BeneficiaryName);
                            break;
                        }
                    case "date":
                        {
                            query = sortOrder == SortOrder.Asc ? query.OrderBy(x => x.Date) : query.OrderByDescending(x => x.Date);
                            break;
                        }
                    case "direction":
                        {
                            query = sortOrder == SortOrder.Asc ? query.OrderBy(x => x.Direction) : query.OrderByDescending(x => x.Direction);
                            break;
                        }
                    case "amount":
                        {
                            query = sortOrder == SortOrder.Asc ? query.OrderBy(x => x.Amount) : query.OrderByDescending(x => x.Amount);
                            break;
                        }
                    case "description":
                        {
                            query = sortOrder == SortOrder.Asc ? query.OrderBy(x => x.Description) : query.OrderByDescending(x => x.Description);
                            break;
                        }
                    case "currency":
                        {
                            query = sortOrder == SortOrder.Asc ? query.OrderBy(x => x.Currency) : query.OrderByDescending(x => x.Currency);
                            break;
                        }
                    case "mcc":
                        {
                            query = sortOrder == SortOrder.Asc ? query.OrderBy(x => x.Mcc) : query.OrderByDescending(x => x.Mcc);
                            break;
                        }
                    case "kind":
                        {
                            query = sortOrder == SortOrder.Asc ? query.OrderBy(x => x.Kind) : query.OrderByDescending(x => x.Kind);
                            break;
                        }

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

                if (catcode.All(c => char.IsDigit(c)))
                {
                    var newListCatcode_sub = new List<SpendingInCategoryEntity>();
                    query = _dbContext.Transactions.AsQueryable();
                    var spendings_sub = await query.ToListAsync();
                    int count_sub = 0;
                    double amount_sub = 0;

                    foreach (var row in spendings_sub)
                    {
                        if (row.Catcode != null && row.Catcode.Equals(catcode))
                        {
                            amount_sub += row.Amount;
                            count_sub++;
                        }
                    }
                    SpendingInCategoryEntity spendinInCategory_sub = new SpendingInCategoryEntity(catcode, amount_sub, count_sub);
                    newListCatcode_sub.Add(spendinInCategory_sub);
                    result.Groups = newListCatcode_sub;

                }

                if (catcode.All(c => char.IsLetter(c)))
                {
                    SpendingInCategoryEntity spendinInCategory_sub;
                    var newListCatcode_category_and_sub = new List<SpendingInCategoryEntity>();
                    // da li postoji catcode kategorisan kao kategorija ili kao podkategorija
                    query = _dbContext.Transactions.AsQueryable();
                    var spendings = await query.ToListAsync();
                    int count_category = 0;
                    double amount_category = 0;

                    //provera da li postoji kategorisan kao kategorija prvo
                    foreach (var row in spendings)
                    {
                        if(row.Catcode != null && row.Catcode.Equals(catcode))
                        {
                            amount_category += row.Amount;
                            count_category++;
                        }
                    }
                    if (amount_category != 0)
                    {
                       spendinInCategory_sub = new SpendingInCategoryEntity(catcode, amount_category, count_category);
                        newListCatcode_category_and_sub.Add(spendinInCategory_sub);
                    }
                    //dobijem sve moguce podkategorije za tu kategoriju
                    var query1 = _dbContext.Categories.AsQueryable();
                    var categories = await query1.ToListAsync();
                    List<string> allsubcategory_list = new List<string>();
                    foreach (var row in categories)
                    {
                        if (row.ParentCode != null && row.ParentCode.Equals(catcode))
                        {
                            allsubcategory_list.Add(row.Code);
                        }
                    }

                   foreach(var row in allsubcategory_list)
                    {
                        query = _dbContext.Transactions.AsQueryable();
                        var spendings_sub = await query.ToListAsync();
                        int count_sub = 0;
                        double amount_sub = 0;

                        foreach (var row1 in spendings_sub)
                        {
                            if (row1.Catcode != null && row1.Catcode.Equals(row))
                            {
                                amount_sub += row1.Amount;
                                count_sub++;
                            }
                        }
                        if (amount_sub != 0)
                        {
                            spendinInCategory_sub = new SpendingInCategoryEntity(catcode, amount_sub, count_sub);
                            newListCatcode_category_and_sub.Add(spendinInCategory_sub);
                        }

                    }

                    result.Groups = newListCatcode_category_and_sub;

                   

                }

                }

            if (string.IsNullOrEmpty(catcode))
            {
                
                var newList = new List<SpendingInCategoryEntity>();
                query = _dbContext.Transactions.AsQueryable();
                var spendings = await query.ToListAsync();

                foreach (var transaction in spendings)
                {
                    if (transaction != null && transaction.Catcode != null)
                    {
                        string catcode_i = transaction.Catcode;
                        string subCategory_i = new String(catcode_i);

                        int count_i = 0;
                         double amount_i = 0;

                            // ako je podkategorija
                            if (subCategory_i.All(c => char.IsDigit(c)))
                            {
                                var query1 = _dbContext.Categories.AsQueryable();
                                var categories = await query1.ToListAsync();
                                catcode_i = CheckCategoryForSubcategory(subCategory_i, categories);


                            }
                            // optimizovati - ako bude vremena
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

 

        public async Task<bool> AutoCategorizeTransactions()
        {


            string rulesJson = File.ReadAllText("rules.json");

            Configuration config = JsonSerializer.Deserialize<Configuration>(rulesJson);



            foreach (var rule in config.Rules)
            {
                string predicate = $"WHERE {rule.Predicate}";
                List<TransactionsEntity> transactions_category;
             
               // Console.WriteLine($"SQL upit: SELECT * FROM transactions {predicate}");
                transactions_category =  await _dbContext.Transactions.FromSqlRaw($"SELECT * FROM transactions {predicate}").ToListAsync();
                
           
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

    

      

    }

    }
