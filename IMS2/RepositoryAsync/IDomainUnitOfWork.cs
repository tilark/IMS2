using IMS2.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS2.RepositoryAsync
{
    public interface IDomainUnitOfWork : IDisposable
    {
        DbContext Db { get; }
        //ImsDbContext dbContext { get; }
        Task SaveChangesClientWinAsync();
        Task SaveChangesDataBaseWinAsync();

        void SaveChangesClientWin();
        void SaveChangesDataBaseWin();
    }
}
