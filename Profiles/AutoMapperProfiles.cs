using APItesteInside.DTOs;
using APItesteInside.Models.Domain;
using APItesteInside.Models.Entities;
using AutoMapper;

namespace APItesteInside.Mapping
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            //profiles dos pedidos
            CreateMap<Order, OrdersDTO>().ReverseMap();
            CreateMap<OrderProduct, OrdersDTO.OrderProductDTO>().ReverseMap(); //criar a relação de pedido e produto
            CreateMap<Order, OrderEditDTO>().ReverseMap(); //editar pedido

            //profiles dos produtos
            CreateMap<Product, ProductsDTO>().ReverseMap();
            CreateMap<OrderProduct, ProductsDTO.OrderProductsDTO>().ReverseMap();
            CreateMap<ProductAddDTO, Product>().ReverseMap(); //adicionar produto
            CreateMap<ProductEditDTO, Product>().ReverseMap(); //editar produto
        }
    }
}
