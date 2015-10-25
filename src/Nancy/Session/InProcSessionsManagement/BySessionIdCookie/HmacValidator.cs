namespace Nancy.Session.InProcSessionsManagement.BySessionIdCookie
{
    using System;
    using Nancy.Cryptography;

    internal class HmacValidator : IHmacValidator
    {
        private readonly IHmacProvider hmacProvider;

        public HmacValidator(IHmacProvider hmacProvider)
        {
            if (hmacProvider == null) throw new ArgumentNullException("hmacProvider");
            this.hmacProvider = hmacProvider;
        }

        public bool IsValidHmac(CookieData cookieData)
        {
            if (cookieData == null) throw new ArgumentNullException("cookieData");
            if (cookieData.Hmac == null) return false;

            var incomingBytes = cookieData.Hmac;
            var expectedHmac = this.hmacProvider.GenerateHmac(cookieData.SessionId);
            return HmacComparer.Compare(expectedHmac, incomingBytes, this.hmacProvider.HmacLength);
        }
    }
}