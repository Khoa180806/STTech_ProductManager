using AutoMapper;
using ProductManager.Categories.Dto;
using ProductManager.Entities;

namespace ProductManager.Categories
{
    public class CategoryMapProfile : Profile
    {
        public CategoryMapProfile()
        {
            CreateMap<Category, CategoryDto>();
            CreateMap<CreateCategoryInput, Category>();
            CreateMap<CategoryDto, Category>();
        }
    }
}
