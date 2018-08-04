using Microsoft.AspNet.Identity.EntityFramework;
using $safeprojectname$.Infrastructure;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http.Routing;

namespace $safeprojectname$.Models
{
    public class ModelFactory
    {
        // There is a separate UrlHelper class for MVC, I used the Web API one.
        private UrlHelper _UrlHelper;
        private ApplicationUserManager _AppUserManager;

        public ModelFactory(HttpRequestMessage request, ApplicationUserManager appUserManager)
        {
            _UrlHelper = new UrlHelper(request);
            _AppUserManager = appUserManager;
        }

        public UserReturnModel Create(ApplicationUser appUser)
        {
            return new UserReturnModel
            {
                // Picture
                Url = _UrlHelper.Link("GetUserById", new { id = appUser.Id }),
                Id = appUser.Id,
                UserName = appUser.UserName,
                DateOfBirth = appUser.DateOfBirth,
                Gender = appUser.Gender,
                Email = appUser.Email,
                EmailConfirmed = appUser.EmailConfirmed,
                Roles = _AppUserManager.GetRolesAsync(appUser.Id).Result,
                Claims = _AppUserManager.GetClaimsAsync(appUser.Id).Result
            };
        }

        public RoleReturnModel Create(IdentityRole appRole)
        {
            return new RoleReturnModel
            {
                Url = _UrlHelper.Link("GetRoleById", new { id = appRole.Id }),
                Id = appRole.Id,
                Name = appRole.Name
            };
        }

        public CommentReturnModel CreateComment(Comment comment, string userName, bool isPoster, string imageUrl)
        {
            return new CommentReturnModel
            {
                CommentId = comment.CommentId,
                CommentText = comment.CommentText,
                PostedTime = comment.PostedTime,
                IsPoster = isPoster,
                PosterUserName = userName,
                ParentId = comment.ParentId,
                ImageUrl = imageUrl
            };
        }

        public EventListReturnModel Create(Event @event, string userRship = "")
        {
            return new EventListReturnModel
            {
                EventId = @event.EventId,
                Name = @event.Name,
                Gender = @event.Gender,
                Description = @event.Description,
                EventStart = @event.EventStart,               
                AgeMin = @event.AgeMin,
                AgeMax= @event.AgeMax,
                PartMin = @event.PartMin,
                PartMax = @event.PartMax,
                EventStatus = @event.EventStatus,
                PosLat = @event.Coordinates.Latitude.Value,
                ApprovalReq = @event.ApprovalReq,
                PosLng = @event.Coordinates.Longitude.Value,
                CreatorId = @event.CreatorId,
                ImageUrl = @event.PictureId != null ? @event.Image.ImageUrl : "/Client/assets/img/Modern_floral_background_10to4.jpg",
                currentPartic = @event.Participants.Count,
                currentPending = @event.Pending.Count,
                userRship = userRship
            };  
        }

        public TagDto CreateTag(Tag tag)
        {
            return new TagDto
            {
                Id = tag.Id,
                Name = tag.Name,
                Count = tag.Count
            };
        }
    }


    public class EventListReturnModel
    {
        public int EventId { get; set; }
        public string Name { get; set; }
        public string Gender { get; set; }
        public string Description { get; set; }
        public DateTime EventStart { get; set; }
        public int AgeMin { get; set; }
        public int AgeMax { get; set; }
        public int PartMin { get; set; }
        public int PartMax { get; set; }
        //public bool Open { get; set; }
        public string EventStatus { get; set; }
        public bool ApprovalReq { get; set; }
        public double PosLat { get; set; }
        public double PosLng { get; set; }
        public string ImageUrl { get; set; }
        public string CreatorId { get; set; }
        public string userRship { get; set; }
        public int currentPartic { get; set; }
        public int currentPending { get; set; }
    }

    public class UserReturnModel
    {
        // Picture

        public string Url { get; set; } // Url? 
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public bool EmailConfirmed { get; set; }

        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; }

        public IList<string> Roles { get; set; }
        public IList<System.Security.Claims.Claim> Claims { get; set; }
    }

    public class RoleReturnModel
    {
        public string Url { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
    }

    public class CommentReturnModel
    {
        public int CommentId { get; set; }
        public int? ParentId { get; set; }
        public string CommentText { get; set; }
        public string PosterUserName { get; set; }
        public DateTime PostedTime { get; set; }
        public bool IsPoster { get; set; }
        public string ImageUrl { get; set; }
    }

    public class TagDto
    {
            public int Id { get; set; }
            public string Name { get; set; }
            public int Count { get; set; }
    }
}
