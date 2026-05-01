using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using ProductManager.Products.Dto;

namespace ProductManager.Products;

public interface IProductAppService : IApplicationService
{
    Task<ProductDto> GetAsync(EntityDto<int> input);
    
    Task<PagedResultDto<ProductDto>> GetAllAsync(PagedProductResultRequestDto input);
    
    Task<ProductDto> CreateAsync(CreateProductInput input);
    
    Task<ProductDto> UpdateAsync(ProductDto input);
    
    Task DeleteAsync(EntityDto<int> input);
}