using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;



namespace ISBM_Adapter.Processes
{
    class MessageReader
    {
        static MessagingFactory Factory;

        public string ReadFirstPublishedMessage(string ServiceBusConnectionString, string TopicName, string SubscriptionName, ref long messageCount)
        {
            var namespaceManager = NamespaceManager.CreateFromConnectionString(ServiceBusConnectionString);
            var subscriptionDesc = namespaceManager.GetSubscription(TopicName, SubscriptionName);
            messageCount = subscriptionDesc.MessageCount;
            if (messageCount != 0)
            {
                Factory = MessagingFactory.CreateFromConnectionString(ServiceBusConnectionString);
                SubscriptionClient mySubscriptionClient = Factory.CreateSubscriptionClient(TopicName, SubscriptionName);

                BrokeredMessage MessageReceived = mySubscriptionClient.Peek();

                Stream myMessage = MessageReceived.GetBody<Stream>();
                Byte[] ByteData = StreamToByteArray(myMessage);
                string strData = Encoding.UTF8.GetString(ByteData);

                MessageReceived.Dispose();
                mySubscriptionClient.Close();

                return strData;

            }

            return "";
        }

        public void RemoveFirstPublishedMessage(string ServiceBusConnectionString, string TopicName, string SubscriptionName)
        {
            var namespaceManager = NamespaceManager.CreateFromConnectionString(ServiceBusConnectionString);
            var subscriptionDesc = namespaceManager.GetSubscription(TopicName, SubscriptionName);
            long messageCount = subscriptionDesc.MessageCount;

            if (messageCount != 0)
            {
                Factory = MessagingFactory.CreateFromConnectionString(ServiceBusConnectionString);
                SubscriptionClient mySubscriptionClient = Factory.CreateSubscriptionClient(TopicName, SubscriptionName);

                BrokeredMessage MessageReceived = mySubscriptionClient.Receive();

                mySubscriptionClient.Complete(MessageReceived.LockToken);

                MessageReceived.Dispose();
                mySubscriptionClient.Close();
            }
        }

        public static byte[] StreamToByteArray(Stream myMessage)
        {
            byte[] bytes = new byte[myMessage.Length];
            using (MemoryStream memoryStream = new MemoryStream())
            {
                int count;
                while ((count = myMessage.Read(bytes, 0, bytes.Length)) > 0)
                {
                    memoryStream.Write(bytes, 0, count);
                }
                return memoryStream.ToArray();
            }
        }




    }
}
