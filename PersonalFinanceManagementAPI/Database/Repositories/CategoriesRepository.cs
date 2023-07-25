using PersonalFinanceManagementAPI.Database.Entities;

using PersonalFinanceManagementAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace PersonalFinanceManagementAPI.Database.Repositories
{
    public class CategoriesRepository : ICategoriesRepository
    {

        PFMDbContext _dbContext;

        public CategoriesRepository(PFMDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<CategoriesEntity> GetCategoryByCode(string code)
        {
            return await _dbContext.Categories.FirstOrDefaultAsync(x => x.Code.Equals(code));
        }

        public async Task<CategoriesEntity> ImportCategoriesRepository(CategoriesEntity newCategoriesEntity)
        {
            var existingCategory = await _dbContext.Categories.FirstOrDefaultAsync(x => x.Code.Equals(newCategoriesEntity.Code));

            if (existingCategory != null)
            {
                // Azuriram naziv kategorije
                existingCategory.Name = newCategoriesEntity.Name;
                // Azuriram roditeljski kod (ako postoji)
                existingCategory.ParentCode = newCategoriesEntity.ParentCode;

                // Azuriram kategoriju u bazi podataka
                _dbContext.Categories.Update(existingCategory);
            }
            else
            {
                // Dodam novu kategoriju u bazu podataka
                _dbContext.Categories.Add(newCategoriesEntity);
            }
            await _dbContext.SaveChangesAsync();
            return newCategoriesEntity;
        }

        public async Task<CategoriesPagedSortedList<CategoriesEntity>> GetListCategories(string? parentCode = null)
        {
            var query = _dbContext.Categories.AsQueryable();


            //sortiranje
            if (!String.IsNullOrEmpty(parentCode))
            {
                query = query.OrderBy(x => x.Code);
             
            }
            else
            {
                query = query.OrderBy(x => x.Name);
            }


            if (!String.IsNullOrEmpty(parentCode))
            {
                query = query.Where(x => x.ParentCode == parentCode);
            }






            var categories = await query.ToListAsync();

            return new CategoriesPagedSortedList<CategoriesEntity>
            {

                Items = categories


            };
        }
    }
}



   