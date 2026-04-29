using Abp.AspNetCore.Mvc.ViewComponents;

namespace ProductManager.Web.Views;

public abstract class ProductManagerViewComponent : AbpViewComponent
{
    protected ProductManagerViewComponent()
    {
        LocalizationSourceName = ProductManagerConsts.LocalizationSourceName;
    }
}
