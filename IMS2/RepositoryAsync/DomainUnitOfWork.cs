using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Data.Entity;
using IMS2.Models;
using System.Data.Entity.Infrastructure;

namespace IMS2.RepositoryAsync
{
    public class DomainUnitOfWork : IDomainUnitOfWork
    {
        private readonly ImsDbContext _db;
        public DomainUnitOfWork()
        {
            this._db = new ImsDbContext();
        }
        public DomainUnitOfWork(ImsDbContext db)
        {
            this._db = db;
        }
        public DbContext Db
        {
            get
            {
                return this._db;
            }
        }

        //public ImsDbContext dbContext
        //{
        //    get
        //    {
        //        return this._db;
        //    }
        //}

        public async Task SaveChangesClientWinAsync()
        {
            #region Client win async
            bool saveFailed;
            do
            {
                saveFailed = false;
                try
                {
                    await this._db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    saveFailed = true;

                    // Update original values from the database 
                    var entry = ex.Entries.Single();
                    entry.OriginalValues.SetValues(entry.GetDatabaseValues());
                }

            } while (saveFailed);
            #endregion

        }

        public void SaveChangesClientWin()
        {
            #region Client win
            bool saveFailed;
            do
            {
                saveFailed = false;
                try
                {
                    this._db.SaveChanges();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    saveFailed = true;

                    // Update original values from the database 
                    var entry = ex.Entries.Single();
                    entry.OriginalValues.SetValues(entry.GetDatabaseValues());
                }

            } while (saveFailed);
            #endregion
        }

        public void SaveChangesDataBaseWin()
        {
            #region Database win
            bool saveFailed;
            do
            {
                saveFailed = false;

                try
                {
                    this._db.SaveChanges();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    saveFailed = true;

                    // Update the values of the entity that failed to save from the store 
                    ex.Entries.Single().Reload();
                }

            } while (saveFailed);
            #endregion
        }
        public async Task SaveChangesDataBaseWinAsync()
        {
            #region Database win async
            bool saveFailed;
            do
            {
                saveFailed = false;

                try
                {
                    await this._db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    saveFailed = true;

                    // Update the values of the entity that failed to save from the store 
                    ex.Entries.Single().Reload();
                }

            } while (saveFailed);
            #endregion
        }
        #region IDisposable Support
        private bool disposedValue = false; // 要检测冗余调用

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)。
                    if (this._db != null)
                    {
                        this._db.Dispose();
                    }
                }

                // TODO: 释放未托管的资源(未托管的对象)并在以下内容中替代终结器。
                // TODO: 将大型字段设置为 null。

                disposedValue = true;
            }
        }

        // TODO: 仅当以上 Dispose(bool disposing) 拥有用于释放未托管资源的代码时才替代终结器。
        // ~UnitOfWork() {
        //   // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
        //   Dispose(false);
        // }

        // 添加此代码以正确实现可处置模式。
        public void Dispose()
        {
            // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
            Dispose(true);
            // TODO: 如果在以上内容中替代了终结器，则取消注释以下行。
            // GC.SuppressFinalize(this);
        }



        #endregion
    }
}