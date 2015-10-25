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

        public INancyCookie CreateCookie(CookieData cookieData)
        {
            if (cookieData == null) throw new ArgumentNullException("cookieData");

            return new NancyCookie(
                this.bySessionIdCookieIdentificationMethod.CookieName,
                cookieData.ToString(),
                true)
            {
                Domain = this.bySessionIdCookieIdentificationMethod.Domain,
                Path = this.bySessionIdCookieIdentificationMethod.Path
            };
        }
    }
}