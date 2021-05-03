using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestaurantAPI.Models.Validators
{
    public class RestaurantQueryValidator : AbstractValidator<RestaurantQuery>
    {
        private int[] allowedPageSize = new int[] { 5, 10, 15 };
        public RestaurantQueryValidator()
        {
            // Sprawdza czy numer strony jest => od 1
            RuleFor(q => q.PageNumber).GreaterThanOrEqualTo(1);

            RuleFor(q => q.PageSize).Custom((value, context) =>
            {
                if (!allowedPageSize.Contains(value))
                    context.AddFailure("PageSize", $"PageSize must be {string.Join(", ",allowedPageSize)}");
            });
        }
    }
}
