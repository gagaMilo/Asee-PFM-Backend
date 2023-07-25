using CsvHelper;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using PersonalFinanceManagementAPI.Commands;
using PersonalFinanceManagementAPI.Mappings;
using PersonalFinanceManagementAPI.Models;
using PersonalFinanceManagementAPI.Services;
using System.Globalization;

namespace PersonalFinanceManagementAPI.Controllers
{
    [EnableCors("MyCORSPolicy")]
    [ApiController]
    [Route("categories")] //Ruta kontrolera
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoriesService _categoriesService;

        private readonly ILogger<CategoriesController> _logger;


        public CategoriesController(ILogger<CategoriesController> logger, ICategoriesService categoriesService)
        {
            _logger = logger;
            _categoriesService = categoriesService;

        }

       
        
        // POST : /categories/import 
        [HttpPost("import")]
        public async Task<IActionResult> ImportFromCsvCategories(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Please upload a valid CSV file.");
            try
            {

                var categories = GetCsvCategories(file);


                foreach (var row in categories)
                {
                    await _categoriesService.ImportCsvCategories(row);

                }

                return Ok();


            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while importing categories.");

                return StatusCode(500);
            }
        }

        // GET : /categories   
        [HttpGet]
        public async Task<IActionResult> GetListTransactions([FromQuery] string? parentId = null)
        {
            var categories = await _categoriesService.GetListCategories(parentId);
            return Ok(categories);
        }



        private List<CsvLineCategoriesCommand> GetCsvCategories(IFormFile file)
        {
            using (var fs = new StreamReader(file.OpenReadStream()))

            using (var csvFile = new CsvReader(fs, CultureInfo.InvariantCulture))

            {
                csvFile.Context.RegisterClassMap<CategoriesMapper>();

                if (csvFile != null)
                {
                    return csvFile.GetRecords<CsvLineCategoriesCommand>().ToList();
                }
                return null;
            }
        }



    }
}