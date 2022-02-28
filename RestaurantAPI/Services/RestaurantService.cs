using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RestaurantAPI.Authorization;
using RestaurantAPI.Entitites;
using RestaurantAPI.Exceptions;
using RestaurantAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RestaurantAPI.Services
{
    public interface IRestaurantService
    {
        public PagedResult<RestaurantDto> GetRestaurants(RestaurantQuery query);
        public RestaurantDto GetRestaurant(int id);
        public int CreateRestaurant(CreateRestaurantDto dto);
        public void DeleteRestaurant(int id);
        public void UpdateRestaurant(int id, UpdateRestaurantDto dto);
    }
    public class RestaurantService:IRestaurantService
    {
        private readonly RestaurantDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;
        private readonly IAuthorizationService _authorization;
        private readonly IUserContextService _userContextService;
        public RestaurantService(RestaurantDbContext dbContext, IMapper mapper, 
            ILogger<RestaurantService>logger,IAuthorizationService authorization,
            IUserContextService userContextService)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _logger = logger;
            _authorization = authorization;
            _userContextService = userContextService;
        }
        public PagedResult<RestaurantDto> GetRestaurants(RestaurantQuery query)
        {
            var baseQuery = _dbContext
                   .Restaurants
                   .Include(r => r.Address)
                   .Include(r => r.Dishes)
                   .Where(r => query.SearchPhrase == null ||
                        r.Name.ToLower().Contains(query.SearchPhrase.ToLower()) ||
                        r.Description.ToLower().Contains(query.SearchPhrase.ToLower()));//filtrowanie

            if (!string.IsNullOrEmpty(query.SortBy))
            {
                //slownik do informacji konkretnej ekspresji do kolumny
                var columnSelector = new Dictionary<string, Expression<Func<Restaurant, object>>>
                {
                    {nameof(Restaurant.Name),r=>r.Name },
                    {nameof(Restaurant.Description),r=>r.Description },
                    {nameof(Restaurant.Category),r=>r.Category },
                };
                var selectedColumn = columnSelector[query.SortBy];

                baseQuery = query.SortDirection == SortDirection.ASC
                    ? baseQuery.OrderBy(selectedColumn)
                    : baseQuery.OrderByDescending(selectedColumn);
            }


            var restaurants = baseQuery
                   .Skip(query.PageSize*(query.PageNumber-1))//pominiecie elementów wczesniejszych
                   .Take(query.PageSize)//wyswietlenie konkretnych itemow 
                   .ToList();



            var totalItemsCount = baseQuery.Count();

            var restaurantsDtos = _mapper.Map<List<RestaurantDto>>(restaurants);

            var result = new PagedResult<RestaurantDto>(restaurantsDtos, query.PageSize, query.PageSize, totalItemsCount);

            return result;
        }
        public RestaurantDto GetRestaurant(int id)
        {
            var restaurant = _dbContext
                .Restaurants
                .Include(r => r.Address)
                .Include(r => r.Dishes)
                .SingleOrDefault(r => r.Id == id);
            ifRestaurantExist(restaurant);

            var restaurantDto = _mapper.Map<RestaurantDto>(restaurant);

            return restaurantDto;
        }

        public int CreateRestaurant(CreateRestaurantDto dto)
        {         
            var restaurant = _mapper.Map<Restaurant>(dto);
            restaurant.CreatedById = _userContextService.GetUserId;

            _dbContext.Restaurants.Add(restaurant);
            _dbContext.SaveChanges();
            return restaurant.Id;
        }

        public void DeleteRestaurant(int id)
        {
            _logger.LogError($"Restaurant with id: {id} DELETE action invoked");
            var restaurant = _dbContext.Restaurants.SingleOrDefault(r => r.Id == id);
            ifRestaurantExist(restaurant);

            var user = _userContextService.User;
            var authorizationResult = _authorization.AuthorizeAsync(user, restaurant,
                new ResourceOperationRequirement(ResourceOperation.Delete)).Result;
            if (!authorizationResult.Succeeded)
                throw new ForbidException("Not your resource");

            _dbContext.Restaurants.Remove(restaurant);
            _dbContext.SaveChanges();
            //return true;//zeby uniknac zaleznosci miedzy kontrolerem a servisem (serwis nie moze bezposrednio komunikowac sie z klientem, przez co np info o braku restauracji musi byc
            //else return false;             //zwrocona do kontrolera i dopiero tam obsluzona, mozna stworzyc wyjatek, ktory obsluzy ewentualny brak restauracji


        }
        public void UpdateRestaurant(int id,UpdateRestaurantDto dto)
        {
            
            //name descr hasdelivery
            var restaurant = _dbContext.Restaurants.SingleOrDefault(r => r.Id == id);
            ifRestaurantExist(restaurant);
            /*if (restaurant == null) //return false;
                throw new NotFoundException("Restaurant not found");*/
            var user = _userContextService.User;
            var authorizationResult = _authorization.AuthorizeAsync(user, restaurant, 
                new ResourceOperationRequirement(ResourceOperation.Update)).Result;
            if (!authorizationResult.Succeeded)
                throw new ForbidException("Not your resource");

            if (dto.Name != null) restaurant.Name = dto.Name;
            if (dto.Description != null) restaurant.Description = dto.Description;
            restaurant.HasDelivery = dto.HasDelivery;
            _dbContext.SaveChanges();
            //return true;
        }

        private void ifRestaurantExist(Restaurant restaurant)
        {
            if (restaurant == null) //return false;
                throw new NotFoundException("Restaurant not found");
        }
    }
}
