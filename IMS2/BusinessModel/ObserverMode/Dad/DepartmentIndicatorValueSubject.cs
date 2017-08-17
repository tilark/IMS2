using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using IMS2.RepositoryAsync;
using IMS2.Models;
using System.Data.Entity.Infrastructure;

namespace IMS2.BusinessModel.ObserverMode.Dad
{
    /// <summary>
    /// 值表变动对象。
    /// </summary>
    /// <remarks>映射1个值表记录。封装“锁定”、“解锁”功能。作为变动对象，可触发观察者更新。</remarks>
    /// <see cref="基于值表变动动态更新虚拟值表机制"/>
    /// <example>
    /// 通过传入一个值表实例进行初始化，调用SetLocked或者SetUnlocked进行操作。
    /// <code>
    /// var s = new DepartmentIndicatorValueSubject(origin, unitOfWork);
    /// 
    /// s.SetLocked();
    /// s.SetUnlocked();
    /// </code>
    /// </example>
    public class DepartmentIndicatorValueSubject : ISubject
    {
        public DepartmentIndicatorValueSubject()
        {
            this.Observers = new List<IObserver>();
        }
        public DepartmentIndicatorValueSubject(Models.DepartmentIndicatorValue origin, RepositoryAsync.IDomainUnitOfWork unitOfWork)
        {
            this.DepartmentIndicatorValueId = origin.DepartmentIndicatorValueId;
            this.DepartmentId = origin.DepartmentId;
            this.IndicatorId = origin.IndicatorId;
            this.Time = origin.Time;
            this.IsLocked = origin.IsLocked;

            this.Observers = new List<IObserver>();

            this.unitOfWork = unitOfWork;
        }


        public DepartmentIndicatorValueSubject(RepositoryAsync.IDomainUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
            this.Observers = new List<IObserver>();
        }



        private RepositoryAsync.IDomainUnitOfWork unitOfWork;





        /// <summary>
        /// 值表ID。
        /// </summary>
        public Guid DepartmentIndicatorValueId { get; set; }

        /// <summary>
        /// 数据归属科室ID。
        /// </summary>
        public Guid DepartmentId { get; set; }

        /// <summary>
        /// 指标ID。
        /// </summary>
        public Guid IndicatorId { get; set; }

        /// <summary>
        /// 时间。
        /// </summary>
        public DateTime Time { get; set; }

        /// <summary>
        /// 是否锁定。
        /// </summary>
        public bool IsLocked { get; set; }





        /// <summary>
        /// 观察者集合。
        /// </summary>
        public List<IObserver> Observers { get; set; }





        /// <summary>
        /// 注册观察者。
        /// </summary>
        /// <param name="observer">观察者。</param>
        /// <remarks>有同一检查。</remarks>
        public void Attach(IObserver observer)
        {
            if (!this.Observers.Contains(observer))
                this.Observers.Add(observer);
        }

        /// <summary>
        /// 撤销观察者。
        /// </summary>
        /// <param name="observer">观察者。</param>
        public void Detach(IObserver observer)
        {
            this.Observers.Remove(observer);
        }

        /// <summary>
        /// 通知。
        /// </summary>
        /// <remarks>触发所有观察者。触发前给观察者设置Subject。</remarks>
        public void Notify()
        {
            foreach (var observer in this.Observers)
            {
                observer.Subject = this;
                observer.Update();
            }
        }

        /// <summary>
        /// 设置为锁定。
        /// </summary>
        /// <remarks>触发“通知”。</remarks>
        public void SetLocked()
        {
            this.IsLocked = true;

            this.UpdateDatabase();

            this.Notify();
        }

        /// <summary>
        /// 设置为未锁定。
        /// </summary>
        /// <remarks>触发“通知”。</remarks>
        public void SetUnlocked()
        {
            this.IsLocked = false;

            this.UpdateDatabase();

            this.Notify();
        }

        /// <summary>
        /// 更新数据库。
        /// </summary>
        protected void UpdateDatabase()
        {
            using (var context = new ImsDbContext())
            {
                var target = context.DepartmentIndicatorValues.Find(this.DepartmentIndicatorValueId);
                if (target != null)
                {

                    target.IsLocked = this.IsLocked;
                    //如果原值表中的Value为null，则设为false，不能审核。如果不为null，则按IsLocked操作
                    if (target.Value == null)
                    {
                        target.IsLocked = false;
                    }
                    else
                    {
                        target.IsLocked = this.IsLocked;
                    }

                    target.UpdateTime = DateTime.Now;
                    context.DepartmentIndicatorValues.Attach(target);

                    context.Entry(target).State = System.Data.Entity.EntityState.Modified;


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

            //var departmentIndicatorValueRepo = new DepartmentIndicatorValueRepositoryAsync(unitOfWork);
            //var target = departmentIndicatorValueRepo.SingleOrDefault(this.DepartmentIndicatorValueId);
            //if(target != null)
            //{
            //    target.IsLocked = this.IsLocked;
            //    target.UpdateTime = DateTime.Now;
            //    departmentIndicatorValueRepo.Update(target);
            //    unitOfWork.SaveChangesClientWin();
            //}

        }
    }
}