using System.Web;
using System.Web.Optimization;

namespace AdminPanel
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)    
        {
            bundles.Add(new StyleBundle("~/bundles/css").Include(
                        "~/Content/css/pages/login.css",
                        "~/Content/vendors/vendors.min.css",
                        "~/Content/vendors/sweetalert/sweetalert.css",
                        "~/Content/vendors/quill/quill.snow.css",
                        "~/Content/vendors/data-tables/css/jquery.dataTables.min.css",
                        "~/Content/vendors/data-tables/extensions/responsive/css/responsive.dataTables.min.css",
                        "~/Content/vendors/data-tables/css/select.dataTables.min.css",
                        "~/Content/vendors/select2/select2.min.css",
                        "~/Content/vendors/select2/select2-materialize.css",
                        "~/Content/css/themes/vertical-modern-menu-template/materialize.min.css",
                        "~/Content/css/themes/vertical-modern-menu-template/style.min.css",
                        "~/Content/css/pages/eCommerce-products-page.min.css",
                        "~/Content/css/pages/form-select2.min.css",
                        "~/Content/css/pages/page-users.min.css",
                        "~/Content/css/pages/app-chat.min.css",
                        "~/Content/css/pages/dashboard.min.css",
                        "~/Content/css/pages/app-email.min.css",
                        "~/Content/css/pages/data-tables.min.css",
                        "~/Content/css/custom/custom.css"
            ));
        }
    }
}
