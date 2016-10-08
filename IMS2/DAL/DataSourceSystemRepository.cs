using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using IMS2.Models;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;

namespace IMS2.DAL
{
    public class DataSourceSystemRepository : IDataSourceSystemRepository
    {
        private ImsDbContext context = null;
        public DataSourceSystemRepository(ImsDbContext context)
        {
            this.context = context;
        }
        public void AddDataSourceSystem(DataSourceSystem dataSourceSystem)
        {
            context.DataSourceSystems.Add(dataSourceSystem);
        }

        public void DeleteDataSourceSystem(DataSourceSystem dataSourceSystem)
        {
            context.DataSourceSystems.Remove(dataSourceSystem);

        }

        public List<DataSourceSystem> GetAllDataSourceSystem()
        {
            return context.DataSourceSystems.ToList();
        }

        public DataSourceSystem GetDataSourceSystemById(Guid id)
        {
            return context.DataSourceSystems.SingleOrDefault(d => d.DataSourceSystemId == id);
        }

        public void Save()
        {
            //client win
            bool saveFailed;
            do
            {
                saveFailed = false;
                try
                {
                    context.SaveChanges();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    saveFailed = true;

                    // Update original values from the database 
                    var entry = ex.Entries.Single();
                    entry.OriginalValues.SetValues(entry.GetDatabaseValues());
                }

            } while (saveFailed);


        }

        public void UpdateDataSourceSystem(DataSourceSystem dataSourceSystem)
        {
            context.Entry(dataSourceSystem).State = EntityState.Modified;
        }
    }
}