using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Text;
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

            ISBMHandler myISBMHandler = new ISBMHandler();
            myISBMHandler.GetChannels(ref responseContent, ref statusCode, ref reasonPhrase);

            HttpResponseMessage response = new HttpResponseMessage(statusCode);
            response.ReasonPhrase = reasonPhrase;
            response.Content = new StringContent(responseContent, Encoding.UTF8, "application/json");

            return response;
        }

        // ChannelManagement
        // Create a new channel with the specified URI path fragment.
        //---------------------------------------------------------------------
        [Route("isbm/2.0/channels")]
        [HttpPost]
        public HttpResponseMessage CreateChannel(HttpRequestMessage request) 
        {
            string body = request.Content.ReadAsStringAsync().Result;

            HttpStatusCode statusCode = HttpStatusCode.OK;
            string reasonPhrase = "";
            string responseContent = "";

            ISBMHandler myISBMHandler = new ISBMHandler();
            myISBMHandler.CreateChannel(body, ref responseContent, ref statusCode, ref reasonPhrase);

            HttpResponseMessage response = new HttpResponseMessage(statusCode);
            response.ReasonPhrase = reasonPhrase;
            response.Content = new StringContent(responseContent, Encoding.UTF8, "application/json");

            return response;
        }

        // ChannelManagement
        // Retreieve the Channel identified by 'channel-id'
        //---------------------------------------------------------------------
        [Route("isbm/2.0/channels/{channelId}")]
        [HttpGet]
        public HttpResponseMessage GetChannel(string channelId)
        {
            HttpStatusCode statusCode = HttpStatusCode.OK;
            string reasonPhrase = "";
            string responseContent = "";

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

            ISBMHandler myISBMHandler = new ISBMHandler();
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
        public HttpResponseMessage OpenProviderPublicationSession(string channelId, HttpRequestMessage request)
        {
            string body = request.Content.ReadAsStringAsync().Result;

            HttpStatusCode statusCode = HttpStatusCode.OK;
            string reasonPhrase = "";
            string responseContent = "";

            ISBMHandler myISBMHandler = new ISBMHandler();
            myISBMHandler.OpenProviderPublicationSession(channelId, body, ref responseContent, ref statusCode, ref reasonPhrase);

            HttpResponseMessage response = new HttpResponseMessage(statusCode);
            response.ReasonPhrase = reasonPhrase;
            response.Content = new StringContent(responseContent, Encoding.UTF8, "application/json");

            return response;
        }

        //ConsumerPublicationService
        //Opens a subscription session for a channel
        //---------------------------------------------------------------------
        [Route("isbm/2.0/channels/{channelId}/subscription-sessions")]
        [HttpPost]
        public HttpResponseMessage OpenConsumerSubscriptionSession(string channelId, HttpRequestMessage request)
        {

            string body = request.Content.ReadAsStringAsync().Result;

            HttpStatusCode statusCode = HttpStatusCode.OK;
            string reasonPhrase = "";
            string responseContent = "";

            ISBMHandler myISBMHandler = new ISBMHandler();
            myISBMHandler.OpenConsumerPublicationSession(channelId, body, ref responseContent, ref statusCode, ref reasonPhrase);

            HttpResponseMessage response = new HttpResponseMessage(statusCode);
            response.ReasonPhrase = reasonPhrase;
            response.Content = new StringContent(responseContent, Encoding.UTF8, "application/json");

            return response;
        }



    }
}
