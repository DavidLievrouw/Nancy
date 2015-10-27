namespace Nancy.Session.InProcSessionsManagement.ByQueryStringParam
{
    using System;
    using Nancy.Cryptography;
    using Nancy.Helpers;

    internal class SessionIdentificationDataProvider : ISessionIdentificationDataProvider
    {
        private readonly IByQueryStringParamIdentificationMethod byQueryStringParamIdentificationMethod;
        private readonly IHmacProvider hmacProvider;

        public SessionIdentificationDataProvider(IHmacProvider hmacProvider,
            IByQueryStringParamIdentificationMethod byQueryStringParamIdentificationMethod)
        {
            if (hmacProvider == null) throw new ArgumentNullException("hmacProvider");
            if (byQueryStringParamIdentificationMethod == null)
                throw new ArgumentNullException("byQueryStringParamIdentificationMethod");
            this.hmacProvider = hmacProvider;
            this.byQueryStringParamIdentificationMethod = byQueryStringParamIdentificationMethod;
        }

        public SessionIdentificationData ProvideDataFromQuery(Request request)
        {
            if (request == null) throw new ArgumentNullException("request");

            var querystringDictionary = request.Query.ToDictionary();
            if (querystringDictionary == null ||
                !querystringDictionary.ContainsKey(this.byQueryStringParamIdentificationMethod.ParameterName))
                return null;

            string parameterValue = querystringDictionary[this.byQueryStringParamIdentificationMethod.ParameterName];
            var hmacLength = Base64Helpers.GetBase64Length(this.hmacProvider.HmacLength);

            if (parameterValue.Length < hmacLength)
            {
                // Definitely invalid
                return null;
            }

            var hmacString = parameterValue.Substring(0, hmacLength);
            var encryptedSessionId = parameterValue.Substring(hmacLength);

            var hmacBytes = new byte[] {};
            try
            {
                hmacBytes = Convert.FromBase64String(hmacString);
            }
            catch (FormatException)
            {
                // Invalid HMAC
                return null;
            }

            return new SessionIdentificationData
            {
                SessionId = encryptedSessionId,
                Hmac = hmacBytes
            };
        }
    }
}