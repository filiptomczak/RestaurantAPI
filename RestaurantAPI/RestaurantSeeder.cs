using Microsoft.EntityFrameworkCore;
using RestaurantAPI.Entitites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestaurantAPI
{
    public class RestaurantSeeder
    {
        private readonly RestaurantDbContext _dbContext;
        public RestaurantSeeder(RestaurantDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        private IEnumerable<Restaurant> GetRestaurants()
        {
            var restaurants = new List<Restaurant>()
            {
                new Restaurant()
                {
                    Name = "Lajla",
                    Category = "Kebap",
                    Description = "Turkish style kebap, based in Wrzesnia by Hamid, Drakula and driver Leniwe Oko",
                    ContactMail = "contact@laiila.pl",
                    HasDelivery = true,
                    Dishes = new List<Dish>()
                    {
                        new Dish()
                        {
                            Name = "Tortilla",
                            Price = 14.0M
                        },
                        new Dish()
                        {
                            Name = "Bulka",
                            Price = 13.0M
                        },
                    },
                    Address = new Address()
                    {
                        City = "Wrzesnia",
                        Street = "Rynek 3",
                        ZipCode = "62-300"
                    }
                },

                new Restaurant()
                {
                    Name = "WeWe",
                    Category = "Restaurant",
                    Description = "Polish fancy restaurant, polish kitchen and wines from Europe",
                    ContactMail = "contact@wewe.pl",
                    HasDelivery = false,
                    Dishes = new List<Dish>()
                    {
                        new Dish()
                        {
                            Name = "Sandacz z talarkami",
                            Price = 38.0M
                        },
                        new Dish()
                        {
                            Name = "Deska serow",
                            Price = 50.0M
                        },
                    },
                    Address = new Address()
                    {
                        City = "Wrzesnia",
                        Street = "Rynek 2",
                        ZipCode = "62-300"
                    }
                }
            };
            return restaurants;
        }


        
        public void Seed()
        {
            if (_dbContext.Database.CanConnect()) 
            {
                //automatyczne migracje 
                var pendingMigration = _dbContext.Database.GetPendingMigrations();
                if(pendingMigration != null && pendingMigration.Any())
                {
                    _dbContext.Database.Migrate();
                }
                if (!_dbContext.Restaurants.Any())
                {
                    var restaurants = GetRestaurants();

                    _dbContext.AddRange(restaurants);
                    _dbContext.SaveChanges();
                }
                if (!_dbContext.Roles.Any())
                {
                    var roles = GetRoles();
                    _dbContext.AddRange(roles);
                    _dbContext.SaveChanges();
                }
            }
        }

        private IEnumerable<Role> GetRoles()
        {
            var roles = new List<Role>() 
            { 
                new Role() { Name = "admin" },
                new Role() { Name = "manager" }, 
                new Role() { Name = "user" } 
            };
            return roles;
        }
    }
}
