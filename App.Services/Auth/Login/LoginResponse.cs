﻿using App.Services.Users.Dtos;

namespace App.Services.Auth.Login
{
    public record LoginResponse(string Token, string RefreshToken, UserDto User);
}
