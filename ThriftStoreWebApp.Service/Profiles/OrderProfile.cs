using AutoMapper;
using ThriftStoreWebApp.Models;
using ThriftStoreWebApp.Service.DTOs;

namespace ThriftStoreWebApp.Service.Profiles
{
    public class OrderProfile : Profile
    {
        public OrderProfile()
        {
            // Map OrderItem to OrderItemDto and ignore Id when mapping back
            CreateMap<OrderItem, OrderItemDto>()
                .ReverseMap()
                .ForMember(dest => dest.Id, opt => opt.Ignore());

            // Map ApplicationUser to ClientDto
            CreateMap<ApplicationUser, ClientDto>();

            // Map Order to OrderDto and include nested Client and ClientEmail
            CreateMap<Order, OrderDto>()
                .ForMember(dest => dest.ClientEmail, opt => opt.MapFrom(src => src.Client.Email))
                .ForMember(dest => dest.Client, opt => opt.MapFrom(src => src.Client))
                .ReverseMap()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Client, opt => opt.Ignore()); // Avoid circular mapping
        }
    }
}
