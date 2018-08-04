using $safeprojectname$.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace $safeprojectname$.Models
{
    public class Image
    {
        public int Id { get; set; }
        // Why are both below [Required] allowed to be null in db?
        [Required(AllowEmptyStrings = false)]
        public string Name { get; set; }
        [Required(AllowEmptyStrings =false)]
        public string ImageUrl { get; set; }
        public long SizeInBytes { get; set; }
        public long SizeInKb { get; set; }
        public bool IsDefault { get; set; }

        // Do I really need these? Does not the relationship already exist? 
        // And these are only for: Find all users with this image? 
        // ** Might be good for clean-up**
        // If Image.Events.Count && Image.Users.Count Images.Remove(Image)
        public virtual ICollection<Event> Events { get; set; }
        public virtual ICollection<ApplicationUser> Users { get; set; }
    }
}
