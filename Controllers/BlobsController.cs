using Microsoft.AspNet.Identity;
using $safeprojectname$.Models;
using $safeprojectname$.Services;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;

namespace $safeprojectname$.Controllers
{
    // Sample service endpoint for storage emulator: http://127.0.0.1:10000/devstoreaccount1/secondcontainer/52.jpg
    // Azure Blob implementation
    // Source: Dave Donaldson http://arcware.net/upload-and-download-files-with-web-api-and-azure-blob-storage/ 

    public class BlobsController : ApiController // Note: Not BaseApiCtrl
    {
        // Interface in place so you can resolve with IoC container of your choice - (DI injection, ninject(?))
        private readonly IBlobService _service = new BlobService();

        /// <summary>
        /// Uploads one or more blob files.
        /// </summary>
        /// <returns>
        /// List of BlobUploadModel.
        /// </returns>
        /// <remarks>
        /// *** I Could easily combine User and Event Endpoints -> Behaviour on eventId provided ? 
        /// </remarks>
        [Route("blobs/uploadUserImage")]
        [ResponseType(typeof(List<BlobUploadModel>))]
        public async Task<IHttpActionResult> UploadUserImage()
        {
            try
            {
                // This endpoint only supports multipart form data
                if (!Request.Content.IsMimeMultipartContent("form-data"))
                {
                    
                    return StatusCode(HttpStatusCode.UnsupportedMediaType);
                }

                // Call service to perform upload, then check result to return as content
                var result = await _service.UploadBlobs(Request.Content, false, null, User.Identity.GetUserId()); // (httpContent, isDefault, eventId?, userId="")
                
                if (result != null && result.Count > 0)
                {
                    return Ok(result); 
                }

                // Otherwise
                return BadRequest();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [Route("blobs/uploadEventImage/{eventId}")]
        [ResponseType(typeof(List<BlobUploadModel>))]
        public async Task<IHttpActionResult> UploadEventImage(int eventId)
        {
            try
            {
                // This endpoint only supports multipart form data
                if (!Request.Content.IsMimeMultipartContent("form-data"))
                {

                    return StatusCode(HttpStatusCode.UnsupportedMediaType);
                }

                // Call service to perform upload, then check result to return as content
                var result = await _service.UploadBlobs(Request.Content, false, eventId, User.Identity.GetUserId());

                if (result != null && result.Count > 0)
                {
                    return Ok(result);
                }

                // Otherwise
                return BadRequest();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }


        [Authorize(Roles = "Admin")]
        [Route("blobs/uploadDefaultImages")]
        [ResponseType(typeof(List<BlobUploadModel>))]
        public async Task<IHttpActionResult> UploadDefaultImages()
        {
            try
            {
                // This endpoint only supports multipart form data
                if (!Request.Content.IsMimeMultipartContent("form-data"))
                {

                    return StatusCode(HttpStatusCode.UnsupportedMediaType);
                }

                // Call service to perform upload, then check result to return as content
                var result = await _service.UploadBlobs(Request.Content, true, null);

                if (result != null && result.Count > 0)
                {
                    return Ok(result);
                }

                // Otherwise
                return BadRequest();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }


        /// <summary>
        /// Downloads a blob file.
        /// Takes a unique blob ID; this is an ID stored in DB
        /// </summary>
        /// <param name="blobId">The ID of the blob.</param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> GetBlobDownload(int blobId)
        {
            // Important: This must return HttpResponseMessage instead if IHttpActionResult

            try
            {
                var result = await _service.DownloadBlob(blobId);
                if (result == null)
                {
                    return new HttpResponseMessage(HttpStatusCode.NotFound);
                }

                // Reset the stream position; otherwise, download will not work
                result.BlobStream.Position = 0;

                // Create response message with blob stream as its content
                var message = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StreamContent(result.BlobStream)
                };

                // Set proper response headers for length, content type, and disposition.
                message.Content.Headers.ContentLength = result.BlobLength;
                message.Content.Headers.ContentType = new MediaTypeHeaderValue(result.BlobContentType);
                message.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                {
                    FileName = HttpUtility.UrlDecode(result.BlobFileName),
                    Size = result.BlobLength
                };

                return message;
            }
            catch (Exception ex)
            {
                return new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    Content = new StringContent(ex.Message)
                };
            }

        }

    }
}
