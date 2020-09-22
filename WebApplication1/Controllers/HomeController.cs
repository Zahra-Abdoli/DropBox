using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebApplication1.Models;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.Azure;
using System.IO;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IAsyncResult Upload()
        {
            string storageConnection = CloudConfigurationManager.GetSetting("BlobStorageConnectionString"); CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(storageConnection);

            //create a block blob
            CloudBlobClient cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();

            //create a container 
            CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference("appcontainer");

            //create a container if it is not already exists

            if (await cloudBlobContainer.CreateIfNotExistsAsync())
            {

                await cloudBlobContainer.SetPermissionsAsync(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });

            }


            string imageName = "Test-" + Path.GetExtension(imageToUpload.FileName);

            //get Blob reference

            CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(imageName); cloudBlockBlob.Properties.ContentType = imageToUpload.ContentType;

            await cloudBlockBlob.UploadFromStreamAsync(imageToUpload.InputStream);
            return View();
        }

        public IActionResult Download()
        {
            var containerName = "testcontainerherbi";

            string storageConnection = CloudConfigurationManager.GetSetting("BlobStorageConnectionString"); CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(storageConnection); CloudBlobClient blobClient = cloudStorageAccount.CreateCloudBlobClient();



            CloudBlobContainer cloudBlobContainer = blobClient.GetContainerReference(containerName); CloudBlockBlob blockBlob = cloudBlobContainer.GetBlockBlobReference("uploadedfilename.ext");



            MemoryStream memStream = new MemoryStream();

            blockBlob.DownloadToStream(memStream);

            HttpContext.Current.Response.ContentType = blockBlob.Properties.ContentType.ToString(); HttpContext.Current.Response.AddHeader("Content-Disposition", "Attachment; filename=" + blockBlob.ToString());

            HttpContext.Current.Response.AddHeader("Content-Length", blockBlob.Properties.Length.ToString()); HttpContext.Current.Response.BinaryWrite(memStream.ToArray()); HttpContext.Current.Response.Flush(); HttpContext.Current.Response.Close();
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
