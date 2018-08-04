using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System;
using $safeprojectname$.Models;
using System.Web;


namespace $safeprojectname$.Infrastructure
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        // Additional properties here
        [Index]
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string AboutMe { get; set; }
        public int? PictureId { get; set; }

        [ForeignKey("PictureId")]
        public virtual Image Image { get; set; }
        //public virtual ICollection<Image> Images { get; set; }

        [InverseProperty("Participants")]
        public virtual ICollection<Event> ParticipantIn { get; set; }
        [InverseProperty("Pending")]
        public virtual ICollection<Event> PendingFor { get; set; }

        [InverseProperty("Creator")]
        public virtual ICollection<Event> CreatorOf { get; set; }

        public virtual ICollection<Comment> Comments { get; set; }



        // authenticationType param added to default implementation
        // Helper method responsible for getting the authenticated users identity (All roles and claims mapped to the user)
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager, string authenticationType)
        {
            var userIdentity = await manager.CreateIdentityAsync(this, authenticationType);
            // Add custom user claims here
            return userIdentity;
        }
    }
}
