namespace Nancy.Session.InProcSessionsManagement.BySessionIdCookie
{
    internal interface IHmacValidator
    {
        bool IsValidHmac(CookieData cookieData);
    }
}