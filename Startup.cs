using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;
using System.Web;

[assembly: OwinStartup(typeof($safeprojectname$.Startup))]

namespace $safeprojectname$
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
