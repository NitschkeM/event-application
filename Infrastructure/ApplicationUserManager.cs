using $safeprojectname$.Services;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using System;
using System.Web;

namespace $safeprojectname$.Infrastructure
{
    // Configure the application UserManager used in this application. UserManager is defined in ASP.NET Identity and is used by the application.
    public class ApplicationUserManager : UserManager<ApplicationUser>
    {
        public ApplicationUserManager(IUserStore<ApplicationUser> store)
            : base(store)
        {
        }

        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context)
        {
            var appDbContext = context.Get<ApplicationDbContext>();
            var appUserManager = new ApplicationUserManager(new UserStore<ApplicationUser>(appDbContext));

            // Configure validation logic for usernames
            appUserManager.UserValidator = new UserValidator<ApplicationUser>(appUserManager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };

            // Configure validation logic for passwords
            appUserManager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                RequireNonLetterOrDigit = true,
                RequireDigit = true,
                RequireLowercase = true,
                RequireUppercase = true,
            };

            return appUserManager;
            //// Configure user lockout defaults
            //appUserManager.UserLockoutEnabledByDefault = true;
            //appUserManager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
            //appUserManager.MaxFailedAccessAttemptsBeforeLockout = 5;

            //// Register two factor authentication providers. This application uses Phone and Emails as a step of receiving a code for verifying the user
            //// You can write your own provider and plug it in here.
            //appUserManager.RegisterTwoFactorProvider("Phone Code", new PhoneNumberTokenProvider<ApplicationUser>
            //{
            //    MessageFormat = "Your security code is {0}"
            //});
            //appUserManager.RegisterTwoFactorProvider("Email Code", new EmailTokenProvider<ApplicationUser>
            //{
            //    Subject = "Security Code",
            //    BodyFormat = "Your security code is {0}"
            //});
            //appUserManager.EmailService = new EmailService();
            //appUserManager.SmsService = new SmsService();

            //var dataProtectionProvider = options.DataProtectionProvider;
            //if (dataProtectionProvider != null)
            //{
            //    appUserManager.UserTokenProvider =
            //        new DataProtectorTokenProvider<ApplicationUser>(dataProtectionProvider.Create("ASP.NET Identity"));
            //}
        }
    }
}
