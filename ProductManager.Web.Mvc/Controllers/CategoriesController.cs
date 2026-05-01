using Abp.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductManager.Authorization;
using ProductManager.Categories;
using ProductManager.Controllers;
using System.Threading.Tasks;

namespace ProductManager.Web.Controllers
{
    [AbpMvcAuthorize(PermissionNames.Pages_Products)]
    public class CategoriesController : ProductManagerControllerBase
    {
        private readonly ICategoryAppService _categoryAppService;

        public CategoriesController(ICategoryAppService categoryAppService)
        {
            _categoryAppService = categoryAppService;
        }

        public async Task<IActionResult> Index()
        {
            var result = await _categoryAppService.GetAllAsync(new Categories.Dto.PagedCategoryResultRequestDto
            {
                MaxResultCount = 10
            });
            return View(result);
        }
    }
}
