using System.ComponentModel.DataAnnotations;

namespace ProductManager.Categories.Dto
{
    public class CreateCategoryInput
    {
        [Required]
        [MaxLength(128)]
        public string Name { get; set; }
        
        public string Description { get; set; }
    }
}
