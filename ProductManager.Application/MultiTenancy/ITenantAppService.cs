using Abp.Application.Services;
using ProductManager.MultiTenancy.Dto;

namespace ProductManager.MultiTenancy;

public interface ITenantAppService : IAsyncCrudAppService<TenantDto, int, PagedTenantResultRequestDto, CreateTenantDto, TenantDto>
{
}

