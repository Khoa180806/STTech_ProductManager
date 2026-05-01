using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using ProductManager.Authorization;
using ProductManager.Products.Dto;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProductManager.Entities;

namespace ProductManager.Products
{
    [AbpAuthorize(PermissionNames.Pages_Products)]
    public class ProductAppService :
        AsyncCrudAppService<Product, ProductDto, int, PagedProductResultRequestDto, CreateProductInput, ProductDto>,
        IProductAppService
    {
        public ProductAppService(IRepository<Product, int> repository) : base(repository)
        {
        }

        [AbpAuthorize(PermissionNames.Pages_Products_Create)]
        public override Task<ProductDto> CreateAsync(CreateProductInput input)
        {
            return base.CreateAsync(input);
        }

        [AbpAuthorize(PermissionNames.Pages_Products_Edit)]
        public override Task<ProductDto> UpdateAsync(ProductDto input)
        {
            return base.UpdateAsync(input);
        }

        [AbpAuthorize(PermissionNames.Pages_Products_Delete)]
        public new async Task DeleteAsync(int id)
        {
            await base.DeleteAsync(new EntityDto<int>(id));
        }

        public new async Task<ProductDto> GetAsync(int id)
        {
            return await base.GetAsync(new EntityDto<int>(id));
        }

        protected override IQueryable<Product> CreateFilteredQuery(PagedProductResultRequestDto input)
        {
            var query = base.CreateFilteredQuery(input);

            if (!string.IsNullOrWhiteSpace(input.Keyword))
            {
                query = query.Where(x => x.Name.Contains(input.Keyword) ||
                                         x.Description.Contains(input.Keyword));
            }

            if (input.CategoryId.HasValue)
            {
                query = query.Where(x => x.CategoryId == input.CategoryId.Value);
            }

            if (input.MinPrice.HasValue)
            {
                query = query.Where(x => x.Price >= input.MinPrice.Value);
            }

            if (input.MaxPrice.HasValue)
            {
                query = query.Where(x => x.Price <= input.MaxPrice.Value);
            }

            return query;
        }

        protected override IQueryable<Product> ApplySorting(IQueryable<Product> query,
            PagedProductResultRequestDto input)
        {
            return query.OrderByDescending(x => x.CreationTime);
        }
    }
}