using Microsoft.AspNetCore.Mvc;
using RestaurantAPI.Models;
using RestaurantAPI.Services;
using System.Collections.Generic;

namespace RestaurantAPI.Controllers
{
    [Route("api/restaurant/{restaurantId}/dish")]
    [ApiController]
    public class DishController : ControllerBase
    {
        private readonly IDishService _dishService;
        public DishController(IDishService dishService)
        {
            _dishService = dishService;
        }

        [HttpPost]
        public ActionResult Create([FromRoute] int restaurantId, [FromBody] CreateDishDto dto)
        {
            var newDishId = _dishService.Create(restaurantId, dto);

            return Created($"/api/restaurant/{restaurantId}/dish/{newDishId}", null);
        }

        [HttpGet("{dishId}")]
        public ActionResult<DishDto> Get([FromRoute] int restaurantId, [FromRoute] int dishId)
        {
            var dishDto = _dishService.GetById(restaurantId, dishId);
            return Ok(dishDto);
        }

        [HttpGet]
        public ActionResult<List<DishDto>> Get([FromRoute] int restaurantId)
        {
            var dishDtos = _dishService.GetAll(restaurantId);
            return Ok(dishDtos);
        }

        [HttpDelete("{dishId}")]
        public ActionResult Delate([FromRoute]int restaurantId, [FromRoute]int dishId)
        {
            _dishService.Remove(restaurantId, dishId);
            return NoContent();
        }

        [HttpDelete]
        public ActionResult Delate([FromRoute]int restaurantId) 
        {
            _dishService.RemoveAll(restaurantId);
            return NoContent();
        }
    }
}
