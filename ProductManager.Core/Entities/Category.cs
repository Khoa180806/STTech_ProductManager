using System.Collections.Generic;
using Abp.Domain.Entities.Auditing;

namespace ProductManager.Entities;

public class Category : FullAuditedEntity<int>
{
    public string Name { get; set; }
    public string Description { get; set; }
    
    public virtual ICollection<Product> Products { get; set; }
}