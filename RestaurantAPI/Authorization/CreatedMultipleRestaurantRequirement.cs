using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestaurantAPI.Authorization
{
    public class CreatedMultipleRestaurantRequirement : IAuthorizationRequirement
    {
        public int MinimumRestaurantCreated { get; }
        public CreatedMultipleRestaurantRequirement(int minimumRestaurantCreated)
        {
            MinimumRestaurantCreated = minimumRestaurantCreated;
        }
    }
}
