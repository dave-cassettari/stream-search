using System.Web.Optimization;

namespace StreamSearch.Web
{
    public class BundleConfig
    {
        public const string UrlStyles = @"~/bundles/css";
        public const string UrlScripts = @"~/bundles/js";

        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new StyleBundle(UrlStyles).Include(
                      "~/Content/Styles/Vendor/materialize-0.97.7.css",
                      "~/Content/Styles/Site.css"));

            bundles.Add(new ScriptBundle(UrlScripts).Include(
                      "~/Content/Scripts/Vendor/jquery-3.1.1.js",
                      "~/Content/Scripts/Vendor/knockout-3.4.0.js",
                      "~/Content/Scripts/Vendor/materialize-0.97.7.js",
                      "~/Content/Scripts/search.js"));
        }
    }
}
