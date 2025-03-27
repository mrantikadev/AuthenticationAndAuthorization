using App.Services.Users.Dtos;

namespace App.Services.Auth.RefreshToken
{
    public record RefreshTokenResponse(string AccessToken, string RefreshToken, UserDto User);
}
