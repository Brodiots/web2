using Microsoft.EntityFrameworkCore;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using TravelPlanner.Common.DTOs.User;
using TravelPlanner.Common.Interfaces;
using TravelPlanner.Common.Shared.DTOs;
using UserService.Data;
using UserService.Entities;

namespace UserService.Services
{
    public class UserDomainService
    {
        private readonly UserDbContext _dbContext;

        public UserDomainService(UserDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ResultDto<UserDto>> RegisterUserAsync(UserRegisterDto dto)
        {
            if (await _dbContext.Users.AnyAsync(u => u.Username == dto.Username))
            {
                return ResultDto<UserDto>.Failure("Username already exists.");
            }

            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = dto.Username,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Role = "User"
            };

            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            var userDto = new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                Role = user.Role
            };

            return ResultDto<UserDto>.Success(userDto, "User registered successfully.");
        }

        public async Task<ResultDto<string>> LoginUserAsync(UserLoginDto dto)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Username == dto.Username);
            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            {
                return ResultDto<string>.Failure("Invalid credentials.");
            }

            string token = Guid.NewGuid().ToString();
            return ResultDto<string>.Success(token, "Login successful.");
        }

        public async Task<ResultDto<UserDto>> GetUserByIdAsync(Guid id)
        {
            var user = await _dbContext.Users.FindAsync(id);
            if (user == null)
            {
                return ResultDto<UserDto>.Failure("User not found.");
            }

            var userDto = new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                Role = user.Role
            };

            return ResultDto<UserDto>.Success(userDto);
        }

        public async Task<ResultDto<bool>> RemoveUserAsync(Guid id)
        {
            var user = await _dbContext.Users.FindAsync(id);
            if (user == null)
            {
                return ResultDto<bool>.Failure("User not found.");
            }

            try
            {
                var tripServiceProxy = ServiceProxy.Create<ITripService>(new Uri("fabric:/TravelPlannerNext/TripService"));
                var cascadeResult = await tripServiceProxy.DeleteAllUserTripsAsync(id);
                if (!cascadeResult.IsSuccess)
                {
                    return ResultDto<bool>.Failure("Failed to delete associated user trips.");
                }
            }
            catch (Exception)
            {
                return ResultDto<bool>.Failure("Trip service communication failure.");
            }

            _dbContext.Users.Remove(user);
            await _dbContext.SaveChangesAsync();

            return ResultDto<bool>.Success(true, "User removed successfully.");
        }
    }
}