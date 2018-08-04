using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace $safeprojectname$.Models
{
    public class EventDto
    {

        public int? EventId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Gender { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public DateTime EventStart { get; set; }
        [Required]
        public int AgeMin { get; set; }
        [Required]
        public int AgeMax { get; set; }
        [Required]
        public int PartMin { get; set; }
        [Required]
        public int PartMax { get; set; }
        [Required]
        public double PosLat { get; set; }
        [Required]
        public double PosLng { get; set; }
        [Required]
        public int PictureId { get; set; }
        [Required]
        public bool ApprovalReq { get; set; }
        //[Required]
        //public bool Open { get; set; }
        [Required]
        public string EventStatus { get; set; }

        public string CreatorId { get; set; }

        public List<TagDto> Tags { get; set; }
        //public List<ParticipantDto> Participants { get; set; }
        //public List<CommentDto> Comments { get; set; }

        //public int NumberOfParticipants { get; set; }
        //public bool IsParticipant { get; set; }
        //public bool IsCreator { get; set; }

    }

    public class ParticipantDto
    {
        public string UserName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string ParticipantId { get; set; }
        public string Gender { get; set; }
        public string ImageUrl { get; set; }
        public string AboutMe { get; set; }
    }

    public class CommentDto
    {
        public int CommentId { get; set; }
        // Required?
        public string CommentText { get; set; }
        public string PosterUserName { get; set; }
        public DateTime PostedTime { get; set; }
        public bool IsPoster { get; set; }
        public List<CommentDto> Replies { get; set; }
        public int? ParentId { get; set; }
        public string ImageUrl { get; set; }
    }
}
