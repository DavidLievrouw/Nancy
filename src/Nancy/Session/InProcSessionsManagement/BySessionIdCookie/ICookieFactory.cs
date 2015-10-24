namespace Nancy.Session.InProcSessionsManagement.BySessionIdCookie
{
    using Nancy.Cookies;

    internal interface ICookieFactory
    {
        INancyCookie CreateCookie(CookieData cookieData);
    }
}