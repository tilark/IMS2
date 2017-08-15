using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using IMS2.RepositoryAsync;
using IMS2.Models;

namespace IMS2.BusinessModel.DurationTime
{
    /// <summary>
    /// 时段时间组合算法处理器。
    /// </summary>
    /// <remarks>执行“时段”、“时间”的合理组合处理。</remarks>
    /// <see cref="时段时间组合算法"/>
    public class DurationTimeSolver
    {
        private RepositoryAsync.IDomainUnitOfWork unitOfWork;
        /// <summary>
        /// 初始化。
        /// </summary>
        public DurationTimeSolver(IDomainUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public DurationTimeSolver()
        {

        }




        /// <summary>
        /// 对指定的“时段ID”、“时间”进行组合，返回所有可能的“时段时间项”。
        /// </summary>
        /// <param name="durationId">时段ID。</param>
        /// <param name="time">时间。</param>
        /// <returns>指定的“时段ID”、“时间”所有可能的“时段时间项”集合。若输入的“时段ID”无效，或存在“年”、“半年”、“季”和“月”以外的“时段”，则返回null。</returns>
        /// <remarks>涉及“时段ID”硬编码内容。</remarks>
        /// <see cref="时段时间组合算法"/>
        public List<DurationTimeItem> Solve(Guid durationId, DateTime time)
        {
            var returnList = new List<DurationTimeItem>();
            var durations = new List<Duration>();
            //var db = new Models.ImsDbContext();
            //var repo = new DurationRepositoryAsync(this.unitOfWork);
            ////var duration = db.Durations.Find(durationId);
            using (var context = new ImsDbContext())
            {
                var duration = context.Durations.Find(durationId);

                if (duration == null)
                    return null;
                durations = context.Durations.Where(c => c.Level >= duration.Level).ToList();
            }
           

            DateTime tempTime;
            foreach (var itemDuration in durations)
            {
                switch (itemDuration.DurationId.ToString().ToUpper())
                {
                    //年
                    case "BA74E352-0AD5-424B-BF31-738BA5666649":
                        tempTime = new DateTime(time.Year, 1, 1);
                        break;
                    //半年
                    case "24847114-90E4-483D-B290-97781C3FA0C2":
                        if (time.Month <= 6)
                            tempTime = new DateTime(time.Year, 1, 1);
                        else
                            tempTime = new DateTime(time.Year, 7, 1);
                        break;
                    //季
                    case "BD18C4F4-6552-4986-AB4E-BA2DFFDED2B3":
                        if (time.Month <= 3)
                            tempTime = new DateTime(time.Year, 1, 1);
                        else if (time.Month <= 6)
                            tempTime = new DateTime(time.Year, 4, 1);
                        else if (time.Month <= 9)
                            tempTime = new DateTime(time.Year, 7, 1);
                        else
                            tempTime = new DateTime(time.Year, 10, 1);
                        break;
                    //月
                    case "D48AA438-AD71-4419-A2A2-A1C390F6C097":
                        tempTime = new DateTime(time.Year, time.Month, 1);
                        break;
                    //其他
                    default:
                        return null;
                }
                returnList.Add(new DurationTimeItem { DurationId = itemDuration.DurationId, Time = tempTime });
            }

            return returnList;
        }
    }
}