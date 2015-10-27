namespace Nancy.Session.InProcSessionsManagement.ByQueryStringParam
{
    using System;
    using System.Text;
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

        public void ModifyResponseToRedirectToSessionAwareUrl(
            NancyContext context,
            SessionIdentificationData sessionIdentificationData)
        {
            if (context == null) throw new ArgumentNullException("context");
            if (sessionIdentificationData == null) throw new ArgumentNullException("sessionIdentificationData");
            if (context.Request == null)
                throw new ArgumentException("The specified context does not contain a request", "context");
            if (context.Response == null)
                throw new ArgumentException("The specified context does not contain a response", "context");

            var originalUri = (Uri)context.Request.Url;
            var uriBuilder =new UriBuilder(originalUri);
            var queryParameters = HttpUtility.ParseQueryString(uriBuilder.Query);
            queryParameters.Set(this.byQueryStringParamIdentificationMethod.ParameterName, sessionIdentificationData.ToString());

            var newQueryString = string.Empty;
            if (queryParameters.Count > 0) {
                var newQueryBuilder = new StringBuilder();
                foreach (var paramName in queryParameters.AllKeys) {
                    newQueryBuilder.Append(string.Format("{0}={1}&", 
                        paramName, 
                        HttpUtility.UrlEncode(queryParameters[paramName])));
                }
                newQueryString = newQueryBuilder.ToString().TrimEnd('&');
            }
            uriBuilder.Query = newQueryString;
            var redirectUrl = uriBuilder.ToString();

            context.Response.StatusCode = HttpStatusCode.Found;
            context.Response.Headers.Add("Location", redirectUrl);
        }
    }
}