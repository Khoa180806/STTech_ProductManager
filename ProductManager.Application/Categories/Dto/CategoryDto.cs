using Abp.Application.Services.Dto;
using Abp.Domain.Entities.Auditing;
using System;

namespace ProductManager.Categories.Dto
{
    public class CategoryDto : EntityDto<int>, IHasCreationTime
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreationTime { get; set; }
    }
}
