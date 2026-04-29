using Abp.AspNetCore.Mvc.Authorization;
using ProductManager.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace ProductManager.Web.Controllers;

[AbpMvcAuthorize]
public class HomeController : ProductManagerControllerBase
{
    public ActionResult Index()
    {
        return View();
    }
}
