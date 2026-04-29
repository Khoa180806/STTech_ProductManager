using Abp.AspNetCore.Mvc.Views;
using Abp.Runtime.Session;
using Microsoft.AspNetCore.Mvc.Razor.Internal;

namespace ProductManager.Web.Views;

public abstract class ProductManagerRazorPage<TModel> : AbpRazorPage<TModel>
{
    [RazorInject]
    public IAbpSession AbpSession { get; set; }

    protected ProductManagerRazorPage()
    {
        LocalizationSourceName = ProductManagerConsts.LocalizationSourceName;
    }
}
