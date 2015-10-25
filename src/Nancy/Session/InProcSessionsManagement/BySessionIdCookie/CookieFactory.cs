namespace Nancy.Session.InProcSessionsManagement.BySessionIdCookie
{
    using System;
    using Nancy.Cookies;

    internal class CookieFactory : ICookieFactory
    {
        private readonly IBySessionIdCookieIdentificationMethod bySessionIdCookieIdentificationMethod;

        public CookieFactory(IBySessionIdCookieIdentificationMethod bySessionIdCookieIdentificationMethod)
        {
            if (bySessionIdCookieIdentificationMethod == null)
                throw new ArgumentNullException("bySessionIdCookieIdentificationMethod");
            this.bySessionIdCookieIdentificationMethod = bySessionIdCookieIdentificationMethod;
        }

        public INancyCookie CreateCookie(SessionIdentificationData sessionIdentificationData)
        {
            if (sessionIdentificationData == null) throw new ArgumentNullException("sessionIdentificationData");

            return new NancyCookie(
                this.bySessionIdCookieIdentificationMethod.CookieName,
                sessionIdentificationData.ToString(),
                true)
            {
                Domain = this.bySessionIdCookieIdentificationMethod.Domain,
                Path = this.bySessionIdCookieIdentificationMethod.Path
            };
        }
    }
}