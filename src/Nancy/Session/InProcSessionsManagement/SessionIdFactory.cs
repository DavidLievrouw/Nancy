namespace Nancy.Session.InProcSessionsManagement
{
    using System;

    internal class SessionIdFactory : ISessionIdFactory
    {
        public Guid CreateNew()
        {
            return Guid.NewGuid();
        }

        public Guid? CreateFrom(string sessionIdString)
        {
            if (sessionIdString == null) return null;

            Guid sessionId;
            if (Guid.TryParse(sessionIdString, out sessionId))
            {
                return sessionId;
            }
            return null;
        }
    }
}