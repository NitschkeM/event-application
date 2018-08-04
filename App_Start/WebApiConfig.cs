using Newtonsoft.Json.Serialization;
using System.Linq;
using System.Net.Http.Formatting;
using System.Web.Http;
using Microsoft.Owin.Security.OAuth;
using System.Web;
using System.Web.Http.Routing;
using System.Net.Http;

namespace $safeprojectname$
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Return camelCased objects: Fixing the conflict where .NET objects are PascalCased and javaScript objects are camelCased by convention.
            var jsonFormatter = config.Formatters.OfType<JsonMediaTypeFormatter>().First();
            jsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();

            // Web API routes
            config.MapHttpAttributeRoutes();

            // Default: api/controller/id
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            // POST: Blob Upload - User
            config.Routes.MapHttpRoute(
                "PostBlobUpload",
                "blobs/upload", // Does this url mean there is no api/*/* ? 
                new { controller = "Blobs", action = "PostBlobUpload" },
                new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) } // new { httpMethod = new HttpMethodConstraint("POST") } (+ Added using System.Net.Http; )
            );

            // POST: Blob Upload - A Default image
            config.Routes.MapHttpRoute(
                "PostUploadDefault",
                "blobs/uploaddefault", // Does this url mean there is no api/*/* ? 
                new { controller = "Blobs", action = "PostUploadDefault" },
                new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) } // new { httpMethod = new HttpMethodConstraint("POST") } (+ Added using System.Net.Http; )
            );

            // GET: Blob Download
            config.Routes.MapHttpRoute(
                "GetBlobDownload",
                "blobs/{blobId}/download",
                new { controller = "Blobs", action = "GetBlobDownload" },
                new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) } // new { httpMethod = new HttpMethodConstraint("GET") }
            );

        }
    }
}
