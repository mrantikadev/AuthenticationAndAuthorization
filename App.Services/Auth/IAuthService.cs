using App.Services.Auth.Login;
using App.Services.Auth.RefreshToken;
using App.Services.ServiceResults;
using App.Services.Users.Dtos;

namespace App.Services.Auth
{
    public interface IAuthService
    {
        string HashPassword(string password);
        bool VerifyPassword(string password, string passwordHash);
        Task<ServiceResult<LoginResponse>> LoginAsync(LoginRequest request);
        string GenerateToken(UserDto user);
        string GenerateRefreshToken();
        Task<ServiceResult<RefreshTokenResponse>> RefreshTokenAsync(RefreshTokenRequest request);
    }
}
