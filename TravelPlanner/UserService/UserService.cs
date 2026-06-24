using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;
using Microsoft.EntityFrameworkCore;
using System.Fabric;
using TravelPlanner.Common.Interfaces;
using UserService.Data;
using UserService.Services;
using TravelPlanner.Common.Shared.DTOs;
using TravelPlanner.Common.DTOs.User;

namespace UserService
{
    internal sealed class UserService : StatelessService, IUserService
    {
        private readonly DbContextOptions<UserDbContext> _dbOptions;

        public UserService(StatelessServiceContext context)
            : base(context)
        {
            var optionsBuilder = new DbContextOptionsBuilder<UserDbContext>();
            optionsBuilder.UseSqlServer("Server=localhost;Database=TravelPlannerUsersNextDB;Trusted_Connection=True;TrustServerCertificate=True;");
            _dbOptions = optionsBuilder.Options;
        }

        public async Task<ResultDto<UserDto>> RegisterAsync(UserRegisterDto dto)
        {
            using (var context = new UserDbContext(_dbOptions))
            {
                var domainService = new UserDomainService(context);
                return await domainService.RegisterUserAsync(dto);
            }
        }

        public async Task<ResultDto<string>> LoginAsync(UserLoginDto dto)
        {
            using (var context = new UserDbContext(_dbOptions))
            {
                var domainService = new UserDomainService(context);
                return await domainService.LoginUserAsync(dto);
            }
        }

        public async Task<ResultDto<UserDto>> GetUserByIdAsync(Guid id)
        {
            using (var context = new UserDbContext(_dbOptions))
            {
                var domainService = new UserDomainService(context);
                return await domainService.GetUserByIdAsync(id);
            }
        }

        public async Task<ResultDto<bool>> DeleteUserAsync(Guid id)
        {
            using (var context = new UserDbContext(_dbOptions))
            {
                var domainService = new UserDomainService(context);
                return await domainService.RemoveUserAsync(id);
            }
        }

        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            return this.CreateServiceRemotingInstanceListeners();
        }
    }
}