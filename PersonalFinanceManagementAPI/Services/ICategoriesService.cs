using PersonalFinanceManagementAPI.Commands;
using PersonalFinanceManagementAPI.Models;

namespace PersonalFinanceManagementAPI.Services
{
    public interface ICategoriesService
    {

        Task<CategoriesPagedSortedList<Categories>> GetListCategories(string? parentId);
        Task<Categories> ImportCsvCategories(CsvLineCategoriesCommand csvLine);
    }
}
