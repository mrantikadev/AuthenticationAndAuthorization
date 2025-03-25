using App.Repositories.UnitOfWorks;
using App.Repositories.Users;
using App.Services.ServiceResults;
using App.Services.Users.Create;
using App.Services.Users.Dtos;
using System.Net;

namespace App.Services.Users
{
    public class UserService(IUserRepository userRepository, IUnitOfWork unitOfWork) : IUserService
    {
        public async Task<UserDto?> GetByUsernameAsync(string username)
        {
            var user = await userRepository.GetByUsernameAsync(username);

            return user is null ? null : new UserDto(user.Id, user.Username, user.Role);
        }

        public async Task<ServiceResult<CreateUserRequest>> CreateAsync(CreateUserRequest request)
        {
            var existingUser = await userRepository.GetByUsernameAsync(request.Username);

            if (existingUser is not null)
                return ServiceResult<CreateUserRequest>.Failure("Username already exists.", HttpStatusCode.Conflict);

            var user = new User
            {
                Username = request.Username,
                PasswordHash = request.Password, // Will be hashed later.
                Role = RoleNames.User
            };

            await userRepository.CreateAsync(user);
            await unitOfWork.SaveChangesAsync();

            return ServiceResult<CreateUserRequest>.SuccessAsCreated(request, $"/users/{user.Id}");
        }
    }
}
