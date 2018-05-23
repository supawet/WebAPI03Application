using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Net;

namespace WebAPI03Application.MessageHandlers
{
    public class APIKeyMessageHandler : DelegatingHandler
    {
        // "161ACJPBNK4876531" for siam square and "545RGJKD12QAC974" for BLA , "256DVH4789EAXZQ6" for Banpod
        private string[] APIKeyToCheck = { "161ACJPBNK4876531", "545RGJKD12QAC974" , "256DVH4789EAXZQ6" };

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage httpRequestMessage, CancellationToken cancellationToken)
        {
            bool validKey = false;
            IEnumerable<string> requestHeader;
            var checkAPIKeyExists = httpRequestMessage.Headers.TryGetValues("APIKey",out requestHeader);
            if (checkAPIKeyExists)
            {
                //if (requestHeader.FirstOrDefault().Equals(APIKeyToCheck))
                if (Array.Exists(APIKeyToCheck, element => element == requestHeader.FirstOrDefault()))
                {
                    validKey = true;
                }
            }

            if (!validKey)
            {
                return httpRequestMessage.CreateResponse(HttpStatusCode.Forbidden, "Invalid API Key");
            }

            var response = await base.SendAsync(httpRequestMessage, cancellationToken);
            return response;
        }
    }
}