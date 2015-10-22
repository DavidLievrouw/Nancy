﻿using System;

namespace Nancy.Session
{
    using Nancy.Bootstrapper;
    using Nancy.Session.InProcSessionsManagement;
    using Nancy.Session.InProcSessionsManagement.Cache;

    /// <summary>
    /// In-process memory session storage
    /// </summary>
    public static class InProcSessions
    {
        /// <summary>
        /// Initialise and add in-process memory based session hooks to the application pipeline with the default configuration.
        /// </summary>
        /// <param name="pipelines">Application pipelines</param>
        public static void Enable(this IPipelines pipelines)
        {
            Enable(pipelines, InProcSessionsConfiguration.Default);
        }

        /// <summary>
        /// Initialise and add in-process memory based session hooks to the application pipeline
        /// </summary>
        /// <param name="pipelines">Application pipelines</param>
        /// <param name="configuration">In-process memory based sessions configuration.</param>
        public static void Enable(this IPipelines pipelines, InProcSessionsConfiguration configuration)
        {
            if (configuration == null) throw new ArgumentNullException("configuration");
            if (!configuration.IsValid) throw new ArgumentException("Configuration is invalid", "configuration");

            var sessionManager = new InProcSessionManager(
                configuration,
                new InProcSessionCache(new RealSystemClock()), 
                new InProcSessionFactory(configuration, new RealSystemClock()));

            Enable(pipelines, sessionManager);
        }

        internal static void Enable(this IPipelines pipelines, IInProcSessionManager sessionManager)
        {
            if (pipelines == null) throw new ArgumentNullException("pipelines");
            if (sessionManager == null) throw new ArgumentNullException("sessionManager");

            pipelines.BeforeRequest.AddItemToStartOfPipeline(ctx => LoadSession(ctx, sessionManager));
            pipelines.AfterRequest.AddItemToEndOfPipeline(ctx => SaveSession(ctx, sessionManager));
        }

        private static Response LoadSession(NancyContext ctx,
            IInProcSessionManager sessionManager)
        {
            sessionManager.Load(ctx);
            return null;
        }

        private static Response SaveSession(NancyContext ctx,
            IInProcSessionManager sessionManager)
        {
            if (ctx.Request == null) return null;
            sessionManager.Save(ctx.Request.Session, ctx);
            return null;
        }
    }
}