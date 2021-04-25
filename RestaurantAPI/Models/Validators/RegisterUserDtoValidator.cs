using FluentValidation;
using RestaurantAPI.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestaurantAPI.Models.Validators
{
    public class RegisterUserDtoValidator : AbstractValidator<RegisterUserDto>
    {
        public RegisterUserDtoValidator(RestaurantDbContext dbContext)
        {
            RuleFor(u => u.Email)
                .NotEmpty()
                .EmailAddress()
                .MaximumLength(1000);

            RuleFor(u => u.Password)
                .MinimumLength(6);

            RuleFor(u => u.ConfirmPassword)
                .Equal(p => p.Password);

            RuleFor(u => u.Email)
                .Custom((value, context) =>
                {
                    var emialInUse = dbContext.Users.Any(e => e.Email == value);
                    if (emialInUse)
                    {
                        context.AddFailure("Emial", "That emial is taken");
                    }

                });

        }
    }
}
