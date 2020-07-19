/* Purpose: Restful API routes for ISBM session operations
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
using ISBM_Adapter.Processes;

namespace ISBM_Adapter.Controllers
{
    public class SessionsController : ApiController
    {
        // Common Service for Consumer and Provider
        // Closes a session of any type
        //---------------------------------------------------------------------
        [Route("isbm/2.0/sessions/{sessionId}")]
        [HttpDelete]
        public HttpResponseMessage CloseSession(string sessionId)
        {

            HttpStatusCode statusCode = HttpStatusCode.OK;
            string reasonPhrase = "";
            string responseContent = "";

            ISBMHandler myISBMHandler = new ISBMHandler();
            myISBMHandler.CloseSession(sessionId, ref responseContent, ref statusCode, ref reasonPhrase);

            HttpResponseMessage response = new HttpResponseMessage(statusCode);
            response.ReasonPhrase = reasonPhrase;
            response.Content = new StringContent(responseContent, Encoding.UTF8, "application/json");

            return response;
        }

        // Provider Publication Service
        // Posts a publication message on a channel.
        //---------------------------------------------------------------------
        [Route("isbm/2.0/sessions/{sessionId}/publications")]
        [HttpPost]
        public HttpResponseMessage PostPublicationMessage(string sessionId, HttpRequestMessage request)
        {

            string body = request.Content.ReadAsStringAsync().Result;

            HttpStatusCode statusCode = HttpStatusCode.OK;
            string reasonPhrase = "";
            string responseContent = "";

            ISBMHandler myISBMHandler = new ISBMHandler();
            myISBMHandler.PostPublicationMessage(sessionId, body, ref responseContent, ref statusCode, ref reasonPhrase);

            HttpResponseMessage response = new HttpResponseMessage(statusCode);
            response.ReasonPhrase = reasonPhrase;
            response.Content = new StringContent(responseContent, Encoding.UTF8, "application/json");

            return response;
        }

        // Consumer Get Publication
        // Returns the first non-expired publication message or a previously read expired message that satisfies the session message filters.
        //---------------------------------------------------------------------
        [Route("isbm/2.0/sessions/{sessionId}/publication")]
        [HttpGet]
        public HttpResponseMessage ReadPublicationMessage(string sessionId)
        {

            HttpStatusCode statusCode = HttpStatusCode.OK;
            string reasonPhrase = "";
            string responseContent = "";

            ISBMHandler myISBMHandler = new ISBMHandler();
            myISBMHandler.ReadPublicationMessage(sessionId, ref responseContent, ref statusCode, ref reasonPhrase);

            HttpResponseMessage response = new HttpResponseMessage(statusCode);
            response.ReasonPhrase = reasonPhrase;
            response.Content = new StringContent(responseContent, Encoding.UTF8, "application/json");

            return response;
        }

        // Consumer Delete Publication
        // Removes the first, if any, publication message in the subscription queue.
        //---------------------------------------------------------------------
        [Route("isbm/2.0/sessions/{sessionId}/publication")]
        [HttpDelete]
        public HttpResponseMessage DeletePublicationMessage(string sessionId)
        {

            HttpStatusCode statusCode = HttpStatusCode.OK;
            string reasonPhrase = "";
            string responseContent = "";

            ISBMHandler myISBMHandler = new ISBMHandler();
            myISBMHandler.DeletePublicationMessage(sessionId, ref responseContent, ref statusCode, ref reasonPhrase);

            HttpResponseMessage response = new HttpResponseMessage(statusCode);
            response.ReasonPhrase = reasonPhrase;
            response.Content = new StringContent(responseContent, Encoding.UTF8, "application/json");

            return response;
        }

    }

}
