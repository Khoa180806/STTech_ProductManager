using Abp.Authorization;
using Abp.Localization;
using Abp.MultiTenancy;

namespace ProductManager.Authorization;

public class ProductManagerAuthorizationProvider : AuthorizationProvider
{
    public override void SetPermissions(IPermissionDefinitionContext context)
    {
        context.CreatePermission(PermissionNames.Pages_Users, L("Users"));
        context.CreatePermission(PermissionNames.Pages_Users_Activation, L("UsersActivation"));
        context.CreatePermission(PermissionNames.Pages_Roles, L("Roles"));
        context.CreatePermission(PermissionNames.Pages_Tenants, L("Tenants"), multiTenancySides: MultiTenancySides.Host);
        
        var pages = context.GetPermissionOrNull("Pages") ?? context.CreatePermission("Pages", L("Pages"));

        var products = pages.CreateChildPermission("Pages.Products", L("Products"));
        products.CreateChildPermission("Pages.Products.Create", L("Create"));
        products.CreateChildPermission("Pages.Products.Edit", L("Edit"));
        products.CreateChildPermission("Pages.Products.Delete", L("Delete"));
    }

    private static ILocalizableString L(string name)
    {
        return new LocalizableString(name, ProductManagerConsts.LocalizationSourceName);
    }
}
