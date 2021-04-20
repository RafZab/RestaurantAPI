using Microsoft.AspNetCore.Mvc;
using RestaurantAPI.Entities;
using System.Collections.Generic;
using System.Linq;

namespace RestaurantAPI.Controllers
{
    [Route("api/restaurant")]
    public class RestaurantController : ControllerBase
    {
        private readonly RestaurantDbContext _dbContext;
        public RestaurantController(RestaurantDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Restaurant>> GetAll()
        {
            var result = _dbContext.Restaurants
                         .ToList();

            return Ok(result);
        }

        [HttpGet("{id}")]
        public ActionResult<Restaurant> Get (int id)
        {
            var result = _dbContext.Restaurants
                        .FirstOrDefault(r => r.Id == id);

            if (result is null)
                return NotFound();

            return result;
        }

    }
}
