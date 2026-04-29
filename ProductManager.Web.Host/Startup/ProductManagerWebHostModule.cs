using Abp.Modules;
using Abp.Reflection.Extensions;
using ProductManager.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace ProductManager.Web.Host.Startup
{
    [DependsOn(
       typeof(ProductManagerWebCoreModule))]
    public class ProductManagerWebHostModule : AbpModule
    {
        private readonly IWebHostEnvironment _env;
        private readonly IConfigurationRoot _appConfiguration;

        public ProductManagerWebHostModule(IWebHostEnvironment env)
        {
            _env = env;
            _appConfiguration = env.GetAppConfiguration();
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(ProductManagerWebHostModule).GetAssembly());
        }
    }
}
