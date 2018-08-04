using $safeprojectname$.Infrastructure;
using $safeprojectname$.Models;
using $safeprojectname$.Providers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace $safeprojectname$.Services
{
    public interface IBlobService
    {
        Task<List<BlobUploadModel>> UploadBlobs(HttpContent httpContent, bool isDefault, int? eventId, string userId = null);
        //Task<List<BlobUploadModel>> UploadBlobs(HttpContent httpContent, string userId);
        Task<BlobDownloadModel> DownloadBlob(int blobId);
    }

    public class BlobService : IBlobService
    {

        public async Task<List<BlobUploadModel>> UploadBlobs(HttpContent httpContent, bool isDefault, int? eventId, string userId = null)
        {
            // Supports multiple files at once.
            // Creates an instance of BlobStorageUploadProvider, which gets passed to the
            // ReadAsMultipartAsync() call to perform the actual upload of files.
            string newFileName = null;
            if (!isDefault)
            {
                if (eventId.HasValue)
                {
                    using (var db = new ApplicationDbContext())
                    {
                        if (db.Events.Find(eventId.Value).CreatorId != userId)
                        {
                            return null;
                        }
                        newFileName = eventId.Value.ToString();
                    }
                }
                else { newFileName = userId; }
            }

            var blobUploadProvider = new BlobStorageUploadProvider(newFileName);

            var list = await httpContent.ReadAsMultipartAsync(blobUploadProvider)
                .ContinueWith(task =>
                {
                    if (task.IsFaulted || task.IsCanceled)
                    {
                        throw task.Exception;
                    }
                    var provider = task.Result;

                    return provider.Uploads.ToList();
                });

            // TODO:
            // Use data in the list to store blob info in your database so that you can always retrieve it later. 

            using (var db = new ApplicationDbContext())
            {
                foreach (BlobUploadModel blob in list)
                {
                    blob.IsDefault = isDefault;
                    var image = new Image
                    {
                        Name = blob.FileName,
                        ImageUrl = blob.FileUrl,
                        SizeInBytes = blob.FileSizeInBytes,
                        SizeInKb = blob.FileSizeInKb,
                        // ***
                        //IsDefault = blob.IsDefault
                        IsDefault = blob.IsDefault
                    };

                    db.Images.Add(image);
                    db.SaveChanges();
                    // Include imageId in the returned BlobUploadModels
                    blob.PictureId = image.Id;

                    if (!isDefault)
                    {
                        if (eventId.HasValue){db.Events.Find(eventId.Value).PictureId = image.Id;}
                        else { db.Users.Find(userId).PictureId = image.Id; }
                    }
                    db.SaveChanges();
                }
            }
            return list;
        }


        public async Task<BlobDownloadModel> DownloadBlob(int blobId)
        {
            // TODO: var blobName = GetBlobName(blobId); 
            // You must implement this helper method. It should retrieve blob info from your database, based on the blobId.
            // The record should contain the blobName, which you should return as the result of this helper method.

            string blobName; // Tutorial used var blobName.

            using (var context = new ApplicationDbContext())
            {
                blobName = context.Images.Find(blobId).Name;
            }


            if (!String.IsNullOrEmpty(blobName))
            {
                var container = BlobHelper.GetBlobContainer();
                var blob = container.GetBlockBlobReference(blobName);

                // Download the blob into a memory stream. Notice that we're not putting the memory
                // stream in a using statement. This is because we need the stream to be open for the
                // API controller in order for the file to acctually be downloadable. The closing and
                // disposing og the stream is handled by the Web API framework.
                var ms = new MemoryStream();
                await blob.DownloadToStreamAsync(ms);

                // Strip off any folder structure so the file name is just the file name
                var lastPos = blob.Name.LastIndexOf('/');
                var fileName = blob.Name.Substring(lastPos + 1, blob.Name.Length - lastPos - 1);

                // Build and return the download model with the blob stream and its relevant info.
                var download = new BlobDownloadModel
                {
                    BlobStream = ms,
                    BlobFileName = fileName,
                    BlobLength = blob.Properties.Length,
                    BlobContentType = blob.Properties.ContentType
                };

                return download;
            }

            // Otherwise
            return null;
        }


    }
}
