using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using PersonalFinanceManagementAPI.Models;
using PersonalFinanceManagementAPI.Services;
using System.ComponentModel;

namespace PersonalFinanceManagementAPI.Controllers
{
    [EnableCors("MyCORSPolicy")]
    [ApiController]
    [Route("")] //Ruta kontrolera
    public class AnalyticsController : ControllerBase
    {

        private readonly ILogger<AnalyticsController> _logger;
        private readonly ITransactionsService _transactionsService;

       
        public AnalyticsController(ILogger<AnalyticsController> logger, ITransactionsService transactionsService)
        {
            _logger = logger;
            _transactionsService = transactionsService;

        }

        [HttpGet("spending-analytics")]
        [Description("Retrieve spending analytics by category or by subcategories within category.")]
        public async Task<IActionResult> GetSpendingAnalytics([FromQuery] string? catcode, [FromQuery(Name = "start-date")] DateTime? startDate, [FromQuery(Name = "end-date")] DateTime? endDate, [FromQuery] Direction direction)
        {
            var analytics = await _transactionsService.GetSpendingAnalytics(catcode, startDate, endDate, direction);
            
            return Ok(analytics);
        }

    }
}
