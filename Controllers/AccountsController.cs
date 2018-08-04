using $safeprojectname$.Infrastructure;
using $safeprojectname$.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web;




namespace $safeprojectname$.Controllers
{
    // BaseApiController Provides Three methods: 
    // 1. AppUserManager
    // 2. TheModelFactory
    // 3. GetErrorResult
    [RoutePrefix("api/accounts")]
    public class AccountsController : BaseApiController
    {
        // Returns all registred users in our system by calling the enumeration "Users" from the "ApplicationUserManager" class.
        // GET: api/accounts/users
        [Authorize(Roles = "Admin")]
        [Route("users")]
        public IHttpActionResult GetUsers()
        {
            return Ok(this.AppUserManager.Users.ToList().Select(u => this.TheModelFactory.Create(u)));
        }

        // Return info about the user who sent the request.
        public async Task<IHttpActionResult> GetUserInfo()
        {
            var user = await this.AppUserManager.FindByNameAsync(User.Identity.Name);

            if (user != null)
            {
                var userInfo = new ParticipantDto
                {
                    UserName = user.UserName,
                    ImageUrl = user.PictureId != null ? user.Image.ImageUrl : "/Client/assets/img/signup_male.png",
                    //ImageUrl = user.Images.Count != 0 ? user.Images.FirstOrDefault().ImageUrl : "/Client/assets/img/signup_male.png",
                    Gender = user.Gender,
                    DateOfBirth = user.DateOfBirth,
                    AboutMe = user.AboutMe
                };

                return Ok(userInfo);
            }
            return NotFound();
        }


        [Authorize(Roles = "Admin")]
        [Route("query")]
        public IHttpActionResult GetUsers(int? ageMax = null, int? ageMin = null)
        {
            DateTime maxBirth = default(DateTime);
            DateTime minBirth = default(DateTime);
            if (ageMin != null)
            {
                maxBirth = DateTime.Today.AddYears(-ageMin.Value);
            }
            if (ageMax != null)
            {
                minBirth = DateTime.Today.AddYears(-ageMax.Value);
            }

            var users = AppUserManager.Users.Where(p => (ageMin == null || p.DateOfBirth >= minBirth) && (ageMax == null || p.DateOfBirth <= maxBirth));



            return Ok(users.ToList().Select(u => this.TheModelFactory.Create(u)));
        }

        // Get single user by id.
        // GET: api/accounts/user/{id}
        [Authorize(Roles = "Admin")]
        [Route("user/{id:guid}", Name = "GetUserById")]
        public async Task<IHttpActionResult> GetUser(string Id)
        {
            var user = await this.AppUserManager.FindByIdAsync(Id);

            if (user != null)
            {
                return Ok(this.TheModelFactory.Create(user));
            }

            return NotFound();
        }


        // Get single user by username
        // GET: api/accounts/user/jondoe
        [Authorize(Roles = "Admin")]
        [Route("user/{username}")]
        public async Task<IHttpActionResult> GetUserByName(string username)
        {
            var user = await this.AppUserManager.FindByNameAsync(username);

            if (user != null)
            {
                return Ok(this.TheModelFactory.Create(user));
            }

            return NotFound();
        }

        // How does it know this is POST? (Complex param?)
        // How can it differentiate from PUT? (No Id?)
        // POST: api/accounts/register
        [AllowAnonymous]
        [Route("register")]
        public async Task<IHttpActionResult> Register(CreateUserBindingModel createUserModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = new ApplicationUser()
            {
                UserName = createUserModel.Username,
                Email = createUserModel.Email,
                Gender = createUserModel.Gender,
                DateOfBirth = createUserModel.DateOfBirth
            };

            IdentityResult addUserResult = await this.AppUserManager.CreateAsync(user, createUserModel.Password);

            if (!addUserResult.Succeeded)
            {
                return GetErrorResult(addUserResult);
            }

            Uri locationHeader = new Uri(Url.Link("GetUserById", new { id = user.Id }));

            return Created(locationHeader, TheModelFactory.Create(user));
        }

        // POST: api/accounts/changepassword
        [Authorize]
        [Route("ChangePassword")]
        public async Task<IHttpActionResult> ChangePassword(ChangePasswordBindingModel model)
        {
            // Not a valid model? 
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Change password and return result 
            IdentityResult result = await this.AppUserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);

            // Not changed successfully? 
            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            // Change was successful
            return Ok();
        }

        // PUT: api/accounts/editAboutMe/{string}
        [Authorize]
        [Route("editAboutMe/{aboutMe}")]
        [HttpPut]
        public async Task<IHttpActionResult> editAboutMe(string aboutMe)
        {


            var user = AppUserManager.FindById(User.Identity.GetUserId());
            user.AboutMe = aboutMe;
            IdentityResult result = await AppUserManager.UpdateAsync(user);
            


            // Not changed successfully? 
            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            // Change was successful
            return Ok();
        }


        // DELETE: api/accounts/user/{id}
        [Authorize(Roles = "Admin")]
        [Route("user/{id:guid}")]
        public async Task<IHttpActionResult> DeleteUser(string id)
        {
            // Only SuperAdmin or Admin can delete users (Later when roles are implemented)

            var appUser = await this.AppUserManager.FindByIdAsync(id);

            // User found? 
            if (appUser != null)
            {
                // Delete user and return result
                IdentityResult result = await this.AppUserManager.DeleteAsync(appUser);

                // Not deleted successfully? 
                if (!result.Succeeded)
                {
                    return GetErrorResult(result);
                }

                // Delete was successful
                return Ok();
            }

            // User not found
            return NotFound();
        }

        // PUT: api/accounts/user/{id} + [FromBody]-array
        [Authorize(Roles = "Admin")]
        [Route("user/{id:guid}/roles")]
        [HttpPut]
        public async Task<IHttpActionResult> AssignRolesToUser([FromUri] string id, [FromBody] string[] rolesToAssign)
        {
            // Allow Admins to mange the roles for a selected user.
            // Accepts UserId and an array of the roles this UserId should be enrolled in.

            var appUser = await this.AppUserManager.FindByIdAsync(id);

            if (appUser == null)
            {
                return NotFound();
            }

            // Users current roles
            var currentRoles = await this.AppUserManager.GetRolesAsync(appUser.Id);

            // rolesToAssign(-)existingRoles.      (-) = setDifference  
            var rolesNotExists = rolesToAssign.Except(this.AppRoleManager.Roles.Select(x => x.Name)).ToArray();

            // Role does not exist
            if (rolesNotExists.Count() > 0)
            {

                ModelState.AddModelError("", string.Format("Roles '{0}' does not exixts in the system", string.Join(",", rolesNotExists)));
                return BadRequest(ModelState);
            }

            // Remove user from all roles
            IdentityResult removeResult = await this.AppUserManager.RemoveFromRolesAsync(appUser.Id, currentRoles.ToArray());

            if (!removeResult.Succeeded)
            {
                ModelState.AddModelError("", "Failed to remove user roles");
                return BadRequest(ModelState);
            }

            // Add user to rolesToAssign
            IdentityResult addResult = await this.AppUserManager.AddToRolesAsync(appUser.Id, rolesToAssign);

            if (!addResult.Succeeded)
            {
                ModelState.AddModelError("", "Failed to add user roles");
                return BadRequest(ModelState);
            }

            return Ok();
        }




    }
}
