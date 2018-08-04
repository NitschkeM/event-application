using $safeprojectname$.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace $safeprojectname$.Models
{
    public class Comment
    {
        [Key]
        public int CommentId { get; set; }
        [Required]
        public string CommentText { get; set; }
        [Required]
        [Index]
        public DateTime PostedTime { get; set; }

        [Index]
        public string PosterId { get; set; }
        [Index]
        public int EventId { get; set; }


        [ForeignKey("PosterId")]
        public virtual ApplicationUser ApplicationUser { get; set; }
        [ForeignKey("EventId")]
        public virtual Event Event { get; set; }

        [Index]
        public int? ParentId { get; set; }

        [ForeignKey("ParentId")]
        public virtual Comment Parent { get; set; }
        [InverseProperty("Parent")]
        public virtual ICollection<Comment> Replies { get; set; }


    }
}
