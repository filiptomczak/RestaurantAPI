using Microsoft.AspNetCore.Mvc;
using RestaurantAPI.Models;
using RestaurantAPI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestaurantAPI.Controller
{

    [Route("api/restaurant/{restaurantId}/dish")]//restaurantId bedzie pobierane ze sciezki kazdej akcji
    [ApiController]//automatyczna walidacja dla każdej akcji
    public class DishController : ControllerBase
    {
        private readonly IDishService _dishService;
        public DishController(IDishService dishService)
        {
            _dishService = dishService;
        }
        [HttpPost]
        public ActionResult Post([FromRoute]int restaurantId,[FromBody] CreateDishDto dto)
        {
            var result = _dishService.CreateDish(restaurantId,dto);
            if (result == 0) 
                return NotFound($"restaurant with this id:{restaurantId} does not exist");
            return Created($"api/{restaurantId}/dish/{result}", null);
        }
        [HttpGet("{dishId}")]
        public ActionResult<DishDto> Get([FromRoute]int restaurantId, [FromRoute]int dishId)
        {
            var result = _dishService.GetDish(restaurantId, dishId);

            return result != null ? Ok(result) : NotFound();
        }
        [HttpGet]
        public ActionResult<IEnumerable<DishDto>>GetAll([FromRoute]int restaurantId)
        {
            var results = _dishService.GetDishes(restaurantId);

            return results != null ? Ok(results) : NotFound();
        }
        [HttpDelete("{dishId}")]
        public ActionResult Delete([FromRoute]int restaurantId,[FromRoute]int dishId)
        {
            _dishService.DeleteDish(restaurantId, dishId);
            return NoContent();
        }
        [HttpDelete]
        public ActionResult DeleteAll([FromRoute] int restaurantId)
        {
            _dishService.DeleteDishes(restaurantId);
            return NoContent();
        }
    }
}
