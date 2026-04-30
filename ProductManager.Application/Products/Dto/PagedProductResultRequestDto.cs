using Abp.Application.Services.Dto;

namespace ProductManager.Products.Dto;

public class PagedProductResultRequestDto : PagedResultRequestDto
{
    public string Keyword { get; set; }
    public int? CategoryId { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
}