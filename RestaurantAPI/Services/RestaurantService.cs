using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RestaurantAPI.Authorization;
using RestaurantAPI.Entities;
using RestaurantAPI.Exceptions;
using RestaurantAPI.Models;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace RestaurantAPI.Services
{
    public interface IRestaurantService
    {
        void Update(int id, UpdateRestaurantDto dto, ClaimsPrincipal user);
        void Delate(int id, ClaimsPrincipal user);
        int Create(CreateRestaurantDto dto, int userId);
        IEnumerable<RestaurantDto> GetAll();
        RestaurantDto GetById(int id);
    }

    public class RestaurantService : IRestaurantService
    {
        private readonly RestaurantDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly ILogger<RestaurantService> _logger;
        private readonly IAuthorizationService _authorizationService;

        public RestaurantService(RestaurantDbContext dbContext, IMapper mapper, ILogger<RestaurantService> logger,
            IAuthorizationService authorizationService)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _logger = logger;
            _authorizationService = authorizationService;
        }

        public void Update(int id, UpdateRestaurantDto dto, ClaimsPrincipal user)
        {
            var restaurant = _dbContext
                .Restaurants
                .FirstOrDefault(r => r.Id == id);

            if (restaurant  is null)
                throw new NotFoundException("Restaurant not found");

            var authorizationResult = _authorizationService.AuthorizeAsync(user, restaurant,
                new ResourceOperationRequirement(ResourceOperation.Update)).Result;

            if (!authorizationResult.Succeeded)
                throw new ForbidException();

            restaurant.Name = dto.Name;
            restaurant.HasDelivery = dto.HasDelivery;
            restaurant.Description = dto.Description;
            
            _dbContext.SaveChanges();
        }

        public void Delate(int id, ClaimsPrincipal user)
        {
            _logger.LogError($"Restaurant with id {id} DELATE action invoked");

            var restaurant = _dbContext.Restaurants
                .FirstOrDefault(r => r.Id == id);

            if (restaurant is null)
                throw new NotFoundException("Restaurant not found");

            var authorizationResult = _authorizationService.AuthorizeAsync(user, restaurant,
                new ResourceOperationRequirement(ResourceOperation.Delate)).Result;

            if (!authorizationResult.Succeeded)
                throw new ForbidException();

            _dbContext.Restaurants.Remove(restaurant);
            _dbContext.SaveChanges();
        }

        public int Create(CreateRestaurantDto dto, int userId)
        {
            var restaurant = _mapper.Map<Restaurant>(dto);
            restaurant.CreatedById = userId;
            _dbContext.Restaurants.Add(restaurant);
            _dbContext.SaveChanges();

            return restaurant.Id;
        }

        public RestaurantDto GetById(int id)
        {
            var restaurant = _dbContext.Restaurants
                        .Include(r => r.Address)
                        .Include(r => r.Dishes)
                        .FirstOrDefault(r => r.Id == id);

            if (restaurant is null)
                throw new NotFoundException("Restaurant not found");

            var result = _mapper.Map<RestaurantDto>(restaurant);
            return result;
        }

        public IEnumerable<RestaurantDto> GetAll()
        {
            var restaurant = _dbContext.Restaurants
                        .Include(r => r.Address)
                        .Include(r => r.Dishes)
                        .ToList();

            var result = _mapper.Map<IEnumerable<RestaurantDto>>(restaurant);
            return result;
        }
    }
}
