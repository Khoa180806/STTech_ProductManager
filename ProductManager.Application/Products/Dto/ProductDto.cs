using System;
using Abp.Application.Services.Dto;
using Abp.Domain.Entities.Auditing;

namespace ProductManager.Products.Dto;

public class ProductDto : EntityDto<int>, IHasCreationTime
{
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public int CategoryId { get; set; }
    public string CategoryName { get; set; }
    public DateTime CreationTime { get; set; }
}