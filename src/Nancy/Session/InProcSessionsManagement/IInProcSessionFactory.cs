using System;

namespace Nancy.Session.InProcSessionsManagement
{
    internal interface IInProcSessionFactory
    {
        InProcSession Create(Guid sessionId, ISession wrappedSession);
    }
}