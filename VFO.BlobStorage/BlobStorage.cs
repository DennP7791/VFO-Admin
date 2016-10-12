using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VFO.BlobStorage
{
    public class BlobStorage
    {
        private CloudBlobContainer _Container;

        public BlobStorage()
        {
            Initialize();
        }

        private void Initialize()
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConfigurationManager.ConnectionStrings["StorageConnectionString"].ConnectionString);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            _Container = blobClient.GetContainerReference("vfo-recordings-staging");
        }

        //public static void Upload(string blockBlobReference, file videoStream)
        //{
        //    CloudBlockBlob blockBlob = _Container.GetBlockBlobReference(blockBlobReference);
        //    FileStream fs = System.IO.File.OpenRead("lreabæeææ");

        //    blockBlob.UploadFromStream(videoStream);

        //}

        public byte[] Download(string blockBlobReference)
        {
            CloudBlockBlob blockBlob = _Container.GetBlockBlobReference(blockBlobReference);
            blockBlob.FetchAttributes();
            long fileByteLength = blockBlob.Properties.Length;
            byte[] fileContent = new byte[fileByteLength];
            for (int i = 0; i < fileByteLength; i++)
            {
                fileContent[i] = 0x20;
            }
            blockBlob.DownloadToByteArray(fileContent, 0);
            return fileContent;

        }
    }
}
