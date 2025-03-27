using App.Repositories.UnitOfWorks;
using App.Repositories.Users;
using App.Services.Auth.Jwt;
using App.Services.Auth.Login;
using App.Services.Auth.RefreshToken;
using App.Services.ServiceResults;
using App.Services.Users.Dtos;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace App.Services.Auth
{
    public class AuthService(IUserRepository userRepository, IUnitOfWork unitOfWork, IOptions<JwtSettings> jwtOptions) : IAuthService
    {
        private readonly JwtSettings jwtSettings = jwtOptions.Value;

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
            var accessToken = GenerateToken(userDto);
            var refreshToken = GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiresAt = DateTime.UtcNow.AddDays(7);
            userRepository.Update(user);

            await unitOfWork.SaveChangesAsync();

            var response = new LoginResponse(accessToken, refreshToken, userDto);

            return ServiceResult<LoginResponse>.Success(response);
        }

        public string GenerateToken(UserDto user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var token = new JwtSecurityToken(
                issuer: jwtSettings.Issuer,
                audience: jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(jwtSettings.ExpiresInMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            var randomBytes = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);

            return Convert.ToBase64String(randomBytes);
        }

        public async Task<ServiceResult<RefreshTokenResponse>> RefreshTokenAsync(RefreshTokenRequest request)
        {
            var user = await userRepository.GetByRefreshTokenAsync(request.RefreshToken);

            if (user is null)
                return ServiceResult<RefreshTokenResponse>.Failure("Invalid or expired refresh token", HttpStatusCode.Unauthorized);

            var userDto = new UserDto(user.Id, user.Username, user.Role);

            var newAccessToken = GenerateToken(userDto);
            var newRefreshToken = GenerateRefreshToken();

            // Token Rotation
            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiresAt = DateTime.UtcNow.AddDays(7);

            userRepository.Update(user);
            await unitOfWork.SaveChangesAsync();

            var response = new RefreshTokenResponse(newAccessToken, newRefreshToken, userDto);

            return ServiceResult<RefreshTokenResponse>.Success(response);
        }
    }
}
