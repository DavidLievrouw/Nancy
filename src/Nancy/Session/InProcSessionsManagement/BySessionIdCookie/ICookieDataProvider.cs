namespace Nancy.Session.InProcSessionsManagement.BySessionIdCookie
{
    internal interface ICookieDataProvider
    {
        CookieData ProvideCookieData(Request request);
    }
}