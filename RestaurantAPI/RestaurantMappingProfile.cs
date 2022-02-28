using AutoMapper;
using RestaurantAPI.Entitites;
using RestaurantAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestaurantAPI
{
    public class RestaurantMappingProfile : Profile
    {
        public RestaurantMappingProfile()
        {
            CreateMap<Restaurant, RestaurantDto>()
                .ForMember(m => m.City, c => c.MapFrom(s => s.Address.City))
                .ForMember(m => m.Street, c => c.MapFrom(s => s.Address.Street))
                .ForMember(m => m.ZipCode, c => c.MapFrom(s => s.Address.ZipCode));
            //recznie trzeba wpisac tylko te wlasciwosci, ktore roznia sie nazwami, reszta jest automatycznie mapowana
            CreateMap<Dish, DishDto>();

            CreateMap<CreateRestaurantDto, Restaurant>()
                .ForMember(m => m.Address,
                    r => r.MapFrom(dto => new Address()
                    { City = dto.City, Street = dto.Street, ZipCode = dto.ZipCode }));

            CreateMap<CreateDishDto, Dish>();

            CreateMap<RegisterUserDto, User>()
                .ForMember(u=>u.PasswordHash,
                    r=>r.MapFrom(dto=>dto.Password));
        }
    }
}
