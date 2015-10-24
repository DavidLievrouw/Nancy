namespace Nancy.Session.InProcSessionsManagement.BySessionIdCookie
{
    using System;

    internal class HmacValidator : IHmacValidator
    {
        public bool IsValidHmac(CookieData cookieData)
        {
            throw new NotImplementedException();
        }
    }
}