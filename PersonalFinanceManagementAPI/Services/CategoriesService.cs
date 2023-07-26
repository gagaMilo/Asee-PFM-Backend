using AutoMapper;
using PersonalFinanceManagementAPI.Commands;
using PersonalFinanceManagementAPI.Database.Entities;
using PersonalFinanceManagementAPI.Database.Repositories;
using PersonalFinanceManagementAPI.Models;

namespace PersonalFinanceManagementAPI.Services
{
    public class CategoriesService : ICategoriesService
    {
        private readonly ICategoriesRepository _repository;
        private readonly IMapper _mapper;

        public CategoriesService(ICategoriesRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }


        public async Task<Categories> ImportCsvCategories(CsvLineCategoriesCommand csvLine)
        {

            var checkIfProductExists = await CheckIfCategoryExistsAsync(csvLine.Name);
            if (!checkIfProductExists)
            {
                var newCategoriesEntity = _mapper.Map<CategoriesEntity>(csvLine);
                await _repository.ImportCategoriesRepository(newCategoriesEntity);
                return _mapper.Map<Categories>(newCategoriesEntity);
            }
            return null;

        }

        private async Task<bool> CheckIfCategoryExistsAsync(string code)
        {

            var transaction = await _repository.GetCategoryByCode(code);
            if (transaction == null)
            {
                return false;

            }
            return true;


        }

        public async Task<CategoriesPagedSortedList<Categories>> GetListCategories(string? parentId)
        {
            var categories = await _repository.GetListCategories(parentId);

            return _mapper.Map<CategoriesPagedSortedList<Categories>>(categories);
        }
       
    }
}
