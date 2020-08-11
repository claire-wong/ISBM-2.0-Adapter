/* Purpose: Main class to handle features defined by ISBM 2.0 specifications.
 *          It supports most of the APIs in ChannelManagement, ProviderPublicationService,
 *          and ConsumerPublicationService
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
using System.Data;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Configuration;

namespace ISBM_Adapter.Processes
{
    public class ISBMHandler
    {
        //Retrieve all the channels. No security restriction for now.
        public void GetChannels(ref string responseContent, ref HttpStatusCode statusCode, ref string reasonPhrase)
        {
            try
            {
                //Create a new Database Handler
                DatabaseHandler myDatabaseHandler = new DatabaseHandler();

                //SQL Statement to read all channels
                string sqlStatement = "Select * from Channels";
                DataSet channelDataset = myDatabaseHandler.Select(sqlStatement);

                JArray jArrayObj = new JArray();

                //Skip formating response content if no record returned
                if (channelDataset.Tables[0].Rows.Count != 0)
                {
                    //Add each record to the Newtonsoft JArray object
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

                //Generate JSON string from the jArray object
                responseContent = jArrayObj.ToString(Formatting.None);

                //Set HTTP status code and reason phrase
                //2xx for success
                statusCode = (HttpStatusCode)200;
                reasonPhrase = "A (possibly empty) list of Channels.";

                return;
            }
            catch (Exception e)
            {
                //Set HTTP status code and reason phrase
                //5xx for server error not defined by ISBM 
                statusCode = (HttpStatusCode)500;
                reasonPhrase = "Internal Server Error";

                return;
            }
        }

        //Create a new channel with the specified URI path fragment.
        public void CreateChannel(string body, ref string responseContent, ref HttpStatusCode statusCode, ref string reasonPhrase)
        {
            try
            {
                //Load HTTP request content into Newtonsoft JObject
                JObject jsonBody = JObject.Parse(body);
                JArray securityTokens = (JArray)jsonBody["securityTokens"];

                string channelId = (string)jsonBody["uri"];
                string channelType = (string)jsonBody["channelType"];
                string description = (string)jsonBody["description"];

                //Create a new Database Handler
                DatabaseHandler myDatabaseHandler = new DatabaseHandler();

                //SQL Statement to read channel with matching channelId 
                string sqlStatement = "Select * from Channels where Channel_Id = '" + channelId + "'";
                DataSet channelDataset = myDatabaseHandler.Select(sqlStatement);

                //Skip creating channel if it already exists
                if (channelDataset.Tables[0].Rows.Count == 0)
                {
                    string channelUUID = Guid.NewGuid().ToString();

                    //SQL Statement to create the new channel on database 
                    sqlStatement = "Insert into Channels (Channel_UUID, Channel_Id, Channel_Type, Description) values ('" + channelUUID + "', '" + channelId + "', '" + channelType + "', '" + description + "')";
                    myDatabaseHandler.Insert(sqlStatement);

                    //Get Azure Bus SAS token string from Web.Config
                    string ServiceBusConnectionString = ConfigurationManager.ConnectionStrings["AzureBus"].ConnectionString;
                    //Create a new Azure Management
                    AzureManagement myAzureManagement = new AzureManagement();
                    //Create an Azure Bus topic in the form of UUID corresponding to channel record
                    //ISBM channels are mapped to Azure Bus topics
                    myAzureManagement.CreateTopic(ServiceBusConnectionString, channelUUID);

                    //Set HTTP status code and reason phrase
                    //2xx for success
                    statusCode = (HttpStatusCode)201;
                    reasonPhrase = "The newly created Channel, excluding any configured security tokens.";
                }
                else
                {
                    //Set HTTP status code and reason phrase
                    //4xx for Client Error
                    statusCode = (HttpStatusCode)409;
                    reasonPhrase = "Could not create the channel, URI already exists.";
                }

                return;
            }

            catch (Exception e)
            {
                //Set HTTP status code and reason phrase
                //5xx for server error not defined by ISBM 
                statusCode = (HttpStatusCode)500;
                reasonPhrase = "Internal Server Error";

                return;
            }
        }

        //Opens a publication session for a channel.
        public void OpenProviderPublicationSession(string channelId, ref string responseContent, ref HttpStatusCode statusCode, ref string reasonPhrase)
        {

            try
            {
                //Repace percent enconding charactors
                channelId = System.Uri.UnescapeDataString(channelId);
               
                //Create a new Database Handler
                DatabaseHandler myDatabaseHandler = new DatabaseHandler();

                //SQL Statement to read channel with matching channelId 
                string sqlStatement = "Select * from Channels where Channel_Id = '" + channelId + "'";
                DataSet channelDataset = myDatabaseHandler.Select(sqlStatement);

                //Skip opening a publication session if no record returned
                if (channelDataset.Tables[0].Rows.Count != 0)
                {
                    string channelType = "";
                    channelType = channelDataset.Tables[0].Rows[0]["Channel_Type"].ToString();
                    //Skip opening a publication session if channelType is not of type Publication 
                    if (channelType == "Publication")
                    {
                        string subscribedBy = "Provider";
                        string status = "Open";

                        string sessionUUID = Guid.NewGuid().ToString();
                        string channelUUID = channelDataset.Tables[0].Rows[0]["Channel_UUID"].ToString();

                        //SQL Statement to create the new session on database 
                        sqlStatement = "Insert into Sessions (Session_UUID, Channel_UUID, Channel_Type, Subscribed_By, Status) values ('" + sessionUUID + "', '" + channelUUID + "', '" + channelType + "', '" + subscribedBy + "', '" + status + "')";
                        myDatabaseHandler.Insert(sqlStatement);

                        //Format response content using Newtonsoft JObject
                        JObject jsonSessionId = new JObject();
                        jsonSessionId.AddFirst(new JProperty("sessionId", sessionUUID));
                        //Generate JSON string from the jArray object
                        responseContent = JsonConvert.SerializeObject(jsonSessionId);

                        //Set HTTP status code and reason phrase
                        //2xx for success
                        statusCode = (HttpStatusCode)201;
                        reasonPhrase = "The publication session has been successfully opened on the channel. Only the SessionID is to be returned.";
                    }
                    else
                    {
                        //Set HTTP status code and reason phrase
                        //4xx for Client Error
                        statusCode = (HttpStatusCode)422;
                        reasonPhrase = "The Channel is not of type Publication.";
                    }

                }
                else
                {
                    //Set HTTP status code and reason phrase
                    //4xx for Client Error
                    statusCode = (HttpStatusCode)404;
                    reasonPhrase = "The Channel does not exists.";
                }

            }

            catch (Exception e)
            {
                //Set HTTP status code and reason phrase
                //5xx for server error not defined by ISBM 
                statusCode = (HttpStatusCode)500;
                reasonPhrase = "Internal Server Error";

                return;
            }
        }

        //Opens a subscription session for a channel.
        public void OpenConsumerPublicationSession(string channelId, string body, ref string responseContent, ref HttpStatusCode statusCode, ref string reasonPhrase)
        {

            try
            {
                //Repace percent enconding charactors
                channelId = System.Uri.UnescapeDataString(channelId);

                //Create a new Database Handler
                DatabaseHandler myDatabaseHandler = new DatabaseHandler();

                //SQL Statement to read channel with matching channelId 
                string sqlStatement = "Select * from Channels where Channel_Id = '" + channelId + "'";
                DataSet channelDataset = myDatabaseHandler.Select(sqlStatement);

                //Skip opening a publication session if no record returned
                if (channelDataset.Tables[0].Rows.Count != 0)
                {
                    string channelType = "";
                    channelType = channelDataset.Tables[0].Rows[0]["Channel_Type"].ToString();
                    //Skip opening a publication session if channelType is not of type Publication 
                    if (channelType == "Publication")
                    {
                        string subscribedBy = "Consumer";
                        string status = "Open";

                        string sessionUUID = Guid.NewGuid().ToString();
                        string channelUUID = channelDataset.Tables[0].Rows[0]["Channel_UUID"].ToString();

                        //SQL Statement to create the new session on database 
                        sqlStatement = "Insert into Sessions (Session_UUID, Channel_UUID, Channel_Type, Subscribed_By, Status) values ('" + sessionUUID + "', '" + channelUUID + "', '" + channelType + "', '" + subscribedBy + "', '" + status + "')";
                        myDatabaseHandler.Insert(sqlStatement);

                        //Load HTTP request content into Newtonsoft JObject
                        JObject jsonBody = JObject.Parse(body);
                        JArray topics = (JArray)jsonBody["topics"];
                        int length = topics.Count;

                        //Create a records for each topic on the database 
                        //Topic filtering will be done by this ISBM adapter in the future release 
                        for (int i = 0; i < length; i++)
                        {
                            string topic = (string)topics[i];

                            //SQL Statement to create the new session topic on database 
                            sqlStatement = "Insert into SessionTopics (Session_UUID, Topic) values ('" + sessionUUID + "', '" + topic + "')";
                            myDatabaseHandler.Insert(sqlStatement);
                        }

                        //Format response content using Newtonsoft JObject
                        JObject jsonSessionId = new JObject();
                        jsonSessionId.AddFirst(new JProperty("sessionId", sessionUUID));
                        //Generate JSON string from the jArray object
                        responseContent = JsonConvert.SerializeObject(jsonSessionId);

                        //Get Azure Bus SAS token string from Web.Config
                        string ServiceBusConnectionString = ConfigurationManager.ConnectionStrings["AzureBus"].ConnectionString;
                        //Create a new Azure Management
                        AzureManagement myAzureManagement = new AzureManagement();
                        //Create an Azure Bus topic subscription in the form of UUID corresponding to channel record
                        //ISBM channels are mapped to Azure Bus topics
                        myAzureManagement.CreateTopicSubscriptions(ServiceBusConnectionString, channelUUID, sessionUUID);

                        //Set HTTP status code and reason phrase
                        //2xx for success
                        statusCode = (HttpStatusCode)201;
                        reasonPhrase = "The subscription session has been successfully opened on the channel. Only the SessionID is to be returned.";

                    }
                    else
                    {
                        //Set HTTP status code and reason phrase
                        //4xx for Client Error
                        statusCode = (HttpStatusCode)422;
                        reasonPhrase = "The Channel is not of type Publication.";
                    }
                }
                else
                {
                    //Set HTTP status code and reason phrase
                    //4xx for Client Error
                    statusCode = (HttpStatusCode)404;
                    reasonPhrase = "The Channel does not exists.";
                }
            }

            catch (Exception e)
            {
                //Set HTTP status code and reason phrase
                //5xx for server error not defined by ISBM 
                statusCode = (HttpStatusCode)500;
                reasonPhrase = "Internal Server Error";

                return;
            }
        }

        //Delete the Channel specified by channelId'
        public void DeleteChannel(string channelId, ref string responseContent, ref HttpStatusCode statusCode, ref string reasonPhrase)
        {
            try
            {
                //Repace percent enconding charactors
                channelId = System.Uri.UnescapeDataString(channelId);

                //Create a new Database Handler
                DatabaseHandler myDatabaseHandler = new DatabaseHandler();

                //SQL Statement to read channel with matching channelId 
                string sqlStatement = "Select * from Channels where Channel_Id = '" + channelId + "'";
                DataSet channelDataset = myDatabaseHandler.Select(sqlStatement);

                //Skip deleting the channel if no record returned
                if (channelDataset.Tables[0].Rows.Count != 0)
                {
                    //SQL Statement to delete the channel with matching channelId 
                    sqlStatement = "Delete from Channels where Channel_Id = '" + channelId + "'";
                    myDatabaseHandler.Delete(sqlStatement);

                    string channelUUID = channelDataset.Tables[0].Rows[0]["Channel_UUID"].ToString();

                    //Get Azure Bus SAS token string from Web.Config
                    string ServiceBusConnectionString = ConfigurationManager.ConnectionStrings["AzureBus"].ConnectionString;
                    //Create a new Azure Management
                    AzureManagement myAzureManagement = new AzureManagement();
                    //Delete the Azure Bus topic in the form of UUID corresponding to channel record
                    //ISBM channels are mapped to Azure Bus topics
                    myAzureManagement.DeleteTopic(ServiceBusConnectionString, channelUUID);

                    //Set HTTP status code and reason phrase
                    //2xx for success
                    statusCode = (HttpStatusCode)204;
                    reasonPhrase = "Channel successfully deleted.";

                }
                else
                {
                    //Set HTTP status code and reason phrase
                    //4xx for Client Error
                    statusCode = (HttpStatusCode)404;
                    reasonPhrase = "The channel doesn't exist";

                }
               
            }

            catch (Exception e)
            {
                //Set HTTP status code and reason phrase
                //5xx for server error not defined by ISBM 
                statusCode = (HttpStatusCode)500;
                reasonPhrase = "Internal Server Error";
            }
            
        }

        //Closes a session of any type
        public void CloseSession(string sessionId, ref string responseContent, ref HttpStatusCode statusCode, ref string reasonPhrase)
        {

            try
            {
                //Create a new Database Handler
                DatabaseHandler myDatabaseHandler = new DatabaseHandler();

                //SQL Statement to read session with matching sessionId 
                string sqlStatement = "Select * from Sessions where Session_UUID = '" + sessionId + "'";
                DataSet SessionsDataset = myDatabaseHandler.Select(sqlStatement);

                //Skip closing the session if no record returned
                if (SessionsDataset.Tables[0].Rows.Count != 0)
                {
                    string status = SessionsDataset.Tables[0].Rows[0]["Status"].ToString();

                    string channelUUID = SessionsDataset.Tables[0].Rows[0]["Channel_UUID"].ToString();

                    //Skip closing the session if the status is not "Open"
                    if (status == "Open")
                    {
                        //SQL Statement to update session status to "Close" with matching sessionId 
                        sqlStatement = "Update Sessions set Status = 'Close' where Session_UUID = '" + sessionId + "'";
                        myDatabaseHandler.Update(sqlStatement);

                        //Get Azure Bus SAS token string from Web.Config
                        string ServiceBusConnectionString = ConfigurationManager.ConnectionStrings["AzureBus"].ConnectionString;
                        //Create a new Azure Management
                        AzureManagement myAzureManagement = new AzureManagement();
                        // Delete the Azure Bus topic Subscriptions in the form of UUID corresponding to channel record
                        //ISBM channels are mapped to Azure Bus topics
                        myAzureManagement.DeleteTopicSubscriptions(ServiceBusConnectionString, channelUUID, sessionId);

                        //Set HTTP status code and reason phrase
                        //2xx for success
                        statusCode = (HttpStatusCode)204;
                        reasonPhrase = "Session is successfully closed.";

                    }

                    else
                    {
                        //Set HTTP status code and reason phrase
                        //4xx for Client Error
                        statusCode = (HttpStatusCode)410;
                        reasonPhrase = "The session is closed.This is optional, 404 could be returned instead.";

                    }
                }

                else
                {
                    //Set HTTP status code and reason phrase
                    //4xx for Client Error
                    statusCode = (HttpStatusCode)404;
                    reasonPhrase = "The session does not exist or has been closed.";
                }
                
            }

            catch (Exception e)
            {
                //Set HTTP status code and reason phrase
                //5xx for server error not defined by ISBM 
                statusCode = (HttpStatusCode)500;
                reasonPhrase = "Internal Server Error";

                return;
            }

        }

        //Posts a publication message on a channel.
        public void PostPublicationMessage(string sessionId, string body, ref string responseContent, ref HttpStatusCode statusCode, ref string reasonPhrase)
        {

            try
            {
                //Create a new Database Handler
                DatabaseHandler myDatabaseHandler = new DatabaseHandler();

                //SQL Statement to read session with matching sessionId 
                string sqlStatement = "Select * from Sessions where Session_UUID = '" + sessionId + "'";
                DataSet SessionsDataset = myDatabaseHandler.Select(sqlStatement);

                string status = SessionsDataset.Tables[0].Rows[0]["Status"].ToString();

                //Skip posting publication if no matching open session
                if (SessionsDataset.Tables[0].Rows.Count != 0 && status == "Open")
                { 
                    string channelType = SessionsDataset.Tables[0].Rows[0]["Channel_Type"].ToString();
                    string subscriber = SessionsDataset.Tables[0].Rows[0]["Subscribed_By"].ToString();

                    //Continue posting publication if the channelType is "Publication" and
                    //subsriber is "provider"
                    if (channelType == "Publication" && subscriber == "Provider")
                    {
                        string messageId = Guid.NewGuid().ToString();
                        string channelId = SessionsDataset.Tables[0].Rows[0]["Channel_UUID"].ToString();

                        //Load HTTP request content into Newtonsoft JObject
                        JObject jsonBody = JObject.Parse(body);
                        //Add messageId to the message
                        jsonBody.Property("topics").AddAfterSelf(new JProperty("messageId", messageId));

                        //Generate modified message from the JObject
                        string messageBody = jsonBody.ToString(Formatting.None);

                        //Get Azure Bus SAS token string from Web.Config
                        string ServiceBusConnectionString = ConfigurationManager.ConnectionStrings["AzureBus"].ConnectionString;
                        //Create a new message sender
                        MessageSender myMessageSender = new MessageSender();
                        //Use PublishMessage method to send Azure Bus message
                        myMessageSender.PublishMessage(ServiceBusConnectionString, messageBody, messageId, channelId);

                        //Format response content using Newtonsoft JObject
                        JObject jsonMessageId = new JObject();
                        jsonMessageId.AddFirst(new JProperty("messageId", messageId));
                        //Generate JSON string from the JObject
                        responseContent = JsonConvert.SerializeObject(jsonMessageId);

                        //Set HTTP status code and reason phrase
                        //2xx for success
                        statusCode = (HttpStatusCode)201;
                        reasonPhrase = "The message has been successfully posted to the channel.Returns only the ID of the message.";
                    }

                    else
                    {
                        //Set HTTP status code and reason phrase
                        //4xx for Client Error
                        statusCode = (HttpStatusCode)422;
                        reasonPhrase = "The Session is not of type Publication Provider";
                    }
                }

                else
                {
                    //Set HTTP status code and reason phrase
                    //4xx for Client Error
                    statusCode = (HttpStatusCode)404;
                    reasonPhrase = "The session does not exist or has been closed.";
                }
            }

            catch (Exception e)
            {
                //Set HTTP status code and reason phrase
                //5xx for server error not defined by ISBM 
                statusCode = (HttpStatusCode)500;
                reasonPhrase = "Internal Server Error";

                return;
            }

        }

        //Returns the first non-expired publication message
        public void ReadPublicationMessage(string sessionId, ref string responseContent, ref HttpStatusCode statusCode, ref string reasonPhrase)
        {
            try
            {
                //Create a new Database Handler
                DatabaseHandler myDatabaseHandler = new DatabaseHandler();

                //SQL Statement to read session with matching sessionId 
                string sqlStatement = "Select * from Sessions where Session_UUID = '" + sessionId + "'";
                DataSet SessionsDataset = myDatabaseHandler.Select(sqlStatement);

                string status = SessionsDataset.Tables[0].Rows[0]["Status"].ToString();
                string channelId = SessionsDataset.Tables[0].Rows[0]["Channel_UUID"].ToString();
                long messageCount = 0;

                //Skip posting publication if no matching "Open" session
                if (SessionsDataset.Tables[0].Rows.Count != 0 && status == "Open")
                {
                    string channelType = SessionsDataset.Tables[0].Rows[0]["Channel_Type"].ToString();
                    string subscriber = SessionsDataset.Tables[0].Rows[0]["Subscribed_By"].ToString();

                    //Continue reading publication if the channel type is "Publication" and
                    //subsriber is "Consumer"
                    if (channelType == "Publication" && subscriber == "Consumer")
                    {
                        //Get Azure Bus SAS token string from Web.Config
                        string ServiceBusConnectionString = ConfigurationManager.ConnectionStrings["AzureBus"].ConnectionString;
                        //Create a new message sender
                        MessageReader myMessageReader = new MessageReader();
                        //Use ReadFirstPublishedMessage method to read Azure Bus message
                        responseContent = myMessageReader.ReadFirstPublishedMessage(ServiceBusConnectionString, channelId, sessionId, ref messageCount );
                        
                        if (messageCount != 0)
                        {
                            //Set HTTP status code and reason phrase
                            //2xx for success
                            statusCode = (HttpStatusCode)200;
                            reasonPhrase = "The first non-expired publication message or previously read expired message.";
                        }

                        else
                        {
                            //Set HTTP status code and reason phrase
                            //4xx for Client Error
                            statusCode = (HttpStatusCode)404;
                            reasonPhrase = "The session does not exist (or has been closed) or there are no messages to retrieve.";
                        }
                        
                    }

                    else
                    {
                        //Set HTTP status code and reason phrase
                        //4xx for Client Error
                        statusCode = (HttpStatusCode)422;
                        reasonPhrase = "The Session is not of type Publication Consumer";
                    }
                }

                else
                {
                    //Set HTTP status code and reason phrase
                    //4xx for Client Error
                    statusCode = (HttpStatusCode)404;
                    reasonPhrase = "The session does not exist (or has been closed) or there are no messages to retrieve.";
                }
            }

            catch (Exception e)
            {
                //Set HTTP status code and reason phrase
                //5xx for server error not defined by ISBM 
                statusCode = (HttpStatusCode)500;
                reasonPhrase = "Internal Server Error";

                return;
            }
        }

        //Removes the first, if any, publication message in the subscription queue.
        public void DeletePublicationMessage(string sessionId, ref string responseContent, ref HttpStatusCode statusCode, ref string reasonPhrase)
        {
            try
            {
                //Create a new Database Handler
                DatabaseHandler myDatabaseHandler = new DatabaseHandler();

                //SQL Statement to read session with matching sessionId 
                string sqlStatement = "Select * from Sessions where Session_UUID = '" + sessionId + "'";
                DataSet SessionsDataset = myDatabaseHandler.Select(sqlStatement);

                string status = SessionsDataset.Tables[0].Rows[0]["Status"].ToString();
                string channelId = SessionsDataset.Tables[0].Rows[0]["Channel_UUID"].ToString();

                //Skip deleting publication if no matching "Open" session
                if (SessionsDataset.Tables[0].Rows.Count != 0 && status == "Open")
                {
                    string channelType = SessionsDataset.Tables[0].Rows[0]["Channel_Type"].ToString();
                    string subscriber = SessionsDataset.Tables[0].Rows[0]["Subscribed_By"].ToString();

                    //Continue deleting publication if the channel type is "Publication" and
                    //subsriber is "Consumer"
                    if (channelType == "Publication" && subscriber == "Consumer")
                    {
                        //Get Azure Bus SAS token string from Web.Config
                        string ServiceBusConnectionString = ConfigurationManager.ConnectionStrings["AzureBus"].ConnectionString;
                        //Create a new message reader
                        MessageReader myMessageReader = new MessageReader();
                        //Use RemoveFirstPublishedMessage method to delete the first, if any, publication message in the subscription queue
                        myMessageReader.RemoveFirstPublishedMessage(ServiceBusConnectionString, channelId, sessionId);

                        //Set HTTP status code and reason phrase
                        //2xx for success
                        statusCode = (HttpStatusCode)204;
                        reasonPhrase = "Publication message has been removed from the subscription queue. Note: This response applies even if no messages are in the queue.";
                    }

                    else
                    {
                        //Set HTTP status code and reason phrase
                        //4xx for Client Error
                        statusCode = (HttpStatusCode)422;
                        reasonPhrase = "The Session is not of type Publication Consumer";
                    }
                }

                else
                {
                    //Set HTTP status code and reason phrase
                    //4xx for Client Error
                    statusCode = (HttpStatusCode)404;
                    reasonPhrase = "The session does not exist or has been closed.";
                }
            }

            catch (Exception e)
            {
                //Set HTTP status code and reason phrase
                //5xx for server error not defined by ISBM 
                statusCode = (HttpStatusCode)500;
                reasonPhrase = "Internal Server Error";

                return;
            }
        }
    }
}
