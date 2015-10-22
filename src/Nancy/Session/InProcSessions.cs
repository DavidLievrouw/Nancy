using System;

namespace Nancy.Session
{
    using Nancy.Bootstrapper;
    using Nancy.Session.InProcSessionsManagement;

    public class InProcSessions
    {
        /// <summary>
        /// Initialise and add in-process memory based session hooks to the application pipeline
        /// </summary>
        /// <param name="pipelines">Application pipelines</param>
        /// <param name="configuration">Cookie based sessions configuration.</param>
        public static void Enable(IPipelines pipelines, InProcSessionsConfiguration configuration)
        {
            if (pipelines == null) {
                throw new ArgumentNullException("pipelines");
            }

            // Composition root for the in proc sessions management
            IInProcSessionIdentificationMethod sessionIdentificationMethod = new BySessionIdCookieIdentificationMethod();

            pipelines.BeforeRequest.AddItemToStartOfPipeline(ctx => LoadSession(ctx, sessionIdentificationMethod));
            pipelines.AfterRequest.AddItemToEndOfPipeline(ctx => SaveSession(ctx, sessionIdentificationMethod));
        }

        /// <summary>
        /// Initialise and add in-process memory based session hooks to the application pipeline with the default configuration.
        /// </summary>
        /// <param name="pipelines">Application pipelines</param>
        public static void Enable(IPipelines pipelines)
        {
            Enable(pipelines, InProcSessionsConfiguration.Default);
        }

        private static Response LoadSession(NancyContext ctx,
            IInProcSessionIdentificationMethod sessionIdentificationMethod)
        {
            sessionIdentificationMethod.LoadSession(ctx);
            return null;
        }

        private static Response SaveSession(NancyContext ctx,
            IInProcSessionIdentificationMethod sessionIdentificationMethod)
        {
            sessionIdentificationMethod.SaveSession(ctx);
            return null;
        }
    }
}