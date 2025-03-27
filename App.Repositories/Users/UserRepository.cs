using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata.Ecma335;

namespace App.Repositories.Users
{
    public class UserRepository(AppDbContext context) : IUserRepository
    {
        private readonly AppDbContext _context = context;
        private readonly DbSet<User> _dbSet = context.Set<User>();
        public async Task<User?> GetByUsernameAsync(string username) => await _dbSet.FirstOrDefaultAsync(u => u.Username == username);
        public async Task CreateAsync(User user) => await _dbSet.AddAsync(user);
        public void Update(User user) => _dbSet.Update(user);
        public async Task<User?> GetByRefreshTokenAsync(string refreshToken)
        {
            return await _dbSet.FirstOrDefaultAsync(u => 
                u.RefreshToken == refreshToken &&
                u.RefreshTokenExpiresAt > DateTime.UtcNow);
        }
    }
}
