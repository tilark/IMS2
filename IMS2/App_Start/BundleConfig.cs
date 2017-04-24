using System.Web;
using System.Web.Optimization;

namespace IMS2
{
    public class BundleConfig
    {
        // 有关绑定的详细信息，请访问 http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // 使用要用于开发和学习的 Modernizr 的开发版本。然后，当你做好
            // 生产准备时，请使用 http://modernizr.com 上的生成工具来仅选择所需的测试。
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                     "~/Content/bootstrap.css",
                     "~/Content/site.css"));
            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));
            //site
            bundles.Add(new ScriptBundle("~/bundles/site").Include(
                   "~/Scripts/site.js"));
            //ajax
            bundles.Add(new ScriptBundle("~/bundles/jqueryajax").Include(
                    "~/Scripts/jquery.unobtrusive-ajax.min.js"));

            //datetimepicker
            bundles.Add(new ScriptBundle("~/bundles/datetimepicker").Include(
                   "~/Scripts/moment-with-locales.min.js",

                   "~/Scripts/bootstrap-datetimepicker.min.js"
                ));
            bundles.Add(new StyleBundle("~/Content/datetimepicker").Include("~/Content/bootstrap-datetimepicker.min.css"));

            //select2
            bundles.Add(new StyleBundle("~/Content/select2").Include("~/Content/select2.min.css"));
            bundles.Add(new ScriptBundle("~/bundles/select2").Include(
                       "~/Scripts/select2.min.js",
                       "~/Scripts/i18n/zh-CN.js"));
            //sweetAlert
            bundles.Add(new StyleBundle("~/Content/sweetAlert").Include(
               "~/Content/sweetalert.css"
              ));
            bundles.Add(new ScriptBundle("~/bundles/sweetAlert").Include(
                   "~/Scripts/sweetalert.min.js"
                   ));

            //datatable
            bundles.Add(new StyleBundle("~/Content/datatable").Include(
              "~/Content/dataTables.bootstrap.min.css"
             ));
            bundles.Add(new ScriptBundle("~/bundles/datatable").Include(
                 "~/Scripts/jquery.dataTables.min.js",
                   "~/Scripts/dataTables.bootstrap.min.js"
                   ));
        }
    }
}
