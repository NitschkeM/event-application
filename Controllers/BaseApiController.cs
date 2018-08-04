using System.Web;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using $safeprojectname$.Infrastructure;
using $safeprojectname$.Models;
using System.Net.Http;

namespace $safeprojectname$.Controllers
{
    public class BaseApiController : ApiController
    {
        // *****Should I have DbContext here aswell?*****
        // *****Or: Using {DbContext} in QueryObject pattern?*****

        // Constructor of BaseApiController
        public BaseApiController() { }

        private ModelFactory _modelFactory;
        private ApplicationUserManager _AppUserManager = null;
        private ApplicationRoleManager _AppRoleManager = null;

        // Getter: ApplicationUserManager
        protected ApplicationUserManager AppUserManager
        {
            get
            {
                return _AppUserManager ?? Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
        }
        // Getter: ApplicationRoleManger
        protected ApplicationRoleManager AppRoleManager
        {
            get
            {
                // Note: Uses GetUserManager, not GetRoleManager.
                return _AppRoleManager ?? Request.GetOwinContext().GetUserManager<ApplicationRoleManager>();
            }
        }
        // Getter: ModelFactory
        protected ModelFactory TheModelFactory
        {
            get
            {
                if (_modelFactory == null)
                {
                    _modelFactory = new ModelFactory(this.Request, this.AppUserManager);
                }
                return _modelFactory;
            }
        }

        protected IHttpActionResult GetErrorResult(IdentityResult result)
        {
            if (result == null)
            {
                return InternalServerError();
            }

            if (!result.Succeeded)
            {
                // If the IdentityResult contains errors
                if (result.Errors != null)
                {
                    // Add all errors to ModelState.
                    foreach (string error in result.Errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                }

                // If the ModelState is valid.
                if (ModelState.IsValid)
                {
                    // No ModelState errors are available to send, so just return an empty BadRequest.
                    return BadRequest();
                }

                // Return BadRequest(ModelState) with added errors from foreach.
                return BadRequest(ModelState);
            }

            return null;
        }
    }
}
