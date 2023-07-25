using PersonalFinanceManagementAPI.Database.Entities;
using PersonalFinanceManagementAPI.Models;

namespace PersonalFinanceManagementAPI.Database.Repositories
{
    public interface ICategoriesRepository
    {
        Task<CategoriesEntity> GetCategoryByCode(string code);
        
        Task<CategoriesPagedSortedList<CategoriesEntity>> GetListCategories(string? parentCode = null);
        Task<CategoriesEntity> ImportCategoriesRepository(CategoriesEntity newCategoriesEntity);
    }
}
