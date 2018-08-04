using $safeprojectname$.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace $safeprojectname$.Models
{
    public class Event
    {
        public int EventId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Gender { get; set; }
        //[Required]
        //public string ShortDescription { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        [Index]
        public DateTime EventStart { get; set; }
        //[Required]
        //public DateTime EventEnd { get; set; }
        [Required]
        [Index]
        public int AgeMin { get; set; }
        [Required]
        [Index]
        public int AgeMax { get; set; }
        [Required]
        [Index]
        public int PartMin { get; set; }
        [Required]
        [Index]
        public int PartMax { get; set; }
        //[Required]
        //[Index]
        //public bool Open { get; set; }
        [Column(TypeName = "VARCHAR")]
        [StringLength(20)]
        [Required]
        [Index]
        public string EventStatus { get; set; } // "open", "closed", or "cancelled"
        [Required]
        [Index]
        public bool ApprovalReq { get; set; }
        [Required]
        public DbGeography Coordinates { get; set; }
        //[Required]
        public int? PictureId { get; set; }

        [ForeignKey("PictureId")]
        public virtual Image Image { get; set; }
        //public virtual ICollection<Image> Images { get; set; }


        // [Required]
        public string CreatorId { get; set; }
        [ForeignKey("CreatorId")]
        public virtual ApplicationUser Creator { get; set; }

        public virtual ICollection<ApplicationUser> Participants { get; set; }
        public virtual ICollection<ApplicationUser> Pending { get; set; }

        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<Tag> Tags { get; set; }
    }
}
