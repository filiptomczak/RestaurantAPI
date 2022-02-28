using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestaurantAPI.Exceptions
{
    public class EmailTakenException : Exception
    {
        public EmailTakenException(string message) : base(message)
        {

        }
    }
}