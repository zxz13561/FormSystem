using System.Web;
using System.Web.Optimization;

namespace FormSystem
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/bootstrap-5.1.3/js/bootstrap.bundles.min.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/bootstrap-5.1.3/css/bootstrap.min.css"));
        }
    }
}
