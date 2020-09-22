using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApplication2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        // GET: api/<ValuesController>
        [HttpGet]
        public async void Get()
        {
            var containerName = "testcontainerherbi";

            string storageConnection = CloudConfigurationManager.GetSetting("DefaultEndpointsProtocol=https;AccountName=dropbox1;AccountKey=S3LxIjH+HG0c0GgZgdzAGytdj1vHXvGk75g0iauv0StSjtVGT3c8j6I6741s9cW7B32CgI7RENCwnX5va+r67A==;EndpointSuffix=core.windows.net");
            CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(storageConnection);
            CloudBlobClient blobClient = cloudStorageAccount.CreateCloudBlobClient();



            CloudBlobContainer cloudBlobContainer = blobClient.GetContainerReference(containerName);
            CloudBlockBlob blockBlob = cloudBlobContainer.GetBlockBlobReference("uploadedfilename.ext");



            MemoryStream memStream = new MemoryStream();

            await blockBlob.DownloadToStreamAsync(memStream);

            var a = memStream.ToArray();

        }

        // GET api/<ValuesController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<ValuesController>
        [HttpPost]
        public async void Post([FromBody]string imageToUpload)
        {
            string storageConnection = CloudConfigurationManager.GetSetting("DefaultEndpointsProtocol=https;AccountName=dropbox1;AccountKey=S3LxIjH+HG0c0GgZgdzAGytdj1vHXvGk75g0iauv0StSjtVGT3c8j6I6741s9cW7B32CgI7RENCwnX5va+r67A==;EndpointSuffix=core.windows.net");
            CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(storageConnection);

            //create a block blob
            CloudBlobClient cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();

            //create a container 
            CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference("dropbox");

            //create a container if it is not already exists

            if (await cloudBlobContainer.CreateIfNotExistsAsync())
            {

                await cloudBlobContainer.SetPermissionsAsync(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });

            }


            string imageName = "Test-" + Path.GetExtension(imageToUpload.FileName);

            //get Blob reference

            CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(imageName); cloudBlockBlob.Properties.ContentType = imageToUpload.ContentType;

            await cloudBlockBlob.UploadFromStreamAsync(imageToUpload.InputStream);
        }


        // DELETE api/<ValuesController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
