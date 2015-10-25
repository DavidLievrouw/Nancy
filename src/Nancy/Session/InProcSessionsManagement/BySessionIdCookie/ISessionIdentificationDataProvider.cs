namespace Nancy.Session.InProcSessionsManagement.BySessionIdCookie
{
    internal interface ISessionIdentificationDataProvider
    {
        SessionIdentificationData ProvideDataFromCookie(Request request);
    }
}