using $safeprojectname$.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using Owin;
using System.Web;
using $safeprojectname$.Providers;
using System.Configuration;
using Microsoft.Owin.Security.DataHandler.Encoder;
using Microsoft.Owin.Security.Jwt;
using Microsoft.Owin.Security;
using System.Web.Http;

namespace $safeprojectname$
{
    public partial class Startup
    {
        // For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
        public void ConfigureAuth(IAppBuilder app)
        {
            HttpConfiguration httpConfig = new HttpConfiguration();

            ConfigureOAuthTokenGeneration(app);

            ConfigureOAuthTokenConsuption(app);

            WebApiConfig.Register(httpConfig);

            app.UseWebApi(httpConfig);

            // Configure the DbContext, UserManager and SignInManager to use a single instance per request.
            // The DbContext and UserManager objects will be available for the entire lifetime of the request.
            //app.CreatePerOwinContext(ApplicationDbContext.Create);
            //app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);
            // app.CreatePerOwinContext<ApplicationSignInManager>(ApplicationSignInManager.Create);


            // ***** Troor det fungerer når jeg inkluderer dette fordi Identity blir inkludert somehow***** using statement blir aktivt
            // Confirmed, funka når jeg uncommented random shit der nede som også aktiverte identity
            //app.UseCookieAuthentication(new CookieAuthenticationOptions());
            //app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);
        }



        private void ConfigureOAuthTokenGeneration(IAppBuilder app)
        {
            // Configure the DbContext, UserManager and RoleManager to use a single instance per request.
            app.CreatePerOwinContext(ApplicationDbContext.Create);
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);
            app.CreatePerOwinContext<ApplicationRoleManager>(ApplicationRoleManager.Create);

            OAuthAuthorizationServerOptions OAuthServerOptions = new OAuthAuthorizationServerOptions()
            {
                //For Dev enviroment only (on production should be AllowInsecureHttp = false)
                AllowInsecureHttp = true,
                // Path for generating token: "localhost:.../oauth/token"
                TokenEndpointPath = new PathString("/oauth/token"),
                // Expiery for token: Set to 1 day.
                AccessTokenExpireTimeSpan = TimeSpan.FromDays(1),
                // Specifying that we will validate the Resource owner user credential this custom class.
                Provider = new CustomOAuthProvider(),

                // Specifying that how we will generate our JWT token
                // This class will be responsible for this instead of the default access token using DPAPI, note that both formats will use the Bearer scheme.
                // The value passed is the issuer, used in the constructor of the class generating the token. 
                AccessTokenFormat = new CustomJwtFormat("http://localhost:55560")
            };

            // OAuth 2.0 Bearer Access Token Generation
            app.UseOAuthAuthorizationServer(OAuthServerOptions);
        }


        // For this I had to install the package Microsoft.Owin.Security.Jwt, 
        // It is responisble for protecting the Resource Server Resources using JWT - it validates and de-serializes JWT tokens.
        public void ConfigureOAuthTokenConsuption(IAppBuilder app)
        {
            // Configure API to trust tokens issued by our Authorization server only.
            // (In this case Resource and Auth servers are the same: (http://localhost:55560)
            var issuer = "http://localhost:55560";

            // Same audience Id and Secret values as we used to generate and issue the JWT (In provider)
            string audienceId = ConfigurationManager.AppSettings["as:AudienceId"];
            byte[] audienceSecret = TextEncodings.Base64Url.Decode(ConfigurationManager.AppSettings["as:AudienceSecret"]);



            // Api controllers with an [Authorize] attribute will be validated with JWT
            // We provide the values above to the "JwtBearerAuthentication" middleware,
            // our API will only be able to consume JWT tokens issued by our trusted Authorization server.
            app.UseJwtBearerAuthentication(
                new JwtBearerAuthenticationOptions
                {
                    AuthenticationMode = AuthenticationMode.Active,
                    AllowedAudiences = new[] { audienceId },
                    IssuerSecurityTokenProviders = new IIssuerSecurityTokenProvider[]
                    {
                        new SymmetricKeyIssuerSecurityTokenProvider(issuer, audienceSecret)
                    }
                });
        }



    }
}











//***********************************************************
//***********FROM EVENTAPPLICATION, TEMPLATED STUFF**********
//***********************************************************
//// Enable the application to use a cookie to store information for the signed in user
//// and to use a cookie to temporarily store information about a user logging in with a third party login provider
//// Configure the sign in cookie
//app.UseCookieAuthentication(new CookieAuthenticationOptions
//{
//    AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
//    LoginPath = new PathString("/Account/Login"),
//    Provider = new CookieAuthenticationProvider
//    {
//        // Enables the application to validate the security stamp when the user logs in.
//        // This is a security feature which is used when you change a password or add an external login to your account.  
//        OnValidateIdentity = SecurityStampValidator.OnValidateIdentity<ApplicationUserManager, ApplicationUser>(
//            validateInterval: TimeSpan.FromMinutes(30),
//            regenerateIdentity: (manager, user) => user.GenerateUserIdentityAsync(manager))
//    }
//});
//app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

//// Enables the application to temporarily store user information when they are verifying the second factor in the two-factor authentication process.
//app.UseTwoFactorSignInCookie(DefaultAuthenticationTypes.TwoFactorCookie, TimeSpan.FromMinutes(5));

//// Enables the application to remember the second login verification factor such as phone or email.
//// Once you check this option, your second step of verification during the login process will be remembered on the device where you logged in from.
//// This is similar to the RememberMe option when you log in.
//app.UseTwoFactorRememberBrowserCookie(DefaultAuthenticationTypes.TwoFactorRememberBrowserCookie);

//// Uncomment the following lines to enable logging in with third party login providers
////app.UseMicrosoftAccountAuthentication(
////    clientId: "",
////    clientSecret: "");

////app.UseTwitterAuthentication(
////   consumerKey: "",
////   consumerSecret: "");

////app.UseFacebookAuthentication(
////   appId: "",
////   appSecret: "");

////app.UseGoogleAuthentication(new GoogleOAuth2AuthenticationOptions()
////{
////    ClientId = "",
////    ClientSecret = ""
////});
