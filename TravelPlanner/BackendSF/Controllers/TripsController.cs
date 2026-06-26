using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TravelPlanner.Common.DTOs.Trip;
using TravelPlanner.Common.Interfaces;
using TravelPlanner.Common.Shared.DTOs;

namespace BackendSF.Controllers
{
    [ApiController]
    [Route("api/trips")]
    public class TripsController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public TripsController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost]
        public async Task<IActionResult> CreateTrip([FromBody] CreateTripDto dto)
        {
            var fabricUrl = _configuration["FabricUrls:TripService"];
            if (string.IsNullOrEmpty(fabricUrl))
            {
                return StatusCode(500, ResultDto<TripDto>.Failure("Service configuration deployment error."));
            }

            try
            {
                var tripServiceProxy = ServiceProxy.Create<ITripService>(new Uri(fabricUrl));
                var result = await tripServiceProxy.CreateTripAsync(dto);
                if (!result.IsSuccess)
                {
                    return BadRequest(result);
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ResultDto<TripDto>.Failure(ex.Message));
            }
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUserTrips(Guid userId)
        {
            var fabricUrl = _configuration["FabricUrls:TripService"];
            if (string.IsNullOrEmpty(fabricUrl))
            {
                return StatusCode(500, ResultDto<List<TripDto>>.Failure("Service configuration deployment error."));
            }

            try
            {
                var tripServiceProxy = ServiceProxy.Create<ITripService>(new Uri(fabricUrl));
                var result = await tripServiceProxy.GetUserTripsAsync(userId);
                if (!result.IsSuccess)
                {
                    return BadRequest(result);
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ResultDto<List<TripDto>>.Failure(ex.Message));
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTrip(Guid id)
        {
            var fabricUrl = _configuration["FabricUrls:TripService"];
            if (string.IsNullOrEmpty(fabricUrl))
            {
                return StatusCode(500, ResultDto<bool>.Failure("Service configuration deployment error."));
            }

            try
            {
                var tripServiceProxy = ServiceProxy.Create<ITripService>(new Uri(fabricUrl));
                var result = await tripServiceProxy.DeleteTripAsync(id);
                if (!result.IsSuccess)
                {
                    return BadRequest(result);
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ResultDto<bool>.Failure(ex.Message));
            }
        }
    }
}