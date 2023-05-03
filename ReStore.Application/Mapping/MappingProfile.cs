using AutoMapper;
using ReStore.Application.Models;
using ReStore.Domain.Entities;
using ReStore.Domain.Entities.OrderAggregate;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace ReStore.Application.Mapping
{
    [ExcludeFromCodeCoverage]
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Basket, BasketModel>()
            .ForMember(dest => dest.BuyerId, opt => opt.MapFrom(src => src.BuyerId))
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.PaymentIntentId, opt => opt.MapFrom(src => src.PaymentIntentId))
            .ForMember(dest => dest.ClientSecret, opt => opt.MapFrom(src => src.ClientSecret))
            .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items.Select(item => new BasketItemModel
            {
                Brand = item.Product.Brand,
                Name = item.Product.Name,
                PictureUrl = item.Product.PictureUrl,
                Price = item.Product.Price,
                ProductId = item.Product.Id,
                Quantity = item.Quantity,
                Type = item.Product.Type
            }).ToList()));

            CreateMap<BasketModel, Basket>()
            .ForMember(dest => dest.BuyerId, opt => opt.MapFrom(src => src.BuyerId))
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items.Select(item => new BasketItem
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                Product = new Product
                {
                    Brand = item.Brand,
                    Id = item.ProductId,
                    Name = item.Name,
                    PictureUrl = item.PictureUrl,
                    Price = item.Price,
                    QuantityInStock = item.Quantity,
                    Type = item.Type
                }
            }).ToList()));

            CreateMap<Order, OrderModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.BuyerId, opt => opt.MapFrom(src => src.BuyerId))
                .ForMember(dest => dest.ShippingAddress, opt => opt.MapFrom(src => src.ShippingAddress))
                .ForMember(dest => dest.OrderDate, opt => opt.MapFrom(src => src.OrderDate))
                .ForMember(dest => dest.Subtotal, opt => opt.MapFrom(src => src.Subtotal))
                .ForMember(dest => dest.DeliveryFee, opt => opt.MapFrom(src => src.DeliveryFee))
                .ForMember(dest => dest.OrderStatus, opt => opt.MapFrom(src => src.OrderStatus.ToString()))
                .ForMember(dest => dest.OrderItems, opt => opt.MapFrom(src => src.OrderItems.Select(item => new OrderItemModel
                {
                    Name = item.ItemOrdered.Name,
                    PictureUrl = item.ItemOrdered.PictureUrl,
                    Price = item.Price,
                    ProductId = item.ItemOrdered.ProductId,
                    Quantity = item.Quantity
                })));

            CreateMap<ViaCepAddress, ShippingAddress>()
                .ForMember(dest => dest.Street, opt => opt.MapFrom(src => src.Logradouro))
                .ForMember(dest => dest.Complement, opt => opt.MapFrom(src => src.Complemento))
                .ForMember(dest => dest.State, opt => opt.MapFrom(src => src.Uf))
                .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.Localidade))
                .ForMember(dest => dest.Zipcode, opt => opt.MapFrom(src => src.Cep.Replace("-", "")))
                .ForMember(dest => dest.Neighborhood, opt => opt.MapFrom(src => src.Bairro));
        }
    }
}
