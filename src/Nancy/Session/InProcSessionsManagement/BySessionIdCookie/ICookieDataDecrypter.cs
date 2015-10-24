namespace Nancy.Session.InProcSessionsManagement.BySessionIdCookie
{
    internal interface ICookieDataDecrypter
    {
        string DecryptCookieData(string encryptedCookieData);
    }
}