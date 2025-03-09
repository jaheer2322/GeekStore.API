using AutoMapper;
using GeekStore.API.Models.Domains;
using GeekStore.API.Models.DTOs;
namespace GeekStore.API.Mappings
{
    public class MappingProfiles: Profile
    {
        public MappingProfiles()
        {
            CreateMap<Tier, TierDto>().ReverseMap();
            CreateMap<Tier, AddTierRequestDto>().ReverseMap();
            CreateMap<Tier, UpdateTierRequestDto>().ReverseMap();
            CreateMap<Product, ProductDto>().ReverseMap();
            CreateMap<Product, AddProductRequestDto>().ReverseMap();
            CreateMap<Product, UpdateProductRequestDto>().ReverseMap();
            CreateMap<Category, CategoryDto>().ReverseMap();
            CreateMap<Category, AddCategoryRequestDto>().ReverseMap();
            CreateMap<Category, UpdateCategoryRequestDto>().ReverseMap();
        }
    }
}
