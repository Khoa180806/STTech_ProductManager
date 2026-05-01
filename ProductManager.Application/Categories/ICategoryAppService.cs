using Abp.Application.Services;
using Abp.Application.Services.Dto;
using ProductManager.Categories.Dto;
using System.Threading.Tasks;

namespace ProductManager.Categories
{
    public interface ICategoryAppService : IApplicationService
    {
        Task<CategoryDto> GetAsync(EntityDto<int> input);
        Task<PagedResultDto<CategoryDto>> GetAllAsync(PagedCategoryResultRequestDto input);
        Task<CategoryDto> CreateAsync(CreateCategoryInput input);
        Task<CategoryDto> UpdateAsync(CategoryDto input);
        Task DeleteAsync(EntityDto<int> input);
    }
}
