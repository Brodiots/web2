using Microsoft.ServiceFabric.Services.Remoting;
using System;
using System.Threading.Tasks;
using TravelPlanner.Common.DTOs.User;
using TravelPlanner.Common.Shared.DTOs;

namespace TravelPlanner.Common.Interfaces
{
    public interface IUserService : IService
    {
        Task<ResultDto<UserDto>> RegisterAsync(UserRegisterDto dto);
        Task<ResultDto<string>> LoginAsync(UserLoginDto dto);
        Task<ResultDto<UserDto>> GetUserByIdAsync(Guid id);
        Task<ResultDto<bool>> DeleteUserAsync(Guid id);
    }
}