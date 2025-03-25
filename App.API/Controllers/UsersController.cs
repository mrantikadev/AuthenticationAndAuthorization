using App.Services.Users;
using App.Services.Users.Create;
using Microsoft.AspNetCore.Mvc;

namespace App.API.Controllers
{
    public class UsersController(IUserService userService) : CustomBaseController
    {
        [HttpGet("{username:string}")]
        public async Task<IActionResult> GetById(string username) => CreateActionResult(await userService.GetByUsernameAsync(username));

        [HttpPost]
        public async Task<IActionResult> Create(CreateUserRequest request) => CreateActionResult(await userService.CreateAsync(request));
    }
}
