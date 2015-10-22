using System;

namespace Nancy.Session
{
    using Nancy.Bootstrapper;

    public static class InProcSessions
    {
        /// <summary>
        /// Initialise and add in-process memory based session hooks to the application pipeline
        /// </summary>
        /// <param name="pipelines">Application pipelines</param>
        /// <param name="configuration">In-process memory based sessions configuration.</param>
        public static void Enable(this IPipelines pipelines, InProcSessionsConfiguration configuration)
        {
            if (pipelines == null) throw new ArgumentNullException("pipelines");
            if (configuration == null) throw new ArgumentNullException("configuration");
            if (!configuration.IsValid) throw new ArgumentException("Configuration is invalid", "configuration");

            pipelines.BeforeRequest.AddItemToStartOfPipeline(ctx => LoadSession(ctx, configuration));
            pipelines.AfterRequest.AddItemToEndOfPipeline(ctx => SaveSession(ctx, configuration));
        }

        /// <summary>
        /// Initialise and add in-process memory based session hooks to the application pipeline with the default configuration.
        /// </summary>
        /// <param name="pipelines">Application pipelines</param>
        public static void Enable(this IPipelines pipelines)
        {
            Enable(pipelines, InProcSessionsConfiguration.Default);
        }

        private static Response LoadSession(NancyContext ctx,
            InProcSessionsConfiguration configuration)
        {
            configuration.SessionIdentificationMethodConfiguration.SessionIdentificationMethod.LoadSession(ctx);
            return null;
        }

        private static Response SaveSession(NancyContext ctx,
            InProcSessionsConfiguration configuration)
        {
            configuration.SessionIdentificationMethodConfiguration.SessionIdentificationMethod.SaveSession(ctx);
            return null;
        }
    }
}