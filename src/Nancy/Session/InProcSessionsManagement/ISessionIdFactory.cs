namespace Nancy.Session.InProcSessionsManagement
{
    using System;

    internal interface ISessionIdFactory
    {
        SessionId CreateNew();

        SessionId CreateFrom(string sessionIdString);
    }
}