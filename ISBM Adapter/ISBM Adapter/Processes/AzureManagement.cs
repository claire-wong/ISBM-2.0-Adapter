/* Purpose: Implement necessary Microsoft Azure Bus managment features to 
 *          support ISBM APIs in this project
 * 
 * Author: Claire Wong
 * Date Created:  2020/04/29
 * 
 * (c) 2020
 * This code is licensed under MIT license
 * 
*/

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
        //Create Azure Service Bus topic
        public void CreateTopic(string ServiceBusConnectionString, string TopicName)
        {
            ManagementClient myManagementClient = new ManagementClient(ServiceBusConnectionString);

            //Check if the topic already exists
            var task = myManagementClient.TopicExistsAsync(TopicName);
            task.Wait();
            bool isTopicExists = task.Result;

            //Skip creating topic if it already exists 
            if (isTopicExists != true)
            {
               //Create topic
                myManagementClient.CreateTopicAsync(TopicName).Wait();
            }
        }

        //Delete Azure Service Bus topic
        public void DeleteTopic(string ServiceBusConnectionString, string TopicName)
        {
            ManagementClient myManagementClient = new ManagementClient(ServiceBusConnectionString);

            //Check if the topic exists
            var task = myManagementClient.TopicExistsAsync(TopicName);
            task.Wait();
            bool isTopicExists = task.Result;

            //Skip deleting topic if it doesn’t exist
            if (isTopicExists != true)
            {
                //Delete topic
                myManagementClient.DeleteTopicAsync(TopicName).Wait();
            }
        }

        public void CreateTopicSubscriptions(string ServiceBusConnectionString, string TopicName, string SubscriptionName)
        {
            ManagementClient myManagementClient = new ManagementClient(ServiceBusConnectionString);

            //Check if the subscription already exists
            var task = myManagementClient.SubscriptionExistsAsync(TopicName, SubscriptionName);
            task.Wait();
            bool isSubscriptionExists = task.Result;

            //Skip creating a subscription to the topic if it already exists
            if (isSubscriptionExists != true)
            {
                //Create subscription
                myManagementClient.CreateSubscriptionAsync(new SubscriptionDescription(TopicName, SubscriptionName)).Wait();
            }
        }

        public void DeleteTopicSubscriptions(string ServiceBusConnectionString, string TopicName, string SubscriptionName)
        {
            ManagementClient myManagementClient = new ManagementClient(ServiceBusConnectionString);

            //Check if the subscription exists
            var task = myManagementClient.SubscriptionExistsAsync(TopicName, SubscriptionName);
            task.Wait();
            bool isSubscriptionExists = task.Result;

            //Skip deleting subscription if it doesn’t exist
            if (isSubscriptionExists == true)
            {
                //Delete subscription
                myManagementClient.DeleteSubscriptionAsync(TopicName, SubscriptionName).Wait();
            }
        }
    }
}