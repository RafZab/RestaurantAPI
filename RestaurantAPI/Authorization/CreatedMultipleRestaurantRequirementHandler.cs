using Microsoft.AspNetCore.Authorization;
using RestaurantAPI.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RestaurantAPI.Authorization
{
    public class CreatedMultipleRestaurantRequirementHandler : AuthorizationHandler<CreatedMultipleRestaurantRequirement>
    {
        private readonly RestaurantDbContext _dbContext;
        public CreatedMultipleRestaurantRequirementHandler(RestaurantDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, CreatedMultipleRestaurantRequirement requirement)
        {
            var userId = int.Parse(context.User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier).Value);

            var createdRestaurantsCount = _dbContext
                .Restaurants
                .Count(r => r.CreatedById == userId);

            if (createdRestaurantsCount > requirement.MinimumRestaurantCreated)
                context.Succeed(requirement);

            return Task.CompletedTask;
        }
    }
}
