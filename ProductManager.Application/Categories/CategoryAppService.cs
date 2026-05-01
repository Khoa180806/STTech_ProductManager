using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using ProductManager.Authorization;
using ProductManager.Categories.Dto;
using ProductManager.Entities;
using System.Linq;
using System.Threading.Tasks;

namespace ProductManager.Categories
{
    [AbpAuthorize(PermissionNames.Pages_Products)]
    public class CategoryAppService : 
        AsyncCrudAppService<Category, CategoryDto, int, PagedCategoryResultRequestDto, CreateCategoryInput, CategoryDto>, 
        ICategoryAppService
    {
        public CategoryAppService(IRepository<Category, int> repository) : base(repository)
        {
        }

        public override async Task<CategoryDto> GetAsync(EntityDto<int> input)
        {
            return await base.GetAsync(input);
        }

        public override async Task DeleteAsync(EntityDto<int> input)
        {
            await base.DeleteAsync(input);
        }

        protected override IQueryable<Category> CreateFilteredQuery(PagedCategoryResultRequestDto input)
        {
            var query = base.CreateFilteredQuery(input);

            if (!string.IsNullOrWhiteSpace(input.Keyword))
            {
                query = query.Where(x => x.Name.Contains(input.Keyword));
            }

            return query;
        }
    }
}
