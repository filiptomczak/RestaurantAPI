using AutoMapper;
using Microsoft.EntityFrameworkCore;
using NLog;
using RestaurantAPI.Entitites;
using RestaurantAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RestaurantAPI.Exceptions;

namespace RestaurantAPI.Services
{
    public interface IDishService
    {
        public int CreateDish(int restaurantId, CreateDishDto dto);
        public DishDto GetDish(int restaurantId, int dishId);
        public IEnumerable<DishDto> GetDishes(int restaurantId);
        public void DeleteDish(int restaurantId, int dishId);
        public void DeleteDishes(int restaurantId);
    }
    
    public class DishService:IDishService
    {
        private readonly RestaurantDbContext _dbContext;
        private readonly IMapper _mapper;
        public DishService(RestaurantDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public int CreateDish(int restaurantId,CreateDishDto dto)
        {
            var restaurant = GetRestaurantById(restaurantId);
            
            var dish = _mapper.Map<Dish>(dto);
            dish.RestaurantId = restaurantId;
            _dbContext.Dishes.Add(dish);
            _dbContext.SaveChanges();
            return dish.Id;
        }

        public DishDto GetDish(int restaurantId,int dishId)
        {
            var restaurant = GetRestaurantById(restaurantId);
            var dish = GetDishById(dishId, restaurant);
            var dishDto = _mapper.Map<DishDto>(dish);
            return dishDto;

        }

        public IEnumerable<DishDto> GetDishes(int restaurantId)
        {
            var restaurant = GetRestaurantById(restaurantId);

            var dishes = restaurant.Dishes.ToList();
            var dishesDto = new List<DishDto>();
            foreach(var dish in dishes)
            {
                var dishDto = _mapper.Map<DishDto>(dish);
                dishesDto.Add(dishDto);
            }
            //var dishesDto2 = _mapper.Map<List<DishDto>>(dishes);

            return dishesDto;

        }
        public void DeleteDish(int restaurantId, int dishId)
        {
            var restaurant = GetRestaurantById(restaurantId);
            var dish = GetDishById(dishId, restaurant);

            _dbContext.Dishes.Remove(dish);
            _dbContext.SaveChanges();
        }
        public void DeleteDishes(int restaurantId)
        {
            var restaurant = GetRestaurantById(restaurantId);
            _dbContext.RemoveRange(restaurant.Dishes);
            _dbContext.SaveChanges();
            /*foreach(var dish in restaurant.Dishes)
            {
                restaurant.Dishes.Remove(dish);
            }*/
        }

        private Restaurant GetRestaurantById(int restaurantId)
        {
            var restaurant = _dbContext.Restaurants.Include(r => r.Dishes).SingleOrDefault(r => r.Id == restaurantId);
            if (restaurant is null)
                throw new NotFoundException("Restaurant not found");
            return restaurant;
        }
        private Dish GetDishById(int dishId, Restaurant restaurant)
        {
            var dish = restaurant.Dishes.SingleOrDefault(d => d.Id == dishId);
            if(dish is null)
                throw new NotFoundException("Dish not found");
            return dish;
        }

    }
}
