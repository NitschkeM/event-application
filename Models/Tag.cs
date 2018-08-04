using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace $safeprojectname$.Models
{
    public class Tag
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        //[Required] ElseOn NewEvent Post: Modelstate of Tag is not valid.
        [Index]
        public int Count { get; set; }

        [InverseProperty("Tags")]
        public virtual ICollection<Event> Events { get; set; }
    }
}

// TAGS: 
// Array of values
// Multiple booleans - in event record
// Separate table: EventId, Bool sports, Bool outside, ... 
// Google Database performance
// http://tagging.pui.ch/post/37027746608/tagsystems-performance-tests 