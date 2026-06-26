using Microsoft.ServiceFabric.Services.Remoting;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TravelPlanner.Common.Shared.DTOs;
using TravelPlanner.Common.DTOs.Trip;

namespace TravelPlanner.Common.Interfaces
{
    public interface ITripService : IService
    {
        Task<ResultDto<TripDto>> CreateTripAsync(CreateTripDto dto);
        Task<ResultDto<List<TripDto>>> GetUserTripsAsync(Guid userId);
        Task<ResultDto<bool>> DeleteTripAsync(Guid id);
        Task<ResultDto<bool>> DeleteAllUserTripsAsync(Guid userId);
    }
}