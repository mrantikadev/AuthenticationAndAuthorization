using App.Repositories.Users;
using App.Services.Auth;
using App.Services.Auth.Login;
using App.Services.Auth.RefreshToken;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace App.API.Controllers
{
    public class AuthController(IAuthService authService) : CustomBaseController
    {
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginRequest request) => CreateActionResult(await authService.LoginAsync(request));

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken(RefreshTokenRequest request) => CreateActionResult(await authService.RefreshTokenAsync(request));

        [HttpGet("admin-area")]
        [Authorize(Roles = RoleNames.Admin)]
        public IActionResult AdminArea()
        {
            return Ok("🟢 Admin area.");
        }
    }
}
