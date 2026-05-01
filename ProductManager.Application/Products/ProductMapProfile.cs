using AutoMapper;
using ProductManager.Entities;
using ProductManager.Products.Dto;

namespace ProductManager.Products
{
    public class ProductMapProfile : Profile
    {
        public ProductMapProfile()
        {
            CreateMap<Product, ProductDto>()
                .ForMember(dest => dest.CategoryName, 
                           opt => opt.MapFrom(src => src.Category.Name));
            
            CreateMap<CreateProductInput, Product>();
            
            CreateMap<ProductDto, Product>();
        }
    }
}
