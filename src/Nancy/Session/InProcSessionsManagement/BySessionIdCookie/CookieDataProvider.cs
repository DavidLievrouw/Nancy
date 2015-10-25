namespace Nancy.Session.InProcSessionsManagement.BySessionIdCookie
{
    using System;
    using Nancy.Cryptography;
    using Nancy.Helpers;

    internal class CookieDataProvider : ICookieDataProvider
    {
        private readonly IHmacProvider hmacProvider;

        public CookieDataProvider(IHmacProvider hmacProvider)
        {
            if (hmacProvider == null) throw new ArgumentNullException("hmacProvider");
            this.hmacProvider = hmacProvider;
        }

        public CookieData ProvideCookieData(Request request, string cookieName)
        {
            if (request == null) throw new ArgumentNullException("request");
            if (string.IsNullOrEmpty(cookieName)) throw new ArgumentNullException("cookieName");

            string cookieValue = null;
            if (!request.Cookies.TryGetValue(cookieName, out cookieValue)) return null;

            var cookieData = HttpUtility.UrlDecode(cookieValue);
            var hmacLength = Base64Helpers.GetBase64Length(this.hmacProvider.HmacLength);

            if (cookieData.Length < hmacLength)
            {
                // Definitely invalid
                return null;
            }

            var hmacString = cookieData.Substring(0, hmacLength);
            var encryptedSessionId = cookieData.Substring(hmacLength);

            var hmacBytes = new byte[] {};
            try
            {
                hmacBytes = Convert.FromBase64String(hmacString);
            }
            catch (FormatException)
            {
                encryptedSessionId = cookieData;
            }

            return new CookieData
            {
                SessionId = encryptedSessionId,
                Hmac = hmacBytes
            };
        }
    }
}