using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus.Management;

namespace ISBM_Adapter.Processes
{
    public class AzureManagement
    {
        public void CreateTopic(string ServiceBusConnectionString, string TopicName)
        {
            ManagementClient myManagementClient = new ManagementClient(ServiceBusConnectionString);

            var task = myManagementClient.TopicExistsAsync(TopicName);
            task.Wait();
            bool isTopicExists = task.Result;

            if (isTopicExists != true)
            {
                myManagementClient.CreateTopicAsync(TopicName).Wait();
            }
        }

        public void DeleteTopic(string ServiceBusConnectionString, string TopicName)
        {
            ManagementClient myManagementClient = new ManagementClient(ServiceBusConnectionString);

            var task = myManagementClient.TopicExistsAsync(TopicName);
            task.Wait();
            bool isTopicExists = task.Result;

            if (isTopicExists != true)
            {
                myManagementClient.DeleteTopicAsync(TopicName).Wait();
            }
        }

        public void CreateTopicSubscriptions(string ServiceBusConnectionString, string TopicName, string SubscriptionName)
        {
            ManagementClient myManagementClient = new ManagementClient(ServiceBusConnectionString);

            var task = myManagementClient.SubscriptionExistsAsync(TopicName, SubscriptionName);
            task.Wait();
            bool isSubscriptionExists = task.Result;

            if (isSubscriptionExists != true)
            {
                myManagementClient.CreateSubscriptionAsync(new SubscriptionDescription(TopicName, SubscriptionName)).Wait();
            }
        }

        public void DeleteTopicSubscriptions(string ServiceBusConnectionString, string TopicName, string SubscriptionName)
        {
            ManagementClient myManagementClient = new ManagementClient(ServiceBusConnectionString);

            var task = myManagementClient.SubscriptionExistsAsync(TopicName, SubscriptionName);
            task.Wait();
            bool isSubscriptionExists = task.Result;

            if (isSubscriptionExists == true)
            {
                myManagementClient.DeleteSubscriptionAsync(TopicName, SubscriptionName).Wait();
            }
        }
    }
}