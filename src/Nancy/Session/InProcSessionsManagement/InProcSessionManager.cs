using System;

namespace Nancy.Session.InProcSessionsManagement
{
    using Nancy.Session.InProcSessionsManagement.Cache;

    internal class InProcSessionManager : IInProcSessionManager
    {
        private readonly InProcSessionsConfiguration configuration;
        private readonly IInProcSessionCache sessionCache;
        private readonly IInProcSessionFactory sessionFactory;
        private readonly IPeriodicCacheCleaner periodicCacheCleaner;

        public InProcSessionManager(
            InProcSessionsConfiguration configuration,
            IInProcSessionCache sessionCache,
            IInProcSessionFactory sessionFactory,
            IPeriodicCacheCleaner periodicCacheCleaner)
        {
            if (configuration == null) throw new ArgumentNullException("configuration");
            if (sessionCache == null) throw new ArgumentNullException("sessionCache");
            if (sessionFactory == null) throw new ArgumentNullException("sessionFactory");
            if (periodicCacheCleaner == null) throw new ArgumentNullException("periodicCacheCleaner");
            this.configuration = configuration;
            this.sessionCache = sessionCache;
            this.sessionFactory = sessionFactory;
            this.periodicCacheCleaner = periodicCacheCleaner;

            // Start periodic cleaning
            this.periodicCacheCleaner.Start();
        }

        /// <summary>
        /// Save the session, for future reference.
        /// </summary>
        /// <param name="session">The session to save.</param>
        /// <param name="context">The current context.</param>
        /// <returns>The early exit response, or null, if everything is OK.</returns>
        public Response Save(ISession session, NancyContext context)
        {
            if (context == null) throw new ArgumentNullException("context");

            if (session == null || !session.HasChanged) return null;
            if (session is NullSessionProvider || session.Count <= 0) return null;

            var identificationMethod = this.configuration.SessionIdentificationMethod;

            var sessionId = identificationMethod.GetCurrentSessionId(context);
            var inProcSession = this.sessionFactory.Create(sessionId, session);
            this.sessionCache.Set(inProcSession);

            return identificationMethod.SaveSessionId(sessionId, context);
        }

        /// <summary>
        /// Load the session that is owned by the specified context.
        /// </summary>
        /// <param name="context">The current context.</param>
        /// <returns>The session that is owned by the specified context, or a new empty session, if none exists.</returns>
        public ISession Load(NancyContext context)
        {
            if (context == null) throw new ArgumentNullException("context");

            var identificationMethod = this.configuration.SessionIdentificationMethod;

            var sessionId = identificationMethod.GetCurrentSessionId(context);
            return (ISession)this.sessionCache.Get(sessionId) ?? new Session();
        }
    }
}