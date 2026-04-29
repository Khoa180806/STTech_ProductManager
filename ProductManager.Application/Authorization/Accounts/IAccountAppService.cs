using Abp.Application.Services;
using ProductManager.Authorization.Accounts.Dto;
using System.Threading.Tasks;

namespace ProductManager.Authorization.Accounts;

public interface IAccountAppService : IApplicationService
{
    Task<IsTenantAvailableOutput> IsTenantAvailable(IsTenantAvailableInput input);

    Task<RegisterOutput> Register(RegisterInput input);
}
