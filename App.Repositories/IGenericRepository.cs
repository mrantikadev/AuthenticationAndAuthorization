using System.Linq.Expressions;

namespace App.Repositories
{
    public interface IGenericRepository<T> where T : class
    {
        IQueryable<T> GetAll();
        ValueTask<T?> GetByIdAsync(int id);
        IQueryable<T> Get(Expression<Func<T, bool>> predicate);
        ValueTask CreateAsync(T entity);
        void Update(T entity);
        void Delete(T entity);
    }
}
