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
        void Update(int id, UpdateRestaurantDto dto);
        void Delate(int id);
        int Create(CreateRestaurantDto dto);
        PagedResult<RestaurantDto> GetAll(RestaurantQuery query);
        RestaurantDto GetById(int id);
    }

    public class RestaurantService : IRestaurantService
    {
        private readonly RestaurantDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly ILogger<RestaurantService> _logger;
        private readonly IAuthorizationService _authorizationService;
        private readonly IUserContextService _userContextService;
        public RestaurantService(RestaurantDbContext dbContext, IMapper mapper, ILogger<RestaurantService> logger,
            IAuthorizationService authorizationService, IUserContextService userContextService)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _logger = logger;
            _authorizationService = authorizationService;
            _userContextService = userContextService;
        }

        public void Update(int id, UpdateRestaurantDto dto)
        {
            var restaurant = _dbContext
                .Restaurants
                .FirstOrDefault(r => r.Id == id);

            if (restaurant  is null)
                throw new NotFoundException("Restaurant not found");

            var authorizationResult = _authorizationService.AuthorizeAsync(_userContextService.User, restaurant,
                new ResourceOperationRequirement(ResourceOperation.Update)).Result;

            if (!authorizationResult.Succeeded)
                throw new ForbidException();

            restaurant.Name = dto.Name;
            restaurant.HasDelivery = dto.HasDelivery;
            restaurant.Description = dto.Description;
            
            _dbContext.SaveChanges();
        }

        public void Delate(int id)
        {
            _logger.LogError($"Restaurant with id {id} DELATE action invoked");

            var restaurant = _dbContext.Restaurants
                .FirstOrDefault(r => r.Id == id);

            if (restaurant is null)
                throw new NotFoundException("Restaurant not found");

            var authorizationResult = _authorizationService.AuthorizeAsync(_userContextService.User, restaurant,
                new ResourceOperationRequirement(ResourceOperation.Delate)).Result;

            if (!authorizationResult.Succeeded)
                throw new ForbidException();

            _dbContext.Restaurants.Remove(restaurant);
            _dbContext.SaveChanges();
        }

        public int Create(CreateRestaurantDto dto)
        {
            var restaurant = _mapper.Map<Restaurant>(dto);
            restaurant.CreatedById = _userContextService.GetUserId;
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

        public PagedResult<RestaurantDto> GetAll(RestaurantQuery query)
        {
            var baseQuery = _dbContext.Restaurants
                        .Include(r => r.Address)
                        .Include(r => r.Dishes)
                        .Where(r => query.SearchPhrase == null || (r.Name.ToLower().Contains(query.SearchPhrase.ToLower()) 
                                                                   || r.Description.ToLower().Contains(query.SearchPhrase.ToLower())));

            var restaurants = baseQuery
                        .Skip(query.PageSize - (query.PageNumber - 1))
                        .Take(query.PageSize)
                        .ToList();

            var totalItemsCount = baseQuery.Count();

            var restaurantsDto = _mapper.Map<List<RestaurantDto>>(restaurants);

            var result = new PagedResult<RestaurantDto>(restaurantsDto, totalItemsCount, query.PageSize, query.PageNumber);

            return result;
        }
    }
}
