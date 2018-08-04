using $safeprojectname$.Infrastructure;
using $safeprojectname$.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Linq.Dynamic;

namespace $safeprojectname$.Controllers
{
    public class TagsController : BaseApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();




        public IHttpActionResult GetTags(
            // Paging parameters
            int page = 1,
            int itemsPerPage = 10,
            string sortBy = "count",
            bool reverse = true
            )
        {
            //var allImages = from im in context.Images select im;
            //context.Images.RemoveRange(allImages);

            //var query = from tag in db.Tags select tag;
            var query = db.Tags.Where(tag => tag != null);

            // sorting (done with the System.Linq.Dynamic library available on NuGet)
            query = query.OrderBy(sortBy + (reverse ? " descending" : ""));

            var queryPaged = query.Skip((page - 1) * itemsPerPage).Take(itemsPerPage);

            var json = new
            {
                tags = queryPaged.ToList().Select(t => this.TheModelFactory.CreateTag(t))
        };
            return Ok(json);
        }

    }
}
