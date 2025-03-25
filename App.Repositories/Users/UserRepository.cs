using Microsoft.EntityFrameworkCore;

namespace App.Repositories.Users
{
    public class UserRepository(AppDbContext context) : IUserRepository
    {
        private readonly AppDbContext _context = context;
        private readonly DbSet<User> _dbSet = context.Set<User>();
        public async Task<User?> GetByUsernameAsync(string username) => await _dbSet.FirstOrDefaultAsync(u => u.Username == username);
        public async Task CreateAsync(User user) => await _dbSet.AddAsync(user);
        public void Update(User user) => _dbSet.Update(user);
    }
}
