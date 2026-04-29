using Abp.Modules;
using Abp.Reflection.Extensions;
using ProductManager.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace ProductManager.Web.Startup;

[DependsOn(typeof(ProductManagerWebCoreModule))]
public class ProductManagerWebMvcModule : AbpModule
{
    private readonly IWebHostEnvironment _env;
    private readonly IConfigurationRoot _appConfiguration;

    public ProductManagerWebMvcModule(IWebHostEnvironment env)
    {
        _env = env;
        _appConfiguration = env.GetAppConfiguration();
    }

    public override void PreInitialize()
    {
        Configuration.Navigation.Providers.Add<ProductManagerNavigationProvider>();
    }

    public override void Initialize()
    {
        IocManager.RegisterAssemblyByConvention(typeof(ProductManagerWebMvcModule).GetAssembly());
    }
}
