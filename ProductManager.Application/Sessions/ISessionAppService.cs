using Abp.Application.Services;
using ProductManager.Sessions.Dto;
using System.Threading.Tasks;

namespace ProductManager.Sessions;

public interface ISessionAppService : IApplicationService
{
    Task<GetCurrentLoginInformationsOutput> GetCurrentLoginInformations();
}
