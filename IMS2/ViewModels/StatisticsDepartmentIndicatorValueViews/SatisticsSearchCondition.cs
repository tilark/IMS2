using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;

namespace IMS2.ViewModels.StatisticsDepartmentIndicatorValueViews
{
    public class SatisticsSearchCondition
    {
        [Required]
        [Display(Name = "时间")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM}", ApplyFormatInEditMode = true)]
        public DateTime SearchTime { get; set; }

        [Display(Name = "科室")]
        public Guid? DepartmentID { get; set; }

        [Display(Name = "跨度")]
        public Guid? DurationID { get; set; }

        [Display(Name ="指标")]
        public Guid? IndicatorID { get; set; }


        #region publicMethod
        public Expression<Func<T, bool>> CreateSearchSatisticsExpressionTree<T>()
        {
            Expression<Func<T, bool>> expressionDeviceSearch = null;

            ParameterExpression satisticParam = Expression.Parameter(typeof(T), "satistic");
            Expression finalSearch = Expression.Constant(true);

            #region 科室
            if (DepartmentID.HasValue)
            {
                var exDepartmentID = Expression.Property(satisticParam, "DepartmentID");
                var searchDepartmentID = Expression.Convert(Expression.Constant(DepartmentID.Value), exDepartmentID.Type);

                var equalDepartmentID = Expression.Equal(exDepartmentID, searchDepartmentID);
                finalSearch = Expression.AndAlso(finalSearch, equalDepartmentID);
            }
            #endregion

            #region 指标
            if (IndicatorID.HasValue)
            {
                var exIndicatorID = Expression.Property(satisticParam, "IndicatorID");
                var searchIndicatorID = Expression.Convert(Expression.Constant(IndicatorID.Value), exIndicatorID.Type);

                var equalIndicatorID = Expression.Equal(exIndicatorID, searchIndicatorID);
                finalSearch = Expression.AndAlso(finalSearch, equalIndicatorID);
            }
            #endregion

            #region 跨度
            if (DurationID.HasValue)
            {
                var exDurationID = Expression.Property(satisticParam, "DurationID");
                var searchDurationID = Expression.Convert(Expression.Constant(DurationID.Value), exDurationID.Type);

                var equalDurationID = Expression.Equal(exDurationID, searchDurationID);
                finalSearch = Expression.AndAlso(finalSearch, equalDurationID);
            }
            #endregion

            #region 时间
            var time = new DateTime(SearchTime.Year, SearchTime.Month, 1);
            var exsearchTime = Expression.Property(satisticParam, "Time");
            var searchTime = Expression.Convert(Expression.Constant(time), exsearchTime.Type);
            var equalTime = Expression.Equal(exsearchTime, searchTime);
            finalSearch = Expression.AndAlso(finalSearch, equalTime);
            #endregion

            if (finalSearch != null)
            {
                expressionDeviceSearch = Expression.Lambda<Func<T, bool>>(finalSearch, satisticParam);
            }
            return expressionDeviceSearch;
        }
        #endregion
    }
}