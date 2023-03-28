using System.Web;
using System.Web.Optimization;

namespace FoodDefense
{
    public class BundleConfig
    {
        private const string fontAwesome = "~/Content/font-awesome.min.css";
        private const string autocomplete = "~/Scripts/jquery.ui.autocomplete.js";
        
        // Para obtener más información sobre las uniones, visite https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js", autocomplete));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryunobtrusive").Include(
                        "~/Scripts/jquery.unobtrusive*"));

            // Utilice la versión de desarrollo de Modernizr para desarrollar y obtener información. De este modo, estará
            // para la producción, use la herramienta de compilación disponible en https://modernizr.com para seleccionar solo las pruebas que necesite.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css",
                      "~/Content/bootstrap-datetimepicker.min.css",
                      fontAwesome));

            bundles.Add(new ScriptBundle("~/bundles/bootstrapdatepicker").Include(
                "~/Scripts/moment.min.js",
                "~/Scripts/bootstrap-datetimepicker.min.js",
                "~/Scripts/locales/bootstrap-datepicker.es.js"));

            

        }
    }
}
