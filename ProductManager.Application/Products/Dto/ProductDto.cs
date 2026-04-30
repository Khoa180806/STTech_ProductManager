using System;
using Abp.Application.Services.Dto;
using Abp.Domain.Entities.Auditing;

namespace ProductManager.Products.Dto;

public class ProductDto : EntityDto<int>, IHasCreationTime
{
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public int stockQuantity { get; set; }
    public int categoryId { get; set; }
    public string categoryName { get; set; }
    public DateTime CreationTime { get; set; }
}