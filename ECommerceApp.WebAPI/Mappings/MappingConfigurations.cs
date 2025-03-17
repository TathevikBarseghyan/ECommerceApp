using AutoMapper;
using ECommerceApp.Domain.Entities;
using ECommerceApp.WebAPI.Models; // Namespace for your DTOs

public class MappingConfigurations : Profile
{
    public MappingConfigurations()
    {
        CreateMap<ProductModel, Product>()
         .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
         .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
         .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price));

        CreateMap<CreateOrderModel, Order>();

        CreateMap<OrderItemModel, OrderItem>()
            .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ProductId))
            .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity))
            .ReverseMap();

        CreateMap<Order, OrderResponseModel>()
            .ForMember(dest => dest.OrderItems, opt => opt.MapFrom(src => src.OrderItems));

    }
}
