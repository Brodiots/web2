using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Fabric;
using System.IO;
using System.Threading.Tasks;
using TravelPlanner.Common.Shared.DTOs;
using TravelPlanner.Common.DTOs.Trip;
using TravelPlanner.Common.Interfaces;
using TripService.Data;
using TripService.Services;

namespace TripService
{
    internal sealed class TripService : StatelessService, ITripService
    {
        private readonly DbContextOptions<TripDbContext> _dbOptions;
        private readonly IConfiguration _configuration;

        public TripService(StatelessServiceContext context)
            : base(context)
        {
            _configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false)
                .AddEnvironmentVariables()
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<TripDbContext>();
            optionsBuilder.UseSqlServer(_configuration.GetConnectionString("DefaultConnection"));
            _dbOptions = optionsBuilder.Options;
        }

        public async Task<ResultDto<TripDto>> CreateTripAsync(CreateTripDto dto)
        {
            using (var context = new TripDbContext(_dbOptions))
            {
                var domainService = new TripDomainService(context, _configuration);
                return await domainService.CreateTripAsync(dto);
            }
        }

        public async Task<ResultDto<List<TripDto>>> GetUserTripsAsync(Guid userId)
        {
            using (var context = new TripDbContext(_dbOptions))
            {
                var domainService = new TripDomainService(context, _configuration);
                return await domainService.GetUserTripsAsync(userId);
            }
        }

        public async Task<ResultDto<bool>> DeleteTripAsync(Guid id)
        {
            using (var context = new TripDbContext(_dbOptions))
            {
                var domainService = new TripDomainService(context, _configuration);
                return await domainService.DeleteTripAsync(id);
            }
        }

        public async Task<ResultDto<bool>> DeleteAllUserTripsAsync(Guid userId)
        {
            using (var context = new TripDbContext(_dbOptions))
            {
                var domainService = new TripDomainService(context, _configuration);
                return await domainService.DeleteAllUserTripsAsync(userId);
            }
        }

        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            return this.CreateServiceRemotingInstanceListeners();
        }
    }
}