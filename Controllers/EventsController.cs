using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using $safeprojectname$.Models;
using Microsoft.AspNet.Identity;
using $safeprojectname$.Infrastructure;
using System.Collections.ObjectModel;
using System.Data.Entity.Spatial;
using System.Globalization;
using System.Linq.Dynamic;

namespace $safeprojectname$.Controllers
{


    // This controller may not be utilizing BaseApiCtrl properly.
    [Authorize]
    public class EventsController : BaseApiController
    {
        //System.Diagnostics.Debug.WriteLine("******BEFORE RETURN: GetEvents()*******");
        // ****************************************************************************************************************
        // ****************************************************************************************************************
        // *****Should probably make QueryObject and use "Using" statements********
        private ApplicationDbContext db = new ApplicationDbContext();
        // Currently, getting event from db(above) then getting user from AppUserManager(below)
        // Leads to bad behaviour: Event and User will be from different contexts, and their Rships cannot be modified.
        //var user = this.AppUserManager.FindById(User.Identity.GetUserId());
        // ****************************************************************************************************************
        // ****************************************************************************************************************


        //***********REMOVE Dto, then redefine dto class (?)***********
        // GET: api/Events/5
        // Get Single Event
        [Authorize]
        [Route("api/event/{id}")]
        public IHttpActionResult GetEvent(int id)
        {
            // Find event. Check if Null 
            Event e = db.Events.Find(id);
            if (e == null)
            {
                return NotFound();
            }

            var Event = new
            {
                EventId = e.EventId,
                Name = e.Name,
                Description = e.Description,
                AgeMin = e.AgeMin,
                AgeMax = e.AgeMax,
                EventStart = e.EventStart,
                Gender = e.Gender,
                PartMin = e.PartMin,
                PartMax = e.PartMax,
                PosLat = e.Coordinates.Latitude.Value,
                PosLng = e.Coordinates.Longitude.Value,
                NumberOfParticipants = e.Participants.Count(),
                EventStatus = e.EventStatus,
                ApprovalReq = e.ApprovalReq,
                ImageUrl = e.PictureId != null ? e.Image.ImageUrl : "/Client/assets/img/Modern_floral_background_10to4.jpg",
                PictureId = e.PictureId,
                Tags = e.Tags.ToList().Select(t => this.TheModelFactory.CreateTag(t)),

                //EventEnd = e.EventEnd,
                //ShortDescription = e.ShortDescription,

                CreatorId = e.CreatorId,
                // Added ToList(), else: "cannot implicitly convert Enumerable to List"
                Participants = (from p in e.Participants
                                select new ParticipantDto
                                {
                                    ParticipantId = p.Id,
                                    DateOfBirth = p.DateOfBirth,
                                    Gender = p.Gender,
                                    UserName = p.UserName,
                                    ImageUrl = p.PictureId != null ? p.Image.ImageUrl : "/Client/assets/img/signup_male.png",
                                    AboutMe = p.AboutMe
                                    //ImageUrl = p.Images.Count != 0 ? p.Images.FirstOrDefault().ImageUrl : "/Client/assets/img/signup_male.png"
                                }).ToList(),
                Pending = (from p in e.Pending
                           select new ParticipantDto
                           {
                               ParticipantId = p.Id,
                               DateOfBirth = p.DateOfBirth,
                               Gender = p.Gender,
                               UserName = p.UserName,
                               ImageUrl = p.PictureId != null ? p.Image.ImageUrl : "/Client/assets/img/signup_male.png",
                               AboutMe = p.AboutMe
                               //ImageUrl = p.Images.Count != 0 ? p.Images.FirstOrDefault().ImageUrl : "/Client/assets/img/signup_male.png"
                           }).ToList(),

                Comments = (from c in e.Comments
                            where c.ParentId == null
                            orderby c.PostedTime
                            select new CommentDto
                            {
                                CommentId = c.CommentId,
                                PosterUserName = c.ApplicationUser.UserName,
                                ImageUrl = c.ApplicationUser.PictureId != null ? c.ApplicationUser.Image.ImageUrl : "/Client/assets/img/signup_male.png",
                                CommentText = c.CommentText,
                                PostedTime = c.PostedTime,
                                IsPoster = false,
                                ParentId = c.ParentId,
                                Replies = (from p in c.Replies
                                               //where p.ParentId == c.CommentId
                                           select new CommentDto
                                           {
                                               CommentId = p.CommentId,
                                               PosterUserName = p.ApplicationUser.UserName,
                                               ImageUrl = p.ApplicationUser.PictureId != null ? p.ApplicationUser.Image.ImageUrl : "/Client/assets/img/signup_male.png",
                                               CommentText = p.CommentText,
                                               PostedTime = p.PostedTime,
                                               IsPoster = false,
                                               ParentId = p.ParentId
                                           }).ToList()
                            }).ToList()
            };

            bool isAdmin = User.IsInRole("Admin");
            // If not creator, participant or pending: userRship = none
            string userRship = "none";

            // If creator
            if (isAdmin || Event.CreatorId == User.Identity.GetUserId())
            {
                userRship = "creator";
            }

            // If not creator, check if participant or pending
            else
            {
                foreach (ParticipantDto participant in Event.Participants)
                {
                    if (participant.ParticipantId == User.Identity.GetUserId())
                    {
                        userRship = "participant";
                    }
                }
                if (userRship == "none")
                {
                    foreach (ParticipantDto pending in Event.Pending)
                    {
                        if (pending.ParticipantId == User.Identity.GetUserId())
                        {
                            userRship = "pending";
                        }
                    }
                }

            }

            // If user is poster of comment: IsPoster = true
            foreach (CommentDto comment in Event.Comments)
            {
                if (isAdmin || comment.PosterUserName == User.Identity.GetUserName())
                {
                    comment.IsPoster = true;
                }
                foreach (CommentDto reply in comment.Replies)
                {
                    if (isAdmin || reply.PosterUserName == User.Identity.GetUserName())
                    {
                        reply.IsPoster = true;
                    }
                }
            }

            var json = new
            {
                Event = Event,
                userRship = userRship
            };
            return Ok(json);
        }



        // Query Events
        [AllowAnonymous]
        public IHttpActionResult GetEvents(
             //Tags
             //[FromUri] List<TagDto> tags = null,
             [FromUri] string[] tags = null,
            // Paging parameters
            int page = 1,
            int itemsPerPage = 10,
            string sortBy = "eventStart",
            bool reverse = false,
            // Search Parameters
            bool eligibleOnly = false,
            string searchText = null,
            string gender = null,
            int? ageMin = null,
            int? ageMax = null,
            int? cPartMin = null,
            int? cPartMax = null,
            //int? PartMin = null,
            //int? PartMax = null,
            double? posLat = null,
            double? posLng = null,
            double radius = 10000,
            bool? approvalReq = null,
            // Date-Time Parameters
            DateTime? dateAfter = null,
            DateTime? dateBefore = null,
            DateTime? timeAfter = null,
            DateTime? timeBefore = null)
        {

            // Because: Without position the query is meaningless
            if (posLat == null || posLng == null)
            {
                return BadRequest("A location(Lat/Lng) is required");
            }

            //// Prepare tagList - Not needed ? 
            //tags = tags ?? new string[0];

            // Prepare recieved coordinates
            var coordinates = string.Format(CultureInfo.InvariantCulture, "POINT({0} {1})", posLng, posLat);

            int timeResult = 0;
            // if both timeAfter and timeBefore !=null, check if timeAfter>timeBefore
            if (timeAfter.HasValue && timeBefore.HasValue)
            {
                // timeResult: (t1 < t2 == -1)   (t1 == t2 == 0)  (t1 > t2 == 1)
                timeResult = TimeSpan.Compare(timeAfter.Value.TimeOfDay, timeBefore.Value.TimeOfDay);
            }

            int userAge = 0;
            string notUserGender = null;
            if (eligibleOnly == true)
            {
                // USER: Get current user - (Check if admin)
                var user = this.AppUserManager.FindById(User.Identity.GetUserId());
                //bool isAdmin = User.IsInRole("Admin");

                // USER: Get DateOfBirth, current date.
                DateTime userBirth = user.DateOfBirth;
                DateTime today = DateTime.Today;

                notUserGender = (user.Gender == "female") ? "male" : "female";

                // Calc: UserAge
                userAge = today.Year - userBirth.Year;
                if (userBirth > today.AddYears(-userAge))
                {
                    userAge--;
                }
            }

            var query = db.Events.Where(e =>
                        e != null &&
                        //e.Status == open && *******ACTIVATE Status*****
                        (eligibleOnly == false ||
                        // Conditions based on user
                        e.AgeMin <= userAge &&
                        e.AgeMax >= userAge &&
                        e.Gender != notUserGender) &&
                        // Conditions based on query string + (eventstart > Now)
                        (ageMin == null ||
                        e.AgeMin >= ageMin) &&
                        (ageMax == null ||
                        e.AgeMax <= ageMax) &&
                        (approvalReq == null ||
                        e.ApprovalReq == approvalReq) &&

                        e.Coordinates.Distance(DbGeography.FromText(coordinates, 4326)) < radius &&

                        //(tags.Count() == 0 ||
                        //e.Tags.Any(eTag => tags.Any(pTag => pTag.Name == eTag.Name))) &&
                        (tags.Count() == 0 ||
                        e.Tags.Any(eTag => tags.Any(pTag => pTag == eTag.Name))) &&


                        // Could: Treat below DT block as 1 scenario.
                        // Then: If betweenDateAfterTime == true && onDateBetweenTime == true
                        // Then: Query must satisfy betweenDateAfterTime block OR onDateBetweenTime block

                        e.EventStart >= DateTime.Now && // This will be server time? != Users local time => Must sync on save or on query.
                        (dateAfter == null ||
                        DbFunctions.TruncateTime(e.EventStart) >= DbFunctions.TruncateTime(dateAfter)) &&
                        (dateBefore == null ||
                        DbFunctions.TruncateTime(e.EventStart) <= DbFunctions.TruncateTime(dateBefore)) &&

                        // Run this if timeAfter && timeBefore HasValue && timeAfter>timeBefore
                        (timeResult != 1 ||
                        (DbFunctions.CreateTime(e.EventStart.Hour, e.EventStart.Minute, e.EventStart.Second) >= DbFunctions.CreateTime(timeAfter.Value.Hour, timeAfter.Value.Minute, timeAfter.Value.Second))
                        ||
                        (DbFunctions.CreateTime(e.EventStart.Hour, e.EventStart.Minute, e.EventStart.Second) <= DbFunctions.CreateTime(timeBefore.Value.Hour, timeBefore.Value.Minute, timeBefore.Value.Second))) &&

                        // Else, run this
                        (timeResult == 1 ||
                        (timeAfter == null ||
                        DbFunctions.CreateTime(e.EventStart.Hour, e.EventStart.Minute, e.EventStart.Second) >= DbFunctions.CreateTime(timeAfter.Value.Hour, timeAfter.Value.Minute, timeAfter.Value.Second)) &&
                        (timeBefore == null ||
                        DbFunctions.CreateTime(e.EventStart.Hour, e.EventStart.Minute, e.EventStart.Second) <= DbFunctions.CreateTime(timeBefore.Value.Hour, timeBefore.Value.Minute, timeBefore.Value.Second))) &&

                        (gender == null ||
                        //gender == "all" || // (Result of "all": "ownGender" + "all".) (NEW/now: Result of "all": all only.)
                        e.Gender == gender) &&

                        (cPartMin == null ||
                        e.Participants.Count >= cPartMin) && // .Count vs .Count() ? 
                        (cPartMax == null ||
                        e.Participants.Count <= cPartMax)
                        //&&
                        //(partMin == null ||
                        //e.PartMin >= partMin) &&
                        //(partMax == null ||
                        //e.PartMax <= partMax)

                        //(searchText == null ||
                        //e.Name.ToLower().Contains(searchText.ToLower()))
                        ).ToList().Select(e => this.TheModelFactory.Create(e, ""));


            // sorting (done with the System.Linq.Dynamic library available on NuGet)
            query = query.OrderBy(sortBy + (reverse ? " descending" : ""));

            var queryPaged = query.Skip((page - 1) * itemsPerPage).Take(itemsPerPage);
            var json = new
            {
                count = query.Count(),
                events = queryPaged
            };

            return Ok(json);
        }

        // UserSpecific: "myEvents"
        [Authorize]
        [Route("api/userevents")]
        public IHttpActionResult GetEvents(
            int page = 1,
            int itemsPerPage = 10,
            string sortBy = "eventStart",
            bool reverse = false,
            DateTime? afterThisDate = null,
            DateTime? beforeThisDate = null)
        {

            // Results can be: Events: After today, After specified date, Before today.
            // Creator
            var creatorQuery = db.Users.Find(User.Identity.GetUserId()).CreatorOf.Where(e => e != null &&
            (beforeThisDate == null ||
            e.EventStart <= beforeThisDate) &&
            (afterThisDate == null ||
            e.EventStart >= afterThisDate)
            );
            // Participant
            var particQuery = db.Users.Find(User.Identity.GetUserId()).ParticipantIn.Where(e => e != null &&
            (beforeThisDate == null ||
            e.EventStart <= beforeThisDate) &&
            (afterThisDate == null ||
            e.EventStart >= afterThisDate) &&
            e.CreatorId != User.Identity.GetUserId()
            );
            // Pending
            var pendingQuery = db.Users.Find(User.Identity.GetUserId()).PendingFor.Where(e => e != null &&
            (beforeThisDate == null ||
            e.EventStart <= beforeThisDate) &&
            (afterThisDate == null ||
            e.EventStart >= afterThisDate)
            );
            // Originally all had ToList() here, Could it be that the query is more efficient without?
            // var creatorOf = creatorQuery.ToList().Select(e => this.TheModelFactory.Create(e, "creator"));
            var creatorOf = creatorQuery.Select(e => this.TheModelFactory.Create(e, "creator"));
            var participantIn = particQuery.Select(e => this.TheModelFactory.Create(e, "participant"));
            var pendingFor = pendingQuery.Select(e => this.TheModelFactory.Create(e, "pending"));
            var myEvents = participantIn.Concat(pendingFor).Concat(creatorOf);

            //query = query.OrderBy(e => e.eventStart);

            // sorting (done with the System.Linq.Dynamic library available on NuGet)
            myEvents = myEvents.OrderBy(sortBy + (reverse ? " descending" : ""));

            var myEventsPaged = myEvents.Skip((page - 1) * itemsPerPage).Take(itemsPerPage);

            var json = new
            {
                count = myEvents.Count(),
                events = myEventsPaged
                //userId = User.Identity.GetUserId() // If I want to customize view on IsCreator
            };

            return Ok(json);
        }


        // POST: api/event
        [ResponseType(typeof(EventDto))]
        [Authorize]
        [Route("api/event")]
        public IHttpActionResult PostEvent(EventDto @event)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Get UserId, Create DummyUser, Attach to context-> Unchanged state.
            string CurrentUserId = User.Identity.GetUserId();
            ApplicationUser DummyUser = new ApplicationUser { Id = CurrentUserId };
            db.Users.Attach(DummyUser);

            // Set CreatorId of Dto
            @event.CreatorId = CurrentUserId;

            // Prepare recieved coordinates
            var coordinates = string.Format(CultureInfo.InvariantCulture, "POINT({0:F5} {1:F5})", @event.PosLng, @event.PosLat);

            // Create NewEvent
            var NewEvent = new Event()
            {
                AgeMax = @event.AgeMax,
                AgeMin = @event.AgeMin,
                CreatorId = @event.CreatorId,
                EventStart = @event.EventStart,
                Gender = @event.Gender,
                PartMax = @event.PartMax,
                PartMin = @event.PartMin,
                Name = @event.Name,
                Coordinates = DbGeography.FromText(coordinates, 4326),
                Description = @event.Description,
                Participants = new Collection<ApplicationUser>(),
                ApprovalReq = @event.ApprovalReq,
                EventStatus = @event.EventStatus,
                Tags = new Collection<Tag>()
            };

            if (db.Images.Find(@event.PictureId) != null)
            {
                NewEvent.PictureId = @event.PictureId;
            }

            // Does this work ? 
            foreach (TagDto eTag in @event.Tags)
            {
                Tag tag = db.Tags.Where(t => t.Name == eTag.Name).FirstOrDefault();

                if (tag != null)
                {
                    tag.Count++;
                    NewEvent.Tags.Add(tag);
                }
                else
                {
                    eTag.Count = 1;
                    var newTag = new Tag
                    {
                        Name = eTag.Name,
                        Count = eTag.Count
                    };
                    NewEvent.Tags.Add(newTag);
                }
            }

            // Add DummyUser as participant (Id is correct)
            NewEvent.Participants.Add(DummyUser);
            // Add NewEvent to context
            db.Events.Add(NewEvent);

            db.SaveChanges();

            @event.EventId = NewEvent.EventId;
            var json = new
            {
                Event = @event,
                userRship = "creator"
            };
            //return Ok();
            return Ok(json);
            //return CreatedAtRoute("DefaultApi", new { id = NewEvent.EventId }, @event);
            //return CreatedAtRoute("DefaultApi", new { id = @event.EventId }, @event);
        }

        // PUT: api/Events/5
        [Authorize]
        [ResponseType(typeof(void))]
        [Route("api/event/{id}")]
        public IHttpActionResult PutEvent(int id, EventDto @eventdto)
        {
            // User must be creator or admin
            if (eventdto.CreatorId != User.Identity.GetUserId() && !User.IsInRole("Admin"))
            {
                return BadRequest("You cannot modify this event");
            }

            // Required fields OK?
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            // Arguments consistent? 
            if (id != @eventdto.EventId)
            {
                return BadRequest();
            }
            // Prepare recieved coordinates
            var coordinates = string.Format(CultureInfo.InvariantCulture, "POINT({0} {1})", @eventdto.PosLng, @eventdto.PosLat);

            // Find event
            Event Event = db.Events.Find(id);

            Event.AgeMin = @eventdto.AgeMin;
            Event.AgeMax = @eventdto.AgeMax;
            Event.Description = @eventdto.Description;
            Event.EventStart = @eventdto.EventStart;
            Event.Gender = @eventdto.Gender;
            Event.Name = @eventdto.Name;
            Event.PartMin = @eventdto.PartMin;
            Event.PartMax = @eventdto.PartMax;
            Event.PictureId = @eventdto.PictureId;
            Event.EventStatus = @eventdto.EventStatus;
            Event.ApprovalReq = @eventdto.ApprovalReq;
            Event.Coordinates = DbGeography.FromText(coordinates, 4326);

            db.Entry(Event).State = EntityState.Modified;


            //// Does this work ? 
            //foreach (Tag eTag in @eventdto.Tags)
            //{
            //    Tag tag = db.Tags.Where(t => t.Name == eTag.Name).FirstOrDefault();

            //    if (tag != null)
            //    {
            //        tag.Count++;
            //        Event.Tags.Add(tag);
            //    }
            //    else
            //    {
            //        eTag.Count = 1;
            //        NewEvent.Tags.Add(eTag);
            //    }
            //}
            // For tags that exists in eventDto, but not in Event from Database
            foreach (TagDto dtoTag in @eventdto.Tags.Where(dtoTag => Event.Tags.Where(eTag => eTag.Name == dtoTag.Name) == null))
            {
                // Does tag exist in database?
                Tag tag = db.Tags.Where(t => t.Name == dtoTag.Name).FirstOrDefault();

                // Yes: Increment Count, Add to databaseEvent.Tags
                if (tag != null)
                {
                    tag.Count++;
                    Event.Tags.Add(tag);
                }
                // No: Set Count = 1, Add to databaseEvent.Tags
                else
                {
                    tag.Count = 1;
                    Event.Tags.Add(tag);
                }

            }

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EventExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }


        [ResponseType(typeof(void))]
        [Authorize]
        [Route("api/participate/{id}/{rShip}")]
        [HttpPut]
        public IHttpActionResult Participate(int id, string rShip)
        {
            // Find event, check null.
            Event @event = db.Events.Find(id);
            if (@event == null)
            {
                return BadRequest("Event not found");
            }
            // Find user, (check null ? ? ?)
            var user = db.Users.Find(User.Identity.GetUserId());
            if (user == null)
            {
                return BadRequest("User not found");
            }

            // If rShip != none: Remove user from Partic/Pending
            if (rShip != "none")
            {
                // Should I confirm user status on serverside/DB? 
                if (rShip == "participant")
                {
                    //@event.Participants.Remove(@event.Participants.Where(u => u.Id == user.Id).FirstOrDefault());
                    @event.Participants.Remove(user);
                }
                else if (rShip == "pending")
                {
                    @event.Pending.Remove(user);
                }
                else
                {
                    return BadRequest("Something went wrong, refresh page and try again");
                }
                rShip = "none";
                //db.Entry(@event).State = EntityState.Modified;
                db.SaveChanges();
                return Ok(rShip);
            }

            // ***If we got here: rShip == none***
            // If event is closed:
            if (@event.EventStatus != "open")
            {
                return BadRequest("The event is closed or cancelled");
            }

            // User: Get DateOfBirth, current time.
            DateTime userBirth = user.DateOfBirth;
            DateTime today = DateTime.Today;

            // User: Get userGender and calculate userAge
            string userGender = user.Gender;
            int userAge = today.Year - userBirth.Year;
            if (userBirth > today.AddYears(-userAge))
            {
                userAge--;
            }

            // If user is: (Too young || too old || not appropriate gender) && not admin
            if (userAge < @event.AgeMin ||
               userAge > @event.AgeMax ||
               !(@event.Gender == "all" || userGender == @event.Gender) &&
               User.IsInRole("Admin") == false)
            {
                return BadRequest("User is not eligible");
            }
            // ApprovalReq == true: Add as pending
            if (@event.ApprovalReq)
            {
                @event.Pending.Add(user);
                rShip = "pending";

                db.SaveChanges();
                return Ok(rShip);
            }
            // ApprovalReq == false: Add as participant
            else if (!@event.ApprovalReq)
            {
                // A user cannot join if the event is full.
                if (@event.PartMax <= @event.Participants.Count)
                {
                    @event.EventStatus = "closed";
                    db.SaveChanges();
                    return BadRequest("Event is full");
                }
                // Add
                @event.Participants.Add(user);
                rShip = "participant";
                // Close Event if full after Addition.
                if (@event.PartMax <= @event.Participants.Count)
                {
                    @event.EventStatus = "closed";
                }
                db.SaveChanges();
                return Ok(rShip);
            }

            return BadRequest("Something went wrong, refresh page and try again");
        }

        // Handle Participants
        [ResponseType(typeof(void))]
        [Authorize]
        [Route("api/handlepartic/{EventId}/{particName}/{creatorAction}")]
        [HttpPost]
        public IHttpActionResult HandlePartic(int EventId, string particName, string creatorAction)
        {
            // Find event, check null.
            Event @event = db.Events.Find(EventId);
            if (@event == null)
            {
                return BadRequest("Event not found");
            }
            // Confirm request is from creator (Or admin)
            if (@event.CreatorId != User.Identity.GetUserId() && !User.IsInRole("Admin"))
            {
                return BadRequest("Only the creator of the event can perform this action");
            }

            if (creatorAction == "remove")
            {

                @event.Participants.Remove(@event.Participants.Where(p => p.UserName == particName).FirstOrDefault());
                //db.Entry(@event).State = EntityState.Modified;
                db.SaveChanges();
                return Ok();
            }

            // Find pending user, check null.
            var userOrNull = @event.Pending.Where(p => p.UserName == particName).FirstOrDefault();

            if (userOrNull == null)
            {
                return BadRequest("User not found");
            }

            if (creatorAction == "reject")
            {
                @event.Pending.Remove(userOrNull);
                db.SaveChanges();
                return Ok();
            }

            if (creatorAction == "accept")
            {
                @event.Pending.Remove(userOrNull);
                @event.Participants.Add(userOrNull);
                db.SaveChanges();
                return Ok();
            }
            return BadRequest("Action is not recognized");
        }

        // Add/Remove Pending from event
        // PUT: api/pending/eventID
        [ResponseType(typeof(void))]
        [Authorize]
        [Route("api/changestatus/{eventId}/{newStatus}")]
        [HttpPut]
        public IHttpActionResult ChangeStatus(int eventId, string newStatus)
        {

            // ***THIS METHOD: Was created without you putting ANY effort into it, think! ***

            // Find event, check null.
            Event @event = db.Events.Find(eventId);
            if (@event == null)
            {
                return BadRequest("Event not found");
            }
            // Confirm request is from creator or admin
            if (@event.CreatorId != User.Identity.GetUserId() && !User.IsInRole("Admin"))
            {
                return BadRequest("Only the creator of the event can perform this action");
            }

            @event.EventStatus = newStatus;
            db.Entry(@event).State = EntityState.Modified;
            db.SaveChanges();
            return Ok(@event.EventStatus);
        }

        // DELETE: api/Events/5
        [Authorize]
        [ResponseType(typeof(Event))]
        [Route("api/event/{id}")]
        public IHttpActionResult DeleteEvent(int id)
        {
            Event @event = db.Events.Find(id);
            if (@event == null)
            {
                return NotFound();
            }

            db.Events.Remove(@event);
            db.SaveChanges();

            return Ok(@event);
        }

        [Authorize]
        [Route("api/defaultimgs")]
        [HttpGet]
        public IHttpActionResult GetDefaultImgs()
        {
            // Where isDefault = true;
            var DefaultImgs = from i in db.Images
                              where i.IsDefault == true
                              select new
                              {
                                  name = i.Name,
                                  pictureId = i.Id,
                                  imageUrl = i.ImageUrl,
                                  isDefault = i.IsDefault,
                                  sizeInKb = i.SizeInKb
                              };

            var json = new
            {
                defaultImgs = DefaultImgs.ToList()
            };
            return Ok(json);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool EventExists(int id)
        {
            return db.Events.Count(e => e.EventId == id) > 0;
        }
    }
}

//******* Interesting technique *********
//ApplicationUser DummyUser = new ApplicationUser { Id = User.Identity.GetUserId() };
//db.Users.Attach(DummyUser);
//@event.Pending.Add(DummyUser);