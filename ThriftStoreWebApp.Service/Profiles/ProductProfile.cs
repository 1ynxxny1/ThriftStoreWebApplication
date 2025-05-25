using AutoMapper;
using ThriftStoreWebApp.Models;

namespace ThriftStoreWebApp.Service.Profiles
{
    public class ProductProfile : Profile
    {
        public ProductProfile()
        {
            CreateMap<Product, ProductDto>()
                .ForMember(dest => dest.ImageFile, opt => opt.Ignore()); 

            CreateMap<ProductDto, Product>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.ImageFileName, opt => opt.Ignore()); 
        }
    }
}
