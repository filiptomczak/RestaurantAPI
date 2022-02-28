using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantAPI.Entitites;
using RestaurantAPI.Models;
using RestaurantAPI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RestaurantAPI.Controller
{
    [Route("api/restaurant")]
    [ApiController]//automatycznie waliduje model
    [Authorize]
    public class RestaurantController : ControllerBase
    {

        /*private readonly RestaurantDbContext _dbContext;
        private readonly IMapper _mapper*/
        private readonly IRestaurantService _restaurantServices;
        //private readonly RestaurantService _restaurantServices;
        public RestaurantController(IRestaurantService restaurantService)
        {
            _restaurantServices = restaurantService;
            /*_dbContext = dbContext;
            _mapper = mapper;*/
        }

        /*
        [HttpGet]
        public ActionResult<IEnumerable<Restaurant>> GetRestaurants()
        {
            var restaurants = _dbContext.Restaurants.ToList();
            return Ok(restaurants);
        }
        [HttpGet("{id}")]
        public ActionResult<Restaurant> GetRestaurant([FromRoute]int id)
        {
            var restaurant = _dbContext.Restaurants.SingleOrDefault(r => r.Id == id);
            return restaurant != null ? Ok(restaurant) : NotFound();
        }
        */
        [HttpGet]
        [AllowAnonymous]//wylaczenie autoryzacji do akcji
        //[Authorize(Policy= "Min2RestaurantsAdded")]
        public ActionResult<IEnumerable<RestaurantDto>> GetRestaurants([FromQuery]RestaurantQuery query)
        {
            /*var restaurants = _dbContext
                .Restaurants
                .Include(r => r.Address)
                .Include(r=>r.Dishes)
                .ToList();
            */
            //mapowanie
            /*var restaurantsDtos = restaurants.Select(r => new RestaurantDto()
            {
                Name = r.Name,
                Category = r.Category,
                City = r.Address.City
            });
            var restaurantsDtos = _mapper.Map<List<RestaurantDto>>(restaurants);*/
            var result = _restaurantServices.GetRestaurants(query);
            return Ok(result);
        }
        [HttpGet("{id}")]
        [Authorize(Policy ="HasNationality")]
        [Authorize(Policy ="AtLeast20")]
        public ActionResult<RestaurantDto> GetRestaurant([FromRoute] int id)
        {
            /*var restaurant = _dbContext
                .Restaurants
                .Include(r=>r.Address)
                .Include(r=>r.Dishes)
                .SingleOrDefault(r => r.Id == id);

            var restaurantDto = _mapper.Map<RestaurantDto>(restaurant);*/
            var result = _restaurantServices.GetRestaurant(id);
            return result != null ? Ok(result) : NotFound();
        }

        [HttpPost]
        [Authorize(Roles ="admin")]
        //[Authorize(Roles ="admin,manager")]
        public ActionResult CreateRestaurant([FromBody]CreateRestaurantDto dto)
        {
            //HttpContext.User.IsInRole("admin");//sprawdzenie uprawnien uzytkownika
            /*if (!ModelState.IsValid)//dzieki apiController mozna to usunac
            {
                return BadRequest(ModelState);
            }*/

            /*var restaurant = _mapper.Map<Restaurant>(dto);
            _dbContext.Restaurants.Add(restaurant);
            _dbContext.SaveChanges();*/
            //var userId =int.Parse(User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier).Value);//
            var result = _restaurantServices.CreateRestaurant(dto);
            return Created($"/api/restaurant/{result}",null);
        }

        [HttpDelete("{id}")]
        public ActionResult DeleteRestaurant([FromRoute] int id)
        {
            //var res = _restaurantServices.DeleteRestaurant(id);
            //return res == true ? Ok() : NoContent(); 
            //po usunieciu zaleznosci miedzy servicem a kontrolerem mozna zwracac wartos res bez obawy ze jest null,
            //bo service w razie wartosci null wyrzuca wyjatek o komunikacie restaurant not found
            _restaurantServices.DeleteRestaurant(id);
            return Ok();
        }
        [HttpPut("{id}")]
        public ActionResult UpdateRestaurant([FromRoute] int id, [FromBody] UpdateRestaurantDto dto)
        {
            /*if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }*/
           
            _restaurantServices.UpdateRestaurant(id, dto);
            return Created($"/api/restaurant/{id}", null);

            //var result = _restaurantServices.UpdateRestaurant(id, dto);
            //return result == true ? Created($"/api/restaurant/{result}", null) : NotFound();
        }
    }
}