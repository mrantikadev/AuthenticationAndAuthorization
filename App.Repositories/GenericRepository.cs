using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace App.Repositories
{
    public class GenericRepository<T>(AppDbContext context) : IGenericRepository<T> where T : class
    {
        private readonly DbSet<T> _dbSet = context.Set<T>();

        public IQueryable<T> GetAll() => _dbSet.AsQueryable().AsNoTracking();
        public async ValueTask<T?> GetByIdAsync(int id) => await _dbSet.FindAsync(id);
        public IQueryable<T> Get(Expression<Func<T, bool>> predicate) => _dbSet.Where(predicate).AsQueryable().AsNoTracking();
        public async ValueTask CreateAsync(T entity) => await _dbSet.AddAsync(entity);
        public void Update(T entity) => _dbSet.Update(entity);
        public void Delete(T entity) => _dbSet.Remove(entity);
    }
}
