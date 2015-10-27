namespace Nancy.Session.InProcSessionsManagement.BySessionIdCookie
{
    using System;
    using Nancy.Cryptography;
    using Nancy.Helpers;

    internal class SessionIdentificationDataProvider : ISessionIdentificationDataProvider
    {
        private readonly IBySessionIdCookieIdentificationMethod bySessionIdCookieIdentificationMethod;
        private readonly IHmacProvider hmacProvider;

        public SessionIdentificationDataProvider(IHmacProvider hmacProvider,
            IBySessionIdCookieIdentificationMethod bySessionIdCookieIdentificationMethod)
        {
            if (hmacProvider == null) throw new ArgumentNullException("hmacProvider");
            if (bySessionIdCookieIdentificationMethod == null)
                throw new ArgumentNullException("bySessionIdCookieIdentificationMethod");
            this.hmacProvider = hmacProvider;
            this.bySessionIdCookieIdentificationMethod = bySessionIdCookieIdentificationMethod;
        }

        public SessionIdentificationData ProvideDataFromCookie(Request request)
        {
            if (request == null) throw new ArgumentNullException("request");

            string cookieValue = null;
            if (!request.Cookies.TryGetValue(this.bySessionIdCookieIdentificationMethod.CookieName, out cookieValue))
                return null;

            var decodedCookieValue = HttpUtility.UrlDecode(cookieValue);
            var hmacLength = Base64Helpers.GetBase64Length(this.hmacProvider.HmacLength);

            if (decodedCookieValue.Length < hmacLength)
            {
                // Definitely invalid
                return null;
            }

            var hmacString = decodedCookieValue.Substring(0, hmacLength);
            var encryptedSessionId = decodedCookieValue.Substring(hmacLength);

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