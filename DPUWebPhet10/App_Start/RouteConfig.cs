using System.Web.Mvc;
using System.Web.Routing;

namespace DPUWebPhet10
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
            ////Custom route for Download file 
            //routes.MapPageRoute(
            // "DownloadFileRoute",                         // Route name 
            // "App_Data/document/{rptmode}/{reportname}/{*parameters}",                // URL 
            // "~/Reports/ReportView.aspx"  // File 
            // ); 


            //Custom route for Download file 
            routes.MapRoute("Download", "App_Data/documents/{file}",
                            new { controller = "Files", action = "Download", file = "" }
                        );
        }
    }
}