using System.Web.Mvc;
using System.Web.Routing;

namespace $safeprojectname$
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "CatchAll",
                url: "{*url}",
                defaults: new { controller = "Home", action = "Index" }
                );


            //routes.MapRoute(
            //    name: "Default",
            //    url: "{controller}/{action}/{id}",
            //    defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            //);

            //// Test - Try to get <base href="/"> to work.
            //routes.MapRoute(
            //    name: "Base",
            //    url: "",
            //    defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            //);

            //// Test - Try to get <base href="/"> to work.
            //routes.MapRoute(
            //    name: "BasePluss",
            //    url: "{something}",
            //    defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            //);

            ////Test - With this I can refresh edit/:id and details/:id views
            //routes.MapRoute(
            //    name: "DefaultExtended",
            //    url: "{controller}/{action}/{id}/{something}",
            //    defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            //);

        }
    }
}

