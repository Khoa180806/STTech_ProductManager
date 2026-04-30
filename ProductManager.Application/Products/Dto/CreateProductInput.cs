using System.ComponentModel.DataAnnotations;

namespace ProductManager.Products.Dto;

public class CreateProductInput
{
    [Required]
    [MaxLength(256)]
    public string Name { get; set; }
    
    public string Description { get; set; }
    
    [Range(0, 999999999)]
    public decimal Price { get; set; }
    
    [Range(0, int.MaxValue)]
    public int StockQuantity { get; set; }
    
    [Required]
    public int CategoryId { get; set; }
}