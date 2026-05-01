using Abp.Application.Services.Dto;

namespace ProductManager.Categories.Dto
{
    public class PagedCategoryResultRequestDto : PagedResultRequestDto
    {
        public string Keyword { get; set; }
    }
}
