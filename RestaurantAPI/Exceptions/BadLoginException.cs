using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestaurantAPI.Exceptions
{
    public class BadLoginException:Exception
    {
        public BadLoginException(string message):base(message)
        {

        }
    }
}
