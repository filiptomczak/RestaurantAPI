using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using RestaurantAPI.Entitites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RestaurantAPI.Authorization
{
    public class MinNumOfRestaurantAddedHandler : AuthorizationHandler<MinNumOfRestaurantAddedRequirement>
    {
        private ILogger _logger;
        private readonly RestaurantDbContext _restaurantDbContext;
        public MinNumOfRestaurantAddedHandler(ILogger<MinNumOfRestaurantAddedHandler>logger, RestaurantDbContext restaurantDbContext)
        {
            _logger = logger;
            _restaurantDbContext = restaurantDbContext;
        }
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, MinNumOfRestaurantAddedRequirement requirement)
        {
            var userId = int.Parse(context.User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier).Value);
            var restaurantAdded = _restaurantDbContext.Restaurants.Count(r => r.CreatedById == userId); 
            if (restaurantAdded >= requirement.MinRestaurantsAdded)
            {
                context.Succeed(requirement);
            }
            return Task.CompletedTask;
        }
    }
}
