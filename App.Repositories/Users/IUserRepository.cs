namespace App.Repositories.Users
{
    public interface IUserRepository
    {
        Task<User?> GetByUsernameAsync(string username);
        Task CreateAsync(User user);
        void Update(User user);
        //Task SetUserRoleAsync(int userId, string role);
    }
}
