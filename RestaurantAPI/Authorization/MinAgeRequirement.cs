using Microsoft.AspNetCore.Authorization;

namespace RestaurantAPI
{
    public class MinAgeRequirement:IAuthorizationRequirement
    {
        public int Age { get; }

        public MinAgeRequirement(int age)
        {
            Age = age;
        }
    }
}