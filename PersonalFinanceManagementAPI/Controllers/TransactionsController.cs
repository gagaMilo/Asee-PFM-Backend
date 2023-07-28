using CsvHelper;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PersonalFinanceManagementAPI.Commands;
using PersonalFinanceManagementAPI.Database;
using PersonalFinanceManagementAPI.Database.Entities;
using PersonalFinanceManagementAPI.Mappings;
using PersonalFinanceManagementAPI.Models;
using PersonalFinanceManagementAPI.Services;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using Swashbuckle.AspNetCore.Annotations;
using System.Runtime.Intrinsics.Arm;

namespace PersonalFinanceManagementAPI.Controllers
{
    

    [EnableCors("MyCORSPolicy")]
    [ApiController]
    [Route("transactions")] //Ruta kontrolera
    [SwaggerTag("Transactions", "Working with finance transactions")]
    public class TransactionsController : ControllerBase
    {
        private readonly ITransactionsService _transactionsService;
        private readonly ILogger<TransactionsController> _logger;
       

        public TransactionsController(ILogger<TransactionsController> logger, ITransactionsService transactionsService)
        {
            _logger = logger;
            _transactionsService = transactionsService;
           
    }
        // GET : /transactions   
        [HttpGet]
        public async Task<IActionResult> GetListTransactions([FromQuery(Name = "transaction-kind")] string? transactionKind = null, [FromQuery(Name = "start-date")] DateTime? startDate = null, [FromQuery(Name = "end-date")] DateTime? endDate = null, [FromQuery(Name = "page")] int page = 1, [FromQuery(Name = "page-size")] int pageSize = 10, [FromQuery(Name = "sort-by")] string? sortBy = null, [FromQuery(Name ="sort-order")] SortOrder sortOrder = SortOrder.Asc)
        {
            TransactionKind? transactionKind1 = traconvertStringTransactionKindToEnum(transactionKind);
            var transactions = await _transactionsService.GetListTransactions(transactionKind1, startDate, endDate, page, pageSize, sortBy, sortOrder);
            return Ok(transactions);
        }

        private TransactionKind? traconvertStringTransactionKindToEnum(string? transactionKind)
        {
            if (transactionKind != null) {
                switch (transactionKind)
                {
                    case "dep": return TransactionKind.dep;
                    case "wdw": return TransactionKind.wdw;
                    case "pmt": return TransactionKind.pmt;
                    case "fee": return TransactionKind.fee;
                    case "inc": return TransactionKind.inc;
                    case "rev": return TransactionKind.rev;
                    case "adj": return TransactionKind.adj;
                    case "lnd": return TransactionKind.lnd;
                    case "lnr": return TransactionKind.lnd;
                    case "fcx": return TransactionKind.lnd;
                    case "aop": return TransactionKind.lnd;
                    case "acl": return TransactionKind.lnd;
                    case "spl": return TransactionKind.lnd;
                    case "sal": return TransactionKind.lnd;
                    default: return null;

                }
            }
            else
            {
                return null;
            }
        }

        // POST : /transactions/import 
        [HttpPost("import")]
        [Description("Import transactions")]

        public async Task<IActionResult> ImportFromCsvTransactions(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Please upload a valid CSV file.");
            try
            {

                var transactions = GetCsvTransactions(file);
                 

               foreach (var row in transactions)
               {
                    await _transactionsService.ImportCsvTransaction(row);

               }

               return Ok();

                
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while importing transactions.");
      
                return StatusCode(500);
            }
        }

        private List<CsvLineTransactionsCommand> GetCsvTransactions(IFormFile file)
        {
            using (var fs = new StreamReader(file.OpenReadStream()))

            using (var csvFile = new CsvReader(fs, CultureInfo.InvariantCulture))

            { 
                csvFile.Context.RegisterClassMap<TransactionsMapper>();

                if (csvFile != null)
                {
                    return csvFile.GetRecords<CsvLineTransactionsCommand>().ToList();
                }
                return null;
            }
        }


        // POST : /transactions/{id}/split   
        [HttpPost("{id}/split")]
        [Description("Split transacation by id")]
        public async Task<IActionResult> SplitTransaction([FromRoute]string id, [FromBody] SplitTransCommand splits)
        {
            try
            {
                var result = await _transactionsService.SplitTransaction(id, splits);

                return Ok(result);
            }catch(Exception e)
            {
                return BadRequest(e.Message);
            }
           

        }

        // POST : /transaction/{id}/categorize
        [HttpPost("{id}/categorize")]
        [Description("Categorize a transacation by id")]

        public async Task<IActionResult> CategorizeTransaction(string id, [FromBody] CategorizeTransCommand catCode)
        {
            var result = await _transactionsService.CategorizeTransaction(id, catCode);

            
            if (result != null)
            {
                return Ok(result);
            }

            return NotFound("Transaction or category not found.");
            

        }


        // POST : /transactions/auto-categorize
        [HttpPost("auto-categorize")]
        [Description("Auto categorize transactions")]

        public async Task<IActionResult> AutoCategorizeTransaction()
        {
            
            var result = await _transactionsService.AutoCategorizeTransactions();

            if (result)
            {
                return Ok("Successfully auto-categorized!");
            }
            else
            {
               return BadRequest("Not successfully auto-categorized!");
            }

        }


    


    }
}