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

namespace $safeprojectname$.Controllers
{
    [Authorize]
    public class CommentsController : BaseApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        //public IHttpActionResult EditComment(int id, CommentDto @commentdto)
        // PUT: api/comment/5 + dtoBody
        [Authorize]
        [ResponseType(typeof(void))]
        [Route("api/comment/{id}")]
        [HttpPut]
        public IHttpActionResult EditComment(int id, Comment comment)
        {

            // (meg)Required fields OK? - no such fields atm
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            // Arguments consistent? 
            if (id != comment.CommentId)
            {
                return BadRequest();
            }
            // Find comment
            Comment Comment = db.Comments.Find(id);
            if(Comment == null)
            {
                return NotFound();
            }
            // Change comment
            Comment.CommentText = comment.CommentText;
            // Comment state to modified
            db.Entry(Comment).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException) // Redundant with null check above? 
            {
                if (!CommentExists(id))
                { return NotFound(); }

                else
                { throw; }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // Create
        // POST: api/comment + body
        [Authorize]
        [Route("api/comment")]
        public IHttpActionResult CreateComment(Comment comment)
        {
            var user = db.Users.Find(User.Identity.GetUserId());
            // Set posterId
            comment.PosterId = user.Id;

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Not a reply
            if (comment.ParentId == null)
            {
                db.Comments.Add(comment);
            }

            // Is reply - ***Perhaps add logic checks here, try catch? ***
            else
            {
                var parent = db.Comments.Find(comment.ParentId);
                if (parent == null)
                {
                    return NotFound();
                }

                parent.Replies.Add(comment);
            }

            db.SaveChanges();

            return Ok(TheModelFactory.CreateComment(comment, user.UserName, true, user.PictureId != null ? user.Image.ImageUrl : "/Client/assets/img/signup_male.png")); //Identity.Get vs comment.ApplicationUser.UserName ? ? ?
            //return CreatedAtRoute("DefaultApi", new { id = comment.CommentId }, comment);
        }

        // DELETE: api/comment/5
        [Authorize]
        [ResponseType(typeof(Comment))]
        [Route("api/comment/{id}")]
        public IHttpActionResult DeleteComment(int id)
        {
            Comment comment = db.Comments.Find(id);
            if (comment == null)
            {
                return NotFound();
            }
            if (comment.ParentId == null)
            {
                db.Comments.Remove(comment);
                db.SaveChanges();

                return Ok(comment); // Probably wrong return code/value
            }
            // ***************************************************
            // More checks on validity of parentId?
            // Skipped else here
            // Is this proper usage of try catch? Is it sensible? 
            // ***************************************************
            try
            {
                // ****CONFIRM: That this i acctually needed, is there really no cascading delete?*****
                foreach (Comment reply in comment.Replies)
                {
                    db.Comments.Remove(reply);
                }
                db.Comments.Remove(comment);
                db.SaveChanges();

                return Ok(comment); // Probably wrong return code/value
            }
            catch
            {
                return InternalServerError();
            }

        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool CommentExists(int id)
        {
            return db.Comments.Count(e => e.CommentId == id) > 0;
        }
    }
}
