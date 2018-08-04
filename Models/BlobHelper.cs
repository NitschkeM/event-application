using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace $safeprojectname$.Models
{

    public static class BlobHelper
    {
        public static CloudBlobContainer GetBlobContainer()
        {
            // myTODO:
            // Could: Give this class access to Identity? Then let it create a container foreach user ? 
            //           + name as GUID ? 
            // Goal: I cannot let images with simple names like "me" etc. overwrite each other. 
            // As for storage space: I can create, in admin view: Delete images+blobs where !isDefault && no user references it. 
            // BlobStorageContainerName
            // Also: I use general connectionstring, he uses BlobStorageConnectionString

            // Pull the from config
            var blobStorageConnectionString = ConfigurationManager.AppSettings["StorageConnectionString"];
            var blobStorageContainerName = ConfigurationManager.AppSettings["BlobStorageContainerName"];
            //var blobStorageConnectionString = ConfigurationManager.AppSettings["BlobStorageConnectionString"];
            //var blobStorageContainerName = ConfigurationManager.AppSettings["BlobStorageContainerName"];

            // Create a blob client and return reference to the container
            var blobStorageAccount = CloudStorageAccount.Parse(blobStorageConnectionString);
            var blobClient = blobStorageAccount.CreateCloudBlobClient();
            //return blobClient.GetContainerReference(blobStorageContainerName);


            // ***Mine***: I do this to create a container
            // Retrieve a reference to a container.
            CloudBlobContainer container = blobClient.GetContainerReference(blobStorageContainerName);
            // Create the container if it dosen't already exists.
            container.CreateIfNotExists();

            // Make container public
            container.SetPermissions(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });

            // return reference to container
            return container;

        }
        
    }
}



// *** This example shows how to create a container if it does not already exist: (From Azure.Microsoft)***

//// Retrieve storage account from connection string.
//CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
//    CloudConfigurationManager.GetSetting("StorageConnectionString"));

//// Create the blob client.
//CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

//// Retrieve a reference to a container.
//CloudBlobContainer container = blobClient.GetContainerReference("mycontainer");

//// Create the container if it doesn't already exist.
//container.CreateIfNotExists();



// By default, the new container is private, meaning that you must specify your storage access key to download blobs from this container.
// If you want to make the files within the container available to everyone, you can set the container to be public using the following code:

// container.SetPermissions(
//     new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });

// Anyone on the Internet can see blobs in a public container, 
// but you can modify or delete them only if you have the appropriate account access key or a shared access signature.