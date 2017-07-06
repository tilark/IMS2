using IMS2.RepositoryAsync;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace IMS2.BusinessModel.IndicatorDepartmentModel
{
    public class IndicatorDepartmentImpl : IIndicatorDepartment
    {
        private IDomainUnitOfWork unitOfWork;
        public IndicatorDepartmentImpl(IDomainUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }
        public async Task<List<IndicatorDepartment>> GetAlgorithmIndicatorDepartment()
        {
            //先取出算法表中ResultID的所有的Indicator
            List<Guid> resultIndicatorIDList = await GetResultIndicatorList();
            //根据每个IndicatorID找到相对应的Department，组合成列表
            List<IndicatorDepartment> indicatorDepartmentList = await GetIndicatorDepartmentList(resultIndicatorIDList);

            return indicatorDepartmentList;
        }

        private async Task<List<IndicatorDepartment>> GetIndicatorDepartmentList(List<Guid> resultIndicatorIDList)
        {
            var result = new List<IndicatorDepartment>();
            var indicatorRepo = new IndicatorRepositoryAsync(this.unitOfWork);
            //获得 IndicatorID与List<DepartmentID>
            foreach(var indicatorID in resultIndicatorIDList)
            {
                var temp = new IndicatorDepartment();
                temp.IndicatorID = indicatorID;
                var departmentList = await indicatorRepo.GetAll(a => a.IndicatorId == indicatorID).SelectMany(a => a.IndicatorGroupMapIndicators).Select(a => a.IndicatorGroup).SelectMany(a => a.DepartmentCategoryMapIndicatorGroups).Select(a => a.DepartmentCategory).SelectMany(a => a.Departments).Select(a => a.DepartmentId).ToListAsync();
                temp.DepartmentIDList = departmentList;
                result.Add(temp);
            }
            return result;
        }

        private async Task<List<Guid>> GetResultIndicatorList()
        {
            var algorithmRepo = new IndicatorAlgorithmRepositoryAsync(this.unitOfWork);
            return await algorithmRepo.GetAll().Select(a => a.ResultId).ToListAsync();
        }
    }
}