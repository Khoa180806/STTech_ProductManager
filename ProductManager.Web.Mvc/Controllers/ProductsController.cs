using Abp.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductManager.Authorization;
using ProductManager.Controllers;
using ProductManager.Products;
using System.Threading.Tasks;
using ProductManager.Products.Dto;

namespace ProductManager.Web.Controllers;

[AbpMvcAuthorize(PermissionNames.Pages_Products)]
public class ProductsController : ProductManagerControllerBase
{
    private readonly IProductAppService _productAppService;
    
    public ProductsController(IProductAppService productAppService)
    {
        _productAppService = productAppService;
    }
    
    public async Task<IActionResult> Index()
    {
        var result = await _productAppService.GetAllAsync(new PagedProductResultRequestDto
        {
            MaxResultCount = 10
        });
        return View(result);
    }
    
    [AbpMvcAuthorize(PermissionNames.Pages_Products_Create)]
    public async Task<IActionResult> Create()
    {
        return View();
    }
    
    [AbpMvcAuthorize(PermissionNames.Pages_Products_Edit)]
    public async Task<IActionResult> Edit(int id)
    {
        var product = await  _productAppService.GetAsync(id);
        return View(product);
    }
}