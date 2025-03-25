using App.Repositories.Users;
using App.Services.Auth.Login;
using App.Services.ServiceResults;
using App.Services.Users.Dtos;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace App.Services.Auth
{
    public class AuthService(IUserRepository userRepository) : IAuthService
    {
        public string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha256.ComputeHash(bytes);

            return Convert.ToBase64String(hash);
        }

        public bool VerifyPassword(string password, string passwordHash)
        {
            var hashOfInput = HashPassword(password);

            return hashOfInput == passwordHash;
        }

        public async Task<ServiceResult<LoginResponse>> LoginAsync(LoginRequest request)
        {
            var user = await userRepository.GetByUsernameAsync(request.Username);

            if (user is null)
                return ServiceResult<LoginResponse>.Failure("User not found", HttpStatusCode.NotFound);

            if (!VerifyPassword(request.Password, user.PasswordHash))
                return ServiceResult<LoginResponse>.Failure("Invalid credentials", HttpStatusCode.Unauthorized);

            var userDto = new UserDto(user.Id, user.Username, user.Role);
            var token = GenerateToken(userDto); //will be implemented later

            return ServiceResult<LoginResponse>.Success(new LoginResponse(token, userDto));
        }

        public string GenerateToken(UserDto user)
        {
            //placeholder for JWT token generation
            return $"fake-token-for-{user.Username}";
        }
    }
}
