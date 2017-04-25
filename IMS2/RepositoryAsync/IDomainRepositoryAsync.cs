using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace IMS2.RepositoryAsync
{
    public interface IDomainRepositoryAsync<T> where T : class
    {

        //Async

        Task<List<T>> GetAllAsync(Expression<Func<T, bool>> predicate = null);

        Task<T> SingleAsync(object primaryKey);
        Task<T> SingleOrDefaultAsync(object primaryKey);
        Task<bool> IsExistsAsync(Expression<Func<T, bool>> predicate = null);

        //同步
        IQueryable<T> GetAll(Expression<Func<T, bool>> predicate = null);
        T Single(object primaryKey);
        T SingleOrDefault(object primaryKey);
        bool IsExist(Expression<Func<T, bool>> predicate = null);
        void Add(T entity);
        void Update(T entity);
        void Delete(T entity);

    }
}
