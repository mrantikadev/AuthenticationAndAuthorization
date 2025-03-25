using App.Repositories.UnitOfWorks;
using App.Repositories.Users;
using App.Services.Auth;
using App.Services.ServiceResults;
using App.Services.Users.Create;
using App.Services.Users.Dtos;
using System.Net;

namespace App.Services.Users
{
    public class UserService(IUserRepository userRepository, IAuthService authService, IUnitOfWork unitOfWork) : IUserService
    {
        public async Task<ServiceResult<UserDto?>> GetByUsernameAsync(string username)
        {
            var user = await userRepository.GetByUsernameAsync(username);

            if (user is null)
                return ServiceResult<UserDto?>.Failure("Invalid username", HttpStatusCode.NotFound);

            var userAsDto = new UserDto(user.Id, user.Username, user.Role);

            return ServiceResult<UserDto?>.Success(userAsDto);
        }

        public async Task<ServiceResult<CreateUserRequest>> CreateAsync(CreateUserRequest request)
        {
            var existingUser = await userRepository.GetByUsernameAsync(request.Username);

            if (existingUser is not null)
                return ServiceResult<CreateUserRequest>.Failure("Username already exists.", HttpStatusCode.Conflict);

            var hashedPassword = authService.HashPassword(request.Password);

            var user = new User
            {
                Username = request.Username,
                PasswordHash = hashedPassword,
                Role = RoleNames.User
            };

            await userRepository.CreateAsync(user);
            await unitOfWork.SaveChangesAsync();

            return ServiceResult<CreateUserRequest>.SuccessAsCreated(request, $"/users/{user.Id}");
        }
    }
}
