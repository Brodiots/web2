using Microsoft.ServiceFabric.Services.Remoting;
using System;
using System.Threading.Tasks;
using TravelPlanner.Common.Shared.DTOs;

namespace TravelPlanner.Common.Interfaces
{
    public interface ITripService : IService
    {
        Task<ResultDto<bool>> DeleteAllUserTripsAsync(Guid userId);
    }
}