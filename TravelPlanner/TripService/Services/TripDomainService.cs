using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TravelPlanner.Common.Shared.DTOs;
using TravelPlanner.Common.DTOs.Trip;
using TripService.Data;
using TripService.Entities;

namespace TripService.Services
{
    public class TripDomainService
    {
        private readonly TripDbContext _dbContext;
        private readonly IConfiguration _configuration;

        public TripDomainService(TripDbContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _configuration = configuration;
        }

        public async Task<ResultDto<TripDto>> CreateTripAsync(CreateTripDto dto)
        {
            if (dto.EstimatedBudget < 0)
            {
                return ResultDto<TripDto>.Failure("Budget cannot be a negative value.");
            }
            if (dto.StartDate > dto.EndDate)
            {
                return ResultDto<TripDto>.Failure("End date cannot be scheduled before the start date.");
            }

            var trip = new Trip
            {
                Id = Guid.NewGuid(),
                Title = dto.Title,
                Description = dto.Description,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                EstimatedBudget = dto.EstimatedBudget,
                UserId = dto.UserId
            };

            _dbContext.Trips.Add(trip);
            await _dbContext.SaveChangesAsync();

            var tripDto = new TripDto
            {
                Id = trip.Id,
                Title = trip.Title,
                Description = trip.Description,
                StartDate = trip.StartDate,
                EndDate = trip.EndDate,
                EstimatedBudget = trip.EstimatedBudget,
                UserId = trip.UserId
            };

            return ResultDto<TripDto>.Success(tripDto);
        }

        public async Task<ResultDto<List<TripDto>>> GetUserTripsAsync(Guid userId)
        {
            var trips = await _dbContext.Trips
                .Where(t => t.UserId == userId)
                .Select(t => new TripDto
                {
                    Id = t.Id,
                    Title = t.Title,
                    Description = t.Description,
                    StartDate = t.StartDate,
                    EndDate = t.EndDate,
                    EstimatedBudget = t.EstimatedBudget,
                    UserId = t.UserId
                }).ToListAsync();

            return ResultDto<List<TripDto>>.Success(trips);
        }

        public async Task<ResultDto<bool>> DeleteTripAsync(Guid id)
        {
            var trip = await _dbContext.Trips.FindAsync(id);
            if (trip == null)
            {
                return ResultDto<bool>.Failure("Trip plan not found.");
            }

            _dbContext.Trips.Remove(trip);
            await _dbContext.SaveChangesAsync();

            return ResultDto<bool>.Success(true, "Trip deleted successfully.");
        }

        public async Task<ResultDto<bool>> DeleteAllUserTripsAsync(Guid userId)
        {
            var userTrips = await _dbContext.Trips.Where(t => t.UserId == userId).ToListAsync();
            foreach (var trip in userTrips)
            {
                var deleteResult = await DeleteTripAsync(trip.Id);
                if (!deleteResult.IsSuccess)
                {
                    return ResultDto<bool>.Failure($"Cascade abort at trip execution instance: {trip.Id}");
                }
            }
            return ResultDto<bool>.Success(true);
        }
    }
}