namespace Nancy.Session.InProcSessionsManagement
{
    internal interface ISessionIdFactory
    {
        SessionId CreateNew();

        SessionId CreateFrom(string sessionIdString);
    }
}