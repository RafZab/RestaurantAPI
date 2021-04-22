using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RestaurantAPI.Entities;
using RestaurantAPI.Exceptions;
using RestaurantAPI.Models;
using System.Collections.Generic;
using System.Linq;

namespace RestaurantAPI.Services
{
    public interface IRestaurantService
    {
        void Update(int id, UpdateRestaurantDto dto);
        void Delate(int id);
        int Create(CreateRestaurantDto dto);
        IEnumerable<RestaurantDto> GetAll();
        RestaurantDto GetById(int id);
    }

    public class RestaurantService : IRestaurantService
    {
        private readonly RestaurantDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly ILogger<RestaurantService> _logger;

        public RestaurantService(RestaurantDbContext dbContext, IMapper mapper, ILogger<RestaurantService> logger)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _logger = logger;
        }

        public void Update(int id, UpdateRestaurantDto dto)
        {
            var resaurant = _dbContext
                .Restaurants
                .FirstOrDefault(r => r.Id == id);

            if (resaurant is null)
                throw new NotFoundException("Restaurant not found");

            resaurant.Name = dto.Name;
            resaurant.HasDelivery = dto.HasDelivery;
            resaurant.Description = dto.Description;
            
            _dbContext.SaveChanges();
        }

        public void Delate(int id)
        {
            _logger.LogError($"Restaurant with id {id} DELATE action invoked");

            var restaurant = _dbContext.Restaurants
                .FirstOrDefault(r => r.Id == id);

            if (restaurant is null)
                throw new NotFoundException("Restaurant not found");

            _dbContext.Restaurants.Remove(restaurant);
            _dbContext.SaveChanges();
        }

        public int Create(CreateRestaurantDto dto)
        {
            var restaurant = _mapper.Map<Restaurant>(dto);
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
