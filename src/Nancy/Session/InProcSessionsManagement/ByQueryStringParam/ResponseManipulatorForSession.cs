namespace Nancy.Session.InProcSessionsManagement.ByQueryStringParam
{
    using System;
    using Nancy.Extensions;
    using Nancy.Helpers;

    internal class ResponseManipulatorForSession : IResponseManipulatorForSession
    {
        private readonly IByQueryStringParamIdentificationMethod byQueryStringParamIdentificationMethod;

        public ResponseManipulatorForSession(
            IByQueryStringParamIdentificationMethod byQueryStringParamIdentificationMethod)
        {
            if (byQueryStringParamIdentificationMethod == null)
                throw new ArgumentNullException("byQueryStringParamIdentificationMethod");
            this.byQueryStringParamIdentificationMethod = byQueryStringParamIdentificationMethod;
        }

        public void ModifyResponseToRedirectToSessionUrl(
            NancyContext context,
            SessionIdentificationData sessionIdentificationData)
        {
            if (context == null) throw new ArgumentNullException("context");
            if (sessionIdentificationData == null) throw new ArgumentNullException("sessionIdentificationData");
            if (context.Request == null)
                throw new ArgumentException("The specified context does not contain a request", "context");
            if (context.Response == null)
                throw new ArgumentException("The specified context does not contain a response", "context");

            var queryDictionary = context.Request.Url.Query.AsQueryDictionary();
            string valueToReplace = null;
            dynamic existingSessionIdValue = null;
            if (queryDictionary.TryGetValue(this.byQueryStringParamIdentificationMethod.ParameterName,
                out existingSessionIdValue)) {
                valueToReplace = existingSessionIdValue.ToString();
            }

            var encodedValueForParameter = HttpUtility.UrlEncode(sessionIdentificationData.ToString());
            string redirectLocation = null;
            if (valueToReplace == null) {
                var joinSymbol = context.Request.Url.Query.Length < 1 ? "?" : "&";
                var queryAddition = joinSymbol + this.byQueryStringParamIdentificationMethod.ParameterName +
                                    "=" + encodedValueForParameter;
                redirectLocation = context.Request.Url + queryAddition;
            }
            else {
                redirectLocation = context.Request.Url.ToString().Replace("=" + valueToReplace, "=" + encodedValueForParameter);
            }
            
            context.Response.StatusCode = HttpStatusCode.Found;
            context.Response.Headers.Add("Location", redirectLocation);
        }
    }
}