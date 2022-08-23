/* Purpose: Restful API routes for ISBM channel operations
 * 
 * Author: Claire Wong
 * Date Created:  2020/04/23
 * 
 * (c) 2020
 * This code is licensed under MIT license
 * 
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using ISBM_Adapter.Processes;

namespace ISBM_Adapter.Controllers
{
    public class ChannelsController : ApiController
    {
        // ChannelManagement
        // Retrieve all the channels, subject to security permissions
        //---------------------------------------------------------------------
        [Route("isbm/2.0/channels")]
        [HttpGet]
        public HttpResponseMessage GetChannels()
        {
            HttpStatusCode statusCode = HttpStatusCode.OK;
            string reasonPhrase= "";
            string responseContent = "";

            //Create a new ISBMHandler
            ISBMHandler myISBMHandler = new ISBMHandler();
            //Use GetChannels method
            myISBMHandler.GetChannels(ref responseContent, ref statusCode, ref reasonPhrase);

            HttpResponseMessage response = new HttpResponseMessage(statusCode);
            response.ReasonPhrase = reasonPhrase;
            response.Content = new StringContent(responseContent, Encoding.UTF8, "application/json");

            return response;
        }

        // ChannelManagement
        // Create a new channel with specified URI path fragment.
        //---------------------------------------------------------------------
        [Route("isbm/2.0/channels")]
        [HttpPost]
        public HttpResponseMessage CreateChannel([FromBody] JObject bodBody) 
        {
            // Get HTTP request content 
            string body = bodBody.ToString(Formatting.None);

            HttpStatusCode statusCode = HttpStatusCode.OK;
            string reasonPhrase = "";
            string responseContent = "";

            // Create a new ISBMHandler
            ISBMHandler myISBMHandler = new ISBMHandler();
            // Use CreateChannel method
            myISBMHandler.CreateChannel(body, ref responseContent, ref statusCode, ref reasonPhrase);

            HttpResponseMessage response = new HttpResponseMessage(statusCode);
            response.ReasonPhrase = reasonPhrase;
            response.Content = new StringContent(responseContent, Encoding.UTF8, "application/json");

            return response;
        }

        // ChannelManagement
        // Delete the Channel identified by 'channel-id'
        //---------------------------------------------------------------------
        [Route("isbm/2.0/channels/{channelId}")]
        [HttpDelete]
        public HttpResponseMessage DeleteChannel(string channelId)
        {

            HttpStatusCode statusCode = HttpStatusCode.OK;
            string reasonPhrase = "";
            string responseContent = "";

            // Create a new ISBMHandler
            ISBMHandler myISBMHandler = new ISBMHandler();
            // Use DeleteChannel method
            myISBMHandler.DeleteChannel(channelId, ref responseContent, ref statusCode, ref reasonPhrase);

            HttpResponseMessage response = new HttpResponseMessage(statusCode);
            response.ReasonPhrase = reasonPhrase;
            response.Content = new StringContent(responseContent, Encoding.UTF8, "application/json");

            return response;
        }

        // ProviderPublicationService
        // Opens a publication session for a channel
        //---------------------------------------------------------------------
        [Route("isbm/2.0/channels/{channelId}/publication-sessions")]
        [HttpPost]
        public HttpResponseMessage OpenProviderPublicationSession(string channelId)
        {
            // Get HTTP request content 
            // string body = request.Content.ReadAsStringAsync().Result;

            HttpStatusCode statusCode = HttpStatusCode.OK;
            string reasonPhrase = "";
            string responseContent = "";

            // Create a new ISBMHandler
            ISBMHandler myISBMHandler = new ISBMHandler();
            // Use OpenProviderPublicationSession method
            myISBMHandler.OpenProviderPublicationSession(channelId, ref responseContent, ref statusCode, ref reasonPhrase);

            HttpResponseMessage response = new HttpResponseMessage(statusCode);
            response.ReasonPhrase = reasonPhrase;
            response.Content = new StringContent(responseContent, Encoding.UTF8, "application/json");

            return response;
        }

        // ConsumerPublicationService
        // Opens a subscription session for a channel
        //---------------------------------------------------------------------
        [Route("isbm/2.0/channels/{channelId}/subscription-sessions")]
        [HttpPost]
        public HttpResponseMessage OpenConsumerSubscriptionSession(string channelId, [FromBody] JObject bodBody)
        {
            // Get HTTP request content 
            string body = bodBody.ToString(Formatting.None);

            HttpStatusCode statusCode = HttpStatusCode.OK;
            string reasonPhrase = "";
            string responseContent = "";

            // Create a new ISBMHandler
            ISBMHandler myISBMHandler = new ISBMHandler();
            // Use OpenConsumerPublicationSession method
            myISBMHandler.OpenConsumerPublicationSession(channelId, body, ref responseContent, ref statusCode, ref reasonPhrase);

            HttpResponseMessage response = new HttpResponseMessage(statusCode);
            response.ReasonPhrase = reasonPhrase;
            response.Content = new StringContent(responseContent, Encoding.UTF8, "application/json");

            return response;
        }



    }
}
