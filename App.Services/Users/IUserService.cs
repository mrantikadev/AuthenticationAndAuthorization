using App.Services.ServiceResults;
using App.Services.Users.Create;
using App.Services.Users.Dtos;

namespace App.Services.Users
{
    public interface IUserService
    {
        Task<UserDto?> GetByUsernameAsync(string username);
        Task<ServiceResult<CreateUserRequest>> CreateAsync(CreateUserRequest request);
    }
}
