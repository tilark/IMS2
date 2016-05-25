using IMS2.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace IMS2
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            InitialItemData initialItemData = new InitialItemData();
            initialItemData.InitialBaseData();

            RoleAction roleAction = new RoleAction();
            roleAction.InitialRoleName();
            roleAction.CreateAdmin();
            //initialItemData.InitialDepartmentCategory();
            //initialItemData.InitialDataSourceSystem();
            //initialItemData.InitialDuration();
            //initialItemData.InitialDepartment();

            //initialItemData.InitialIndicator();
            //initialItemData.InitialIndicatorGroup();
            //initialItemData.InitialIndicatorGroupMapIndicator();

            //initialItemData.InitialDepartmentCategoryMapIndicatorGroup();
            //initialItemData.InitialIndicatorAlgorithm();

            //initialItemData.InitialUserDepartment();
        }
    }
}
