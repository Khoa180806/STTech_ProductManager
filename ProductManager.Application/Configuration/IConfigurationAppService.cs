using ProductManager.Configuration.Dto;
using System.Threading.Tasks;

namespace ProductManager.Configuration;

public interface IConfigurationAppService
{
    Task ChangeUiTheme(ChangeUiThemeInput input);
}
