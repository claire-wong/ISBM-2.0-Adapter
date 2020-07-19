/* Purpose: Read and delete from service bus
 *          
 * Author: Claire Wong
 * Date Created:  2020/05/15
 * 
 * (c) 2020
 * This code is licensed under MIT license
 * 
*/

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

        //Read the first published message from service bus subscription 
        public string ReadFirstPublishedMessage(string ServiceBusConnectionString, string TopicName, string SubscriptionName, ref long messageCount)
        {
            //Create service Bus namespace manager
            var namespaceManager = NamespaceManager.CreateFromConnectionString(ServiceBusConnectionString);
            //Get subscrition information of the topic
            var subscriptionDesc = namespaceManager.GetSubscription(TopicName, SubscriptionName);
            //Check number of published messages
            messageCount = subscriptionDesc.MessageCount;

            //Skip reading message if none exists
            if (messageCount != 0)
            {
                //Create service bus messageing factory
                Factory = MessagingFactory.CreateFromConnectionString(ServiceBusConnectionString);
                //Create subscription client for the topic
                SubscriptionClient mySubscriptionClient = Factory.CreateSubscriptionClient(TopicName, SubscriptionName);

                //Get first broker message from the subscription.
                //Use Peek function to keep the brokerd message in the subscription
                BrokeredMessage MessageReceived = mySubscriptionClient.Peek();

                //Retrieve message body as a stream object
                Stream myMessage = MessageReceived.GetBody<Stream>();
                //Convert the steam object into a byte array
                Byte[] ByteData = StreamToByteArray(myMessage);
                //Convert byte array into message text by using UTF8 encoding  
                string strData = Encoding.UTF8.GetString(ByteData);

                //Clean up
                MessageReceived.Dispose();
                mySubscriptionClient.Close();
               

                return strData;

            }

            return "";
        }

        //Delete the first published message from service bus subscription 
        public void RemoveFirstPublishedMessage(string ServiceBusConnectionString, string TopicName, string SubscriptionName)
        {
            //Create service Bus namespace manager
            var namespaceManager = NamespaceManager.CreateFromConnectionString(ServiceBusConnectionString);
            //Get subscrition information of the topic
            var subscriptionDesc = namespaceManager.GetSubscription(TopicName, SubscriptionName);
            //Check number of published messages
            long messageCount = subscriptionDesc.MessageCount;

            //Skip removing message if none exists
            if (messageCount != 0)
            {
                //Create service bus messageing factory
                Factory = MessagingFactory.CreateFromConnectionString(ServiceBusConnectionString);
                //Create subscription client for the topic
                SubscriptionClient mySubscriptionClient = Factory.CreateSubscriptionClient(TopicName, SubscriptionName);

                //Get first broker message from the subscription.
                //Use Receive function
                BrokeredMessage MessageReceived = mySubscriptionClient.Receive();

                //Use lock token of the received brokered meeage to mark the message is completed.
                //The message will be removed from the subscription
                mySubscriptionClient.Complete(MessageReceived.LockToken);

                //Clean up
                MessageReceived.Dispose();
                mySubscriptionClient.Close();
            }
        }

        //Convert stream to byte array
        public static byte[] StreamToByteArray(Stream myMessage)
        {
            byte[] bytes = new byte[myMessage.Length];
            using (MemoryStream myMemoryStream = new MemoryStream())
            {
                int count;
                while ((count = myMessage.Read(bytes, 0, bytes.Length)) > 0)
                {
                    myMemoryStream.Write(bytes, 0, count);
                }
                return myMemoryStream.ToArray();
            }
        }




    }
}
