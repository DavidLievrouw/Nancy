﻿namespace Nancy.Session.InProcSessionsManagement
{
    using System;
    using Nancy.Session.InProcSessionsManagement.Cache;

    internal class InProcSessionFactory : IInProcSessionFactory
    {
        private readonly InProcSessionsConfiguration configuration;
        private readonly ISystemClock systemClock;

        public InProcSessionFactory(InProcSessionsConfiguration configuration, ISystemClock systemClock)
        {
            if (configuration == null) throw new ArgumentNullException("configuration");
            if (systemClock == null) throw new ArgumentNullException("systemClock");
            this.configuration = configuration;
            this.systemClock = systemClock;
        }

        public InProcSession Create(Guid sessionId, ISession wrappedSession)
        {
            if (sessionId == Guid.Empty) throw new ArgumentException("The session id cannot be empty", "sessionId");
            if (wrappedSession == null) throw new ArgumentNullException("wrappedSession");

            return new InProcSession(sessionId, wrappedSession, this.systemClock.NowUtc,
                this.configuration.SessionTimeout);
        }
    }
}