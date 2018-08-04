using $safeprojectname$.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace $safeprojectname$.Providers
{
    //public class BlobStorageUploadProvider : MultipartFileStreamProvider
    public class BlobStorageUploadProvider : MultipartFormDataStreamProvider
    {
        // Exposes a Uploads property, which contains the list of files that were uploaded.
        public List<BlobUploadModel> Uploads { get; set; }
        private string FileName { get; set; }

        // Uses Path.GetTempPath() as the temporary location on disk to store the files before sending them to Azure.
        public BlobStorageUploadProvider(string newFileName) : base(Path.GetTempPath())
        {
            Uploads = new List<BlobUploadModel>();
            FileName = newFileName;
        }


        // Overrides ExecutePostProcessingAsync()
        // To inject the uploading of the files to Azure, 
        // Then calls the base to complete the task.
        public override Task ExecutePostProcessingAsync()
        {

            // NOTE: FileData is a property of MultipartFileStreamProvider and is a list of multipart files 
            // that have been uploaded and saved to disk in the Path.GetTempPath() location.
            foreach (var fileData in FileData)
            {
                // Sometimes the filename has a leading and trailing double-quote character when uploaded,
                // so we trim it; otherwise, we get an illegal character exception
                //var fileName = Path.GetFileName(fileData.Headers.ContentDisposition.FileName.Trim('"'));


                // If a fileName is passed to the provider, use that, else, use from FileName from header.
                // Inline with: User/Event -> Only 1 image, 1 name. Default: Many images, many names. 
                var fileName = (FileName == null) ? Path.GetFileName(fileData.Headers.ContentDisposition.FileName.Trim('"')) : FileName;


                // Retrieve reference to a blob - via container from BlobHelpper class.
                var blobContainer = BlobHelper.GetBlobContainer();
                var blob = blobContainer.GetBlockBlobReference(fileName);

                // Set the blob content type
                blob.Properties.ContentType = fileData.Headers.ContentType.MediaType;

                // Upload the file into blob storage, basically copying it from the local disk into Azure
                using (var fs = File.OpenRead(fileData.LocalFileName))
                {
                    // Creates blob if it does not exists, else overwrites blob. 
                    blob.UploadFromStream(fs);
                }

                // Delete the local file from disk
                File.Delete(fileData.LocalFileName);

                // Create blob upload model with properties from blob info
                var blobUpload = new BlobUploadModel
                {
                    FileName = blob.Name,
                    FileUrl = blob.Uri.AbsoluteUri,
                    FileSizeInBytes = blob.Properties.Length,
                    // *** 
                    //IsDefault = gotDefault == "true" ? true : false
                };

                // Add uploaded blob to the list
                Uploads.Add(blobUpload);

            }
            return base.ExecutePostProcessingAsync();
        }
    }
}
