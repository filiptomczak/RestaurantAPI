using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestaurantAPI.Models
{
    public class UserQuery
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}
