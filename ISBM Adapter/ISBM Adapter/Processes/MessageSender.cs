/* Purpose: Send message to service bus
 *          
 * Author: Claire Wong
 * Date Created:  2020/05/12
 * 
 * (c) 2020
 * This code is licensed under MIT license
 * 
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Management;

namespace ISBM_Adapter.Processes
{
    class MessageSender
    {
        public void PublishMessage(string ServiceBusConnectionString, string body, string messageId, string TopicName)
        {

            // Create topic client
            TopicClient myTopicClient = new TopicClient(ServiceBusConnectionString, TopicName);

            //Create Azure Bus brokered message with the message text coverted into
            //byte array using UTF8 enconding
            var message = new Message(Encoding.UTF8.GetBytes(body));

            //Set assigned message id
            message.MessageId = messageId;

            // Send the message to the topic.
            myTopicClient.SendAsync(message).Wait();

            // Close topic client.
            myTopicClient.CloseAsync().Wait();

        }
    }
}
