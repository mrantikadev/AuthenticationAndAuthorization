using App.Repositories.UnitOfWorks;
using App.Repositories.Users;

namespace App.Services.Users
{
    public class UserService(IUserRepository userRepository, IUnitOfWork unitOfWork) : IUserService
    {

    }
}
