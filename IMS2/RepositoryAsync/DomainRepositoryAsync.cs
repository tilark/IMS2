using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace IMS2.RepositoryAsync
{
    public class DomainRepositoryAsync<T> : IDomainRepositoryAsync<T> where T : class
    {
        private readonly IDomainUnitOfWork _unitOfWork;
        internal DbSet<T> dbSet;

        public DomainRepositoryAsync(IDomainUnitOfWork unitOfWork)
        {
            if (unitOfWork == null) throw new ArgumentNullException("unitOfWork");
            this._unitOfWork = unitOfWork;
            this.dbSet = _unitOfWork.Db.Set<T>();
        }
        public void Add(T entity)
        {
            dynamic obj = dbSet.Add(entity);
        }

        public void Delete(T entity)
        {
            if (_unitOfWork.Db.Entry(entity).State == EntityState.Detached)
            {
                dbSet.Attach(entity);
            }
            dynamic obj = dbSet.Remove(entity);
        }

        public async Task<List<T>> GetAllAsync(Expression<Func<T, bool>> predicate = null)
        {

            if (predicate != null)
            {
                return await this.dbSet.Where(predicate).ToListAsync();
            }

            return await this.dbSet.ToListAsync();
        }

        public async Task<bool> IsExistsAsync(Expression<Func<T, bool>> predicate = null)
        {
            var result = false;
            if (predicate == null)
            {
                return result;
            }
            var query = await this.dbSet.Where(predicate).FirstOrDefaultAsync();
            result = query == null ? false : true;
            return result;
        }
        /// <summary>
        /// 如果没有找到指定键元素，抛出异常.
        /// </summary>
        /// <param name="primaryKey">The primary key.</param>
        /// <returns>Task&lt;T&gt;.</returns>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException"></exception>
        public async Task<T> SingleAsync(object primaryKey)
        {
            T dbResult = null;
            dbResult = await dbSet.FindAsync(primaryKey);
            if (dbResult == null)
            {
                throw new KeyNotFoundException();
            }
            return dbResult;
        }

        public async Task<T> SingleOrDefaultAsync(object primaryKey)
        {
            var dbResult = await dbSet.FindAsync(primaryKey);
            return dbResult;
        }

        public void Update(T entity)
        {
            this.dbSet.Attach(entity);
            _unitOfWork.Db.Entry(entity).State = EntityState.Modified;
        }

        public IQueryable<T> GetAll(Expression<Func<T, bool>> predicate = null)
        {
            if (predicate != null)
            {
                return this.dbSet.Where(predicate);
            }

            return this.dbSet;
        }

        public T Single(object primaryKey)
        {
            T dbResult = null;
            dbResult = dbSet.Find(primaryKey);
            if (dbResult == null)
            {
                throw new KeyNotFoundException();
            }
            return dbResult;
        }

        public T SingleOrDefault(object primaryKey)
        {
            var dbResult = dbSet.Find(primaryKey);
            return dbResult;
        }

        public bool IsExist(Expression<Func<T, bool>> predicate = null)
        {
            var result = false;
            if (predicate == null)
            {
                return result;
            }
            var query = this.dbSet.Where(predicate).FirstOrDefault();
            result = query == null ? false : true;
            return result;
        }
    }
}