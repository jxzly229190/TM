using System.Web;
using System.Web.Optimization;

namespace TeamManager
{
    public class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            //bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
            //            "~/Scripts/jquery-ui-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.unobtrusive*",
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            //bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
            //            "~/Scripts/bootstrap.js",
            //            "~/Scripts/bootstrap-datetimepicker.js",
            //            "~/Scripts/respond.js"));

            //bundles.Add(new StyleBundle("~/Content/bootstrap").Include(
            //            "~/Content/bootstrap/css/bootstrap.css",
            //            "~/Content/bootstrap/css/bootstrap-theme.css",
            //            "~/Content/bootstrap/css/bootstrap-datetimepicker.css",
            //            "~/Content/site.css"));

            bundles.Add(new ScriptBundle("~/bundles/OverTime").Include(
                        "~/Scripts/OverTime.js"));

            bundles.Add(new StyleBundle("~/Content/OverTime").Include("~/Content/OverTime.css"));

            bundles.Add(new ScriptBundle("~/bundles/BreakOff").Include(
            "~/Scripts/BreakOff.js"));

            bundles.Add(new StyleBundle("~/Content/BreakOff").Include("~/Content/BreakOff.css"));

            bundles.Add(new ScriptBundle("~/bundles/Trainee").Include(
            "~/Scripts/Trainee.js"));

            bundles.Add(new StyleBundle("~/Content/Trainee").Include("~/Content/Trainee.css"));

            bundles.Add(new ScriptBundle("~/bundles/TraineeBreakOff").Include(
            "~/Scripts/TraineeBreakOff.js"));

            bundles.Add(new ScriptBundle("~/bundles/knockout").Include(
            "~/Scripts/knockout-{version}.js"));

            bundles.Add(new StyleBundle("~/Content/TraineeBreakOff").Include("~/Content/TraineeBreakOff.css"));
        }
    }
}