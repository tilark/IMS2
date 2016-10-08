using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS2.DAL.Interface
{
    public interface IUnitOfWork : IDisposable
    {
        void Save();
        DbContext Db { get; }
        void StartTransaction();
    }
}
