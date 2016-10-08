using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IMS2.Models;
using IMS2.ViewModels;
namespace IMS2.DAL
{
    public interface IDataSourceSystemRepository
    {
        List<DataSourceSystem> GetAllDataSourceSystem();
        DataSourceSystem GetDataSourceSystemById(Guid id);
        void AddDataSourceSystem(DataSourceSystem dataSourceSystem);
        void UpdateDataSourceSystem(DataSourceSystem dataSourceSystem);
        void DeleteDataSourceSystem(DataSourceSystem dataSourceSystem);
        void Save();
    }
}
