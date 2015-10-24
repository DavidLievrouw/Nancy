namespace Nancy.Session.InProcSessionsManagement.BySessionIdCookie
{
    internal class CookieData
    {
        public string SessionId { get; set; }
        public string Hmac { get; set; }
    }
}