namespace Nancy.Session.InProcSessionsManagement
{
    using System;

    internal interface ISessionIdFactory
    {
        Guid CreateNew();

        Guid? CreateFrom(string sessionIdString);
    }
}