using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Configuration;

namespace ISBM_Adapter.Processes
{
    public class ISBMHandler
    {
        public void GetChannels(ref string responseContent, ref HttpStatusCode statusCode, ref string reasonPhrase)
        {
            try
            {
                DatabaseHandler myDatabaseHandler = new DatabaseHandler();

                string sqlStatement = "Select * from Channels";
                //SELECT column1, column2, ... FROM table_name WHERE condition;
                DataSet channelDataset = myDatabaseHandler.Select(sqlStatement);

                JArray jArrayObj = new JArray();
                if (channelDataset.Tables[0].Rows.Count != 0)
                {
                    for (int i = 0; i < channelDataset.Tables[0].Rows.Count; i++)
                    {
                        jArrayObj.Add(new JObject()
                        {
                            {"uri", channelDataset.Tables[0].Rows[i]["Channel_Id"].ToString()},
                            {"channelType", channelDataset.Tables[0].Rows[i]["Channel_Type"].ToString()},
                            {"description", channelDataset.Tables[0].Rows[i]["Description"].ToString()},

                        });
                    }
                }

                responseContent = jArrayObj.ToString(Formatting.None);

                statusCode = (HttpStatusCode)200;
                reasonPhrase = "A (possibly empty) list of Channels.";

                return;
            }
            catch (Exception e)
            {
                statusCode = (HttpStatusCode)500;
                reasonPhrase = "Internal Server Error";

                return;
            }
        }

        public void CreateChannel(string body, ref string responseContent, ref HttpStatusCode statusCode, ref string reasonPhrase)
        {
            try
            {
                JObject jsonBody = JObject.Parse(body);
                JArray securityTokens = (JArray)jsonBody["securityTokens"];

                string channelId = (string)jsonBody["uri"];
                string channelType = (string)jsonBody["channelType"];
                string description = (string)jsonBody["description"];

                DatabaseHandler myDatabaseHandler = new DatabaseHandler();

                string sqlStatement = "Select * from Channels where Channel_Id = '" + channelId + "'";
                DataSet channelDataset = myDatabaseHandler.Select(sqlStatement);

                if (channelDataset.Tables[0].Rows.Count == 0)
                {
                    string channelUUID = Guid.NewGuid().ToString();
                   
                    sqlStatement = "Insert into Channels (Channel_UUID, Channel_Id, Channel_Type, Description) values ('" + channelUUID + "', '" + channelId + "', '" + channelType + "', '" + description + "')";
                    myDatabaseHandler.Insert(sqlStatement);

                    string ServiceBusConnectionString = ConfigurationManager.ConnectionStrings["AzureBus"].ConnectionString;
                    AzureManagement myAzureManagement = new AzureManagement();
                    myAzureManagement.CreateTopic(ServiceBusConnectionString, channelUUID);

                    statusCode = (HttpStatusCode)201;
                    reasonPhrase = "The newly created Channel, excluding any configured security tokens.";
                }
                else
                {
                    statusCode = (HttpStatusCode)409;
                    reasonPhrase = "Could not create the channel, URI already exists.";
                }

                return;
            }

            catch (Exception e)
            {
                statusCode = (HttpStatusCode)500;
                reasonPhrase = "Internal Server Error";

                return;
            }
        }

        public void OpenProviderPublicationSession(string channelId, string body, ref string responseContent, ref HttpStatusCode statusCode, ref string reasonPhrase)
        {

            try
            { 

                channelId = channelId.Replace(@"%2F", "/");

                DatabaseHandler myDatabaseHandler = new DatabaseHandler();

                string sqlStatement = "Select * from Channels where Channel_Id = '" + channelId + "'";
                DataSet channelDataset = myDatabaseHandler.Select(sqlStatement);

                if (channelDataset.Tables[0].Rows.Count != 0)
                {
                    string channelType = "";
                    channelType = channelDataset.Tables[0].Rows[0]["Channel_Type"].ToString();
                    if (channelType == "Publication")
                    {
                        string subscribedBy = "Provider";
                        string status = "Open";

                        string sessionUUID = Guid.NewGuid().ToString();
                        string channelUUID = channelDataset.Tables[0].Rows[0]["Channel_UUID"].ToString();

                        sqlStatement = "Insert into Sessions (Session_UUID, Channel_UUID, Channel_Type, Subscribed_By, Status) values ('" + sessionUUID + "', '" + channelUUID + "', '" + channelType + "', '" + subscribedBy + "', '" + status + "')";
                        myDatabaseHandler.Insert(sqlStatement);

                        JObject jsonSessionId = new JObject();
                        jsonSessionId.AddFirst(new JProperty("sessionId", sessionUUID));
                        responseContent = JsonConvert.SerializeObject(jsonSessionId);

                        statusCode = (HttpStatusCode)201;
                        reasonPhrase = "The publication session has been successfully opened on the channel. Only the SessionID is to be returned.";
                    }
                    else
                    {
                        statusCode = (HttpStatusCode)422;
                        reasonPhrase = "The Channel is not of type Publication.";
                    }

                }
                else
                {
                    statusCode = (HttpStatusCode)404;
                    reasonPhrase = "The Channel does not exists.";
                }

            }

            catch (Exception e)
            {
                statusCode = (HttpStatusCode)500;
                reasonPhrase = "Internal Server Error";

                return;
            }
        }

        public void OpenConsumerPublicationSession(string channelId, string body, ref string responseContent, ref HttpStatusCode statusCode, ref string reasonPhrase)
        {

            try
            {

                channelId = channelId.Replace(@"%2F", "/");

                DatabaseHandler myDatabaseHandler = new DatabaseHandler();

                string sqlStatement = "Select * from Channels where Channel_Id = '" + channelId + "'";
                DataSet channelDataset = myDatabaseHandler.Select(sqlStatement);

                if (channelDataset.Tables[0].Rows.Count != 0)
                {
                    string channelType = "";
                    channelType = channelDataset.Tables[0].Rows[0]["Channel_Type"].ToString();
                    if (channelType == "Publication")
                    {
                        string subscribedBy = "Consumer";
                        string status = "Open";

                        string sessionUUID = Guid.NewGuid().ToString();
                        string channelUUID = channelDataset.Tables[0].Rows[0]["Channel_UUID"].ToString();

                        sqlStatement = "Insert into Sessions (Session_UUID, Channel_UUID, Channel_Type, Subscribed_By, Status) values ('" + sessionUUID + "', '" + channelUUID + "', '" + channelType + "', '" + subscribedBy + "', '" + status + "')";
                        myDatabaseHandler.Insert(sqlStatement);

                        JObject jsonBody = JObject.Parse(body);
                        JArray topics = (JArray)jsonBody["topics"];
                        int length = topics.Count;

                        for (int i = 0; i < length; i++)
                        {
                            string topic = (string)topics[i];

                            sqlStatement = "Insert into SessionTopics (Session_UUID, Topic) values ('" + sessionUUID + "', '" + topic + "')";
                            myDatabaseHandler.Insert(sqlStatement);
                        }

                        JObject jsonSessionId = new JObject();
                        jsonSessionId.AddFirst(new JProperty("sessionId", sessionUUID));
                        responseContent = JsonConvert.SerializeObject(jsonSessionId);

                        string ServiceBusConnectionString = ConfigurationManager.ConnectionStrings["AzureBus"].ConnectionString;
                        AzureManagement myAzureManagement = new AzureManagement();
                        myAzureManagement.CreateTopicSubscriptions(ServiceBusConnectionString, channelUUID, sessionUUID);

                        statusCode = (HttpStatusCode)201;
                        reasonPhrase = "The subscription session has been successfully opened on the channel. Only the SessionID is to be returned.";

                    }
                    else
                    {
                        statusCode = (HttpStatusCode)422;
                        reasonPhrase = "The Channel is not of type Publication.";
                    }
                }
                else
                {
                    statusCode = (HttpStatusCode)404;
                    reasonPhrase = "The Channel does not exists.";
                }
            }

            catch (Exception e)
            {
                statusCode = (HttpStatusCode)500;
                reasonPhrase = "Internal Server Error";

                return;
            }
        }

        public void DeleteChannel(string channelId, ref string responseContent, ref HttpStatusCode statusCode, ref string reasonPhrase)
        {
            try
            {
             
                channelId = channelId.Replace(@"%2F", "/");
                DatabaseHandler myDatabaseHandler = new DatabaseHandler();
                string sqlStatement = "Select * from Channels where Channel_Id = '" + channelId + "'";
                DataSet channelDataset = myDatabaseHandler.Select(sqlStatement);

                if (channelDataset.Tables[0].Rows.Count != 0)
                {

                    sqlStatement = "Delete from Channels where Channel_Id = '" + channelId + "'";
                    myDatabaseHandler.Delete(sqlStatement);

                    string ServiceBusConnectionString = ConfigurationManager.ConnectionStrings["AzureBus"].ConnectionString;
                    AzureManagement myAzureManagement = new AzureManagement();
                    myAzureManagement.DeleteTopic(ServiceBusConnectionString, channelId);

                    statusCode = (HttpStatusCode)204;
                    reasonPhrase = "Channel successfully deleted.";

                }
                else
                {

                    statusCode = (HttpStatusCode)404;
                    reasonPhrase = "The channel doesn't exist";

                }
               
            }

            catch (Exception e)
            {
                statusCode = (HttpStatusCode)500;
                reasonPhrase = "Internal Server Error";
            }
            
            
        }

        public void CloseSession(string sessionId, ref string responseContent, ref HttpStatusCode statusCode, ref string reasonPhrase)
        {

            try
            {
                DatabaseHandler myDatabaseHandler = new DatabaseHandler();
                string sqlStatement = "Select * from Sessions where Session_UUID = '" + sessionId + "'";
                DataSet SessionsDataset = myDatabaseHandler.Select(sqlStatement);

                if (SessionsDataset.Tables[0].Rows.Count != 0)
                {
                    string status = SessionsDataset.Tables[0].Rows[0]["Status"].ToString();

                    string channelUUID = SessionsDataset.Tables[0].Rows[0]["Channel_UUID"].ToString();

                    if (status == "Open")
                    {

                        sqlStatement = "Update Sessions set Status = 'Close' where Session_UUID = '" + sessionId + "'";
                        myDatabaseHandler.Update(sqlStatement);

                        string ServiceBusConnectionString = ConfigurationManager.ConnectionStrings["AzureBus"].ConnectionString;
                        AzureManagement myAzureManagement = new AzureManagement();
                        myAzureManagement.DeleteTopicSubscriptions(ServiceBusConnectionString, channelUUID, sessionId);

                        statusCode = (HttpStatusCode)204;
                        reasonPhrase = "Session is successfully closed.";

                    }

                    else
                    {

                        statusCode = (HttpStatusCode)410;
                        reasonPhrase = "The session is closed.This is optional, 404 could be returned instead.";

                    }
                }

                else
                {
                    statusCode = (HttpStatusCode)404;
                    reasonPhrase = "The session does not exist or has been closed.";
                }
                
            }

            catch (Exception e)
            {
                statusCode = (HttpStatusCode)500;
                reasonPhrase = "Internal Server Error";

                return;
            }

        }

        public void PostPublicationMessage(string sessionId, string body, ref string responseContent, ref HttpStatusCode statusCode, ref string reasonPhrase)
        {

            try
            {
                sessionId = sessionId.Replace(@"%2F", "/");

                DatabaseHandler myDatabaseHandler = new DatabaseHandler();
                string sqlStatement = "Select * from Sessions where Session_UUID = '" + sessionId + "'";
                DataSet SessionsDataset = myDatabaseHandler.Select(sqlStatement);
                string status = SessionsDataset.Tables[0].Rows[0]["Status"].ToString();

                if (SessionsDataset.Tables[0].Rows.Count != 0 && status == "Open")
                { 
                    string channelType = SessionsDataset.Tables[0].Rows[0]["Channel_Type"].ToString();
                    string subscriber = SessionsDataset.Tables[0].Rows[0]["Subscribed_By"].ToString(); 

                   if (channelType == "Publication" && subscriber == "Provider")
                    {
                        //    sqlStatement = "Update Sessions set Status = 'Close' where Session_UUID = '" + sessionId + "'";
                        //    myDatabaseHandler.Update(sqlStatement);

                        string messageId = Guid.NewGuid().ToString();
                        string channelId = SessionsDataset.Tables[0].Rows[0]["Channel_UUID"].ToString();

                        JObject jsonBody = JObject.Parse(body);
                        jsonBody.Property("topics").AddAfterSelf(new JProperty("messageId", messageId));

                        string messageBody = jsonBody.ToString(Formatting.None);

                        string ServiceBusConnectionString = ConfigurationManager.ConnectionStrings["AzureBus"].ConnectionString;
                        MessageSender myMessageSender = new MessageSender();
                        myMessageSender.PublishMessage(ServiceBusConnectionString, messageBody, messageId, channelId);

                        JObject jsonMessageId = new JObject();
                        jsonMessageId.AddFirst(new JProperty("messageId", messageId));
                        responseContent = JsonConvert.SerializeObject(jsonMessageId);

                        statusCode = (HttpStatusCode)201;
                        reasonPhrase = "The message has been successfully posted to the channel.Returns only the ID of the message.";
                    }

                    else
                    {
                        statusCode = (HttpStatusCode)422;
                        reasonPhrase = "The Session is not of type Publication Provider";
                    }
                }

                else
                {
                    statusCode = (HttpStatusCode)404;
                    reasonPhrase = "The session does not exist or has been closed.";
                }
            }

            catch (Exception e)
            {
                statusCode = (HttpStatusCode)500;
                reasonPhrase = "Internal Server Error";

                return;
            }

        }

        public void ReadPublicationMessage(string sessionId, ref string responseContent, ref HttpStatusCode statusCode, ref string reasonPhrase)
        {
            try
            {
                //JObject jsonBody = JObject.Parse(body);

                //string messageId = (string)jsonBody["messageId"];
                //string messageType = (string)jsonBody["messageType"];

                sessionId = sessionId.Replace(@"%2F", "/");

                DatabaseHandler myDatabaseHandler = new DatabaseHandler();
                string sqlStatement = "Select * from Sessions where Session_UUID = '" + sessionId + "'";
                DataSet SessionsDataset = myDatabaseHandler.Select(sqlStatement);
                string status = SessionsDataset.Tables[0].Rows[0]["Status"].ToString();
                string channelId = SessionsDataset.Tables[0].Rows[0]["Channel_UUID"].ToString();
                long messageCount = 0;
                
                if (SessionsDataset.Tables[0].Rows.Count != 0 && status == "Open")
                {
                    string channelType = SessionsDataset.Tables[0].Rows[0]["Channel_Type"].ToString();
                    string subscriber = SessionsDataset.Tables[0].Rows[0]["Subscribed_By"].ToString();

                    if (channelType == "Publication" && subscriber == "Consumer")
                    {

                        string ServiceBusConnectionString = ConfigurationManager.ConnectionStrings["AzureBus"].ConnectionString;
                        MessageReader myMessageReader = new MessageReader();
                        responseContent = myMessageReader.ReadFirstPublishedMessage(ServiceBusConnectionString, channelId, sessionId, ref messageCount );
                        
                        if (messageCount != 0)
                        {

                            statusCode = (HttpStatusCode)200;
                            reasonPhrase = "The first non-expired publication message or previously read expired message.";
                        }

                        else
                        {
                            statusCode = (HttpStatusCode)404;
                            reasonPhrase = "The session does not exist (or has been closed) or there are no messages to retrieve.";
                        }
                        
                    }

                    else
                    {
                        statusCode = (HttpStatusCode)422;
                        reasonPhrase = "The Session is not of type Publication Consumer";
                    }
                }

                else
                {
                    statusCode = (HttpStatusCode)404;
                    reasonPhrase = "The session does not exist (or has been closed) or there are no messages to retrieve.";
                }
            }

            catch (Exception e)
            {
                statusCode = (HttpStatusCode)500;
                reasonPhrase = "Internal Server Error";

                return;
            }
        }

        public void DeletePublicationMessage(string sessionId, ref string responseContent, ref HttpStatusCode statusCode, ref string reasonPhrase)
        {
            try
            {
                sessionId = sessionId.Replace(@"%2F", "/");

                DatabaseHandler myDatabaseHandler = new DatabaseHandler();
                string sqlStatement = "Select * from Sessions where Session_UUID = '" + sessionId + "'";
                DataSet SessionsDataset = myDatabaseHandler.Select(sqlStatement);
                string status = SessionsDataset.Tables[0].Rows[0]["Status"].ToString();
                string channelId = SessionsDataset.Tables[0].Rows[0]["Channel_UUID"].ToString();

                if (SessionsDataset.Tables[0].Rows.Count != 0 && status == "Open")
                {
                    string channelType = SessionsDataset.Tables[0].Rows[0]["Channel_Type"].ToString();
                    string subscriber = SessionsDataset.Tables[0].Rows[0]["Subscribed_By"].ToString();

                    if (channelType == "Publication" && subscriber == "Consumer")
                    {

                        string ServiceBusConnectionString = ConfigurationManager.ConnectionStrings["AzureBus"].ConnectionString;
                        MessageReader myMessageReader = new MessageReader();
                        myMessageReader.RemoveFirstPublishedMessage(ServiceBusConnectionString, channelId, sessionId);

                        statusCode = (HttpStatusCode)204;
                        reasonPhrase = "Publication message has been removed from the subscription queue. Note: This response applies even if no messages are in the queue.";
                    }

                    else
                    {
                        statusCode = (HttpStatusCode)422;
                        reasonPhrase = "The Session is not of type Publication Consumer";
                    }
                }

                else
                {
                    statusCode = (HttpStatusCode)404;
                    reasonPhrase = "The session does not exist or has been closed.";
                }
            }

            catch (Exception e)
            {
                statusCode = (HttpStatusCode)500;
                reasonPhrase = "Internal Server Error";

                return;
            }
        }
    }
}
