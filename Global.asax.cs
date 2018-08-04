using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;



namespace $safeprojectname$
{
    public class Global : System.Web.HttpApplication
    {

        protected void Application_Start()
        {
            // For spatial data types, see details at bottom.
            SqlServerTypes.Utilities.LoadNativeAssemblies(Server.MapPath("~/bin"));


            // **********NOTE HOW ALL THESE CONFIGURATION CLASSES MUST NOT BE LOCATED IN THE AUTOMATICALLY ASSIGNED NAMESPACE**********
            //**********BUT IN THE HIGHEST LEVEL NAMESPACE, IGNORING THE FOLDER**********

            AreaRegistration.RegisterAllAreas();
            //GlobalConfiguration.Configure(WebApiConfig.Register);

            // Don't know what this is, I'll include but comment out the class - Uncommented, I see it is included in WepApi template aswell.
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);

            RouteConfig.RegisterRoutes(RouteTable.Routes);

            // Not using bundles yet, implement correctly later.
            //BundleConfig.RegisterBundles(BundleTable.Bundles);
        }


        //  *****  ADDED GLOBAL CLASS FROM "ADD ITEM"  *****
        //protected void Session_Start(object sender, EventArgs e)
        //{

        //}

        //protected void Application_BeginRequest(object sender, EventArgs e)
        //{

        //}

        //protected void Application_AuthenticateRequest(object sender, EventArgs e)
        //{

        //}

        //protected void Application_Error(object sender, EventArgs e)
        //{

        //}

        //protected void Session_End(object sender, EventArgs e)
        //{

        //}

        //protected void Application_End(object sender, EventArgs e)
        //{

        //}
    }
}




// Action required to load native assemblies
// To deploy an application that uses spatial data types to a machine that does not have 'System CLR Types for SQL Server' installed 
// you also need to deploy the native assembly SqlServerSpatial110.dll.Both x86 (32 bit) and x64(64 bit) versions
// of this assembly have been added to your project under the SqlServerTypes\x86 and SqlServerTypes\x64 subdirectories. 
// The native assembly msvcr100.dll is also included in case the C++ runtime is not installed. 
   
   
// You need to add code to load the correct one of these assemblies at runtime (depending on the current architecture). 
   
// ASP.NET applications
// For ASP.NET applications, add the following line of code to the Application_Start method in Global.asax.cs: 
   
//     SqlServerTypes.Utilities.LoadNativeAssemblies(Server.MapPath("~/bin"));

