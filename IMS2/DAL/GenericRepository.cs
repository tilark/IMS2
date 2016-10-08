using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using IMS2.Models;
using System.Data.Entity.Core.Objects;
using System.Data.Entity;
using IMS2.DAL.Interface;
namespace IMS2.DAL
{
    public class GenericRepository<T> : IRepository<T> where T : class
    {
        //private readonly IUnitOfWork _unitOfWork;
        private ImsDbContext context = null;
        internal DbSet<T> dbSet;
        public GenericRepository(ImsDbContext context)
        {
            this.context = context;
            DbContext db = (DbContext)context;
            this.dbSet = db.Set<T>();
        }

        public IEnumerable<T> GetAll(Func<T, bool> predicate = null)
        {
            if (predicate != null)
            {
                return dbSet.Where(predicate);
            }
            return dbSet.AsEnumerable();
        }

        public T Get(Func<T, bool> predicate)
        {
            return dbSet.FirstOrDefault(predicate);
        }

        public void Add(T entity)
        {
            dbSet.Add(entity);
        }

        public void Update(T entity)
        {
            dbSet.Attach(entity);
            context.Entry(entity).State = EntityState.Modified;
        }

        public void Delete(T entity)
        {
            dbSet.Remove(entity);
        }


        public bool Exists(object primaryKey)
        {
            return dbSet.Find(primaryKey) == null ? false : true;
        }
    }
}