using IMS2.Models;
using IMS2.RepositoryAsync;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;

namespace IMS2.BusinessModel.SatisticsValueModel
{
    public class DepartmentIndicatorDurationTimeVirtualValueView
    {
        private IDomainUnitOfWork unitOfWork;
        private DepartmentIndicatorDurationVirtualValueRepositoryAsync repo;
        public DepartmentIndicatorDurationTimeVirtualValueView()
        {

        }
        public DepartmentIndicatorDurationTimeVirtualValueView(Guid indicatorId, Guid departmentId, Guid durationId, DateTime time)
        {
            this.IndicatorId = indicatorId;
            this.DepartmentId = indicatorId;
            this.DurationId = durationId;
            this.Time = time;
        }
        [Display(Name = "指标")]
        public Guid IndicatorId { get; set; }

        [Display(Name = "科室")]
        public Guid DepartmentId { get; set; }

        [Display(Name = "跨度")]
        public Guid DurationId { get; set; }

        [Display(Name = "记录时间")]
        public DateTime Time { get; set; }

        [Display(Name = "值")]
        public decimal? Value { get; set; }


        #region Public Method

        #region 更新或创建新值表
        /// <summary>
        /// 如果存在则更新虚拟值表，不存在则创建一个新的虚拟值表
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// 
        public void UpdateOrCreateIfNotExistDepartmentIndicatorDurationVirtualValue()
        {
            if (this.Value != null)
            {
                using(var context = new ImsDbContext())
                {
                    var query = context.DepartmentIndicatorDurationVirtualValues.Where(a => a.IndicatorId == IndicatorId && a.DepartmentId == DepartmentId && a.DurationId == DurationId && a.Time == Time).FirstOrDefault();
                    if (query == null)
                    {
                        //新建
                        var departmentIndicatorDurationVirtualValue = new DepartmentIndicatorDurationVirtualValue
                        {
                            DepartmentIndicatorDurationVirtualValueID = Guid.NewGuid(),
                            DepartmentId = DepartmentId,
                            IndicatorId = IndicatorId,
                            DurationId = DurationId,
                            Time = Time,
                            Value = Value,
                            CreateTime = DateTime.Now,
                            UpdateTime = System.DateTime.Now
                        };
                        context.DepartmentIndicatorDurationVirtualValues.Add(departmentIndicatorDurationVirtualValue);
                        #region Client win
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
                        #endregion
                    }
                    else
                    {
                        //更新
                        query.Value = Value;
                        query.UpdateTime = DateTime.Now;
                        context.DepartmentIndicatorDurationVirtualValues.Attach(query);
                        context.Entry(query).State = System.Data.Entity.EntityState.Modified;

                        #region Client win
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
                        #endregion
                    }
                }
               
            }
        }
        public void UpdateOrCreateIfNotExistDepartmentIndicatorDurationVirtualValue(IDomainUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
            if (this.Value != null)
            {
                this.repo = new DepartmentIndicatorDurationVirtualValueRepositoryAsync(unitOfWork);
                var query = repo.GetAll(a => a.IndicatorId == IndicatorId && a.DepartmentId == DepartmentId && a.DurationId == DurationId && a.Time == Time).FirstOrDefault();
                if (query == null)
                {
                    //新建
                    CreateDepartmentIndicatorDurationVirtualValue(IndicatorId, DepartmentId, DurationId, Time, this.Value.Value);
                }
                else
                {
                    //更新
                    UpdateDepartmentIndicatorDurationVirtualValue(query, this.Value.Value);
                }
            }
        }

        private void UpdateDepartmentIndicatorDurationVirtualValue(DepartmentIndicatorDurationVirtualValue query, decimal value)
        {
            query.Value = value;
            query.UpdateTime = DateTime.Now;
            repo.Update(query);
            try
            {
                this.unitOfWork.SaveChangesClientWin();
            }
            catch (Exception)
            {

                //throw;
            }
        }

        public void CreateDepartmentIndicatorDurationVirtualValue(Guid indicatorId, Guid departmentId, Guid durationId, DateTime time, decimal value)
        {
            var departmentIndicatorDurationVirtualValue = new DepartmentIndicatorDurationVirtualValue
            {
                DepartmentIndicatorDurationVirtualValueID = Guid.NewGuid(),
                DepartmentId = departmentId,
                IndicatorId = indicatorId,
                DurationId = durationId,
                Time = time,
                Value = value,
                CreateTime = DateTime.Now,
                UpdateTime = System.DateTime.Now
            };
            this.repo.Add(departmentIndicatorDurationVirtualValue);
            try
            {
                this.unitOfWork.SaveChangesClientWin();
            }
            catch (Exception)
            {

                //throw;
            }
        }
        #endregion

        #region 移除新值表项
        /// <summary>
        /// 删除新值表项
        /// </summary>
        /// <param name="unitOfWork"></param>
        public void RemoveDepartmentIndicatorDurationVirtualValue(IDomainUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
            this.repo = new DepartmentIndicatorDurationVirtualValueRepositoryAsync(unitOfWork);
            var query = repo.GetAll(a => a.IndicatorId == IndicatorId && a.DepartmentId == DepartmentId && a.DurationId == DurationId && a.Time == Time).FirstOrDefault();
            if(query != null)
            {
                this.repo.Delete(query);
                try
                {
                    this.unitOfWork.SaveChangesClientWin();
                }
                catch (Exception)
                {

                    throw;
                }
            }
        }


        public void RemoveDepartmentIndicatorDurationVirtualValue()
        {
            using(var context = new ImsDbContext())
            {
                var query = context.DepartmentIndicatorDurationVirtualValues.Where(a => a.IndicatorId == IndicatorId && a.DepartmentId == DepartmentId && a.DurationId == DurationId && a.Time == Time).FirstOrDefault();
                if (query != null)
                {
                    context.DepartmentIndicatorDurationVirtualValues.Remove(query);
                    #region Client win
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
                    #endregion
                }
            }
           
        }
        #endregion
        #endregion


    }
}