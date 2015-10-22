using System;

namespace Nancy.Session.InProcSessionsManagement
{
    using Nancy.Session.InProcSessionsManagement.Cache;

    internal class InProcSessionManager : IInProcSessionManager
    {
        private readonly InProcSessionsConfiguration configuration;
        private readonly IInProcSessionCache sessionCache;
        private readonly IInProcSessionFactory sessionFactory;

        public InProcSessionManager(
            InProcSessionsConfiguration configuration,
            IInProcSessionCache sessionCache,
            IInProcSessionFactory sessionFactory)
        {
            if (configuration == null) throw new ArgumentNullException("configuration");
            if (sessionCache == null) throw new ArgumentNullException("sessionCache");
            if (sessionFactory == null) throw new ArgumentNullException("sessionFactory");
            this.configuration = configuration;
            this.sessionCache = sessionCache;
            this.sessionFactory = sessionFactory;
        }

        public void Save(ISession session, NancyContext context)
        {
            if (context == null) throw new ArgumentNullException("context");

            if (session == null || !session.HasChanged) return;
            if (session is NullSessionProvider || session.Count <= 0) return;

            var identificationMethod =
                this.configuration.SessionIdentificationMethodConfiguration.SessionIdentificationMethod;

            var sessionId = identificationMethod.GetCurrentSessionId(context);
            var inProcSession = this.sessionFactory.Create(sessionId, session);
            this.sessionCache.Set(inProcSession);

            identificationMethod.SaveSessionId(sessionId, context);
        }

        public ISession Load(NancyContext context)
        {
            if (context == null) throw new ArgumentNullException("context");

            var identificationMethod =
                this.configuration.SessionIdentificationMethodConfiguration.SessionIdentificationMethod;

            var sessionId = identificationMethod.GetCurrentSessionId(context);
            return (ISession)this.sessionCache.Get(sessionId) ?? new Session();
        }
    }
}