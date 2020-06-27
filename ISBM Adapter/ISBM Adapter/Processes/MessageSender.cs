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
            // GOT AN ISSUE HERE JJI FEDGSIZ
            TopicClient myTopicClient = new TopicClient(ServiceBusConnectionString, TopicName);

            // Create a new message to send to the topic.
            string messageBody = body;
            var message = new Message(Encoding.UTF8.GetBytes(messageBody));
            message.MessageId = messageId;
            // Send the message to the topic.
            myTopicClient.SendAsync(message).Wait();

            // Close topic client.
            myTopicClient.CloseAsync().Wait();

        }
    }
}
