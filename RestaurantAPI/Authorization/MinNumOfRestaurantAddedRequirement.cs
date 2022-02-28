using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestaurantAPI.Authorization
{
    public class MinNumOfRestaurantAddedRequirement:IAuthorizationRequirement
    {
        public int MinRestaurantsAdded;
        public MinNumOfRestaurantAddedRequirement(int minRestaurantsAdded)
        {
            MinRestaurantsAdded = minRestaurantsAdded;
        }
    }
}
