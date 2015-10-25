namespace Nancy.Session.InProcSessionsManagement.ByQueryStringParam
{
    internal interface ISessionIdentificationDataProvider
    {
        SessionIdentificationData ProvideDataFromQuery(Request request);
    }
}