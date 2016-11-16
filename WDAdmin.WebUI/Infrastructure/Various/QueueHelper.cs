using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using WDAdmin.Domain.Entities;

namespace WDAdmin.WebUI.Infrastructure.Various
{
    public class QueueHelper
    {
        private static CloudStorageAccount storageAccount;
        private static CloudQueueClient queueClient;
        private static CloudQueue queue;

        public static void AddToQueue(Video video)
        {
            storageAccount = CloudStorageAccount.Parse(ConfigurationManager.ConnectionStrings["StorageConnectionString"].ConnectionString);
            queueClient = storageAccount.CreateCloudQueueClient();
            queue = queueClient.GetQueueReference("videocompressionqueue");
            string jsonObject = JsonConvert.SerializeObject(video);
            queue.AddMessage(new CloudQueueMessage(jsonObject));

        }
    }
}