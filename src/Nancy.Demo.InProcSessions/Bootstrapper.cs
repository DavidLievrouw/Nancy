namespace Nancy.Demo.InProcSessions
{
    using System;
    using Nancy.Bootstrapper;
    using Nancy.Cryptography;
    using Nancy.Session;
    using Nancy.Session.InProcSessionsManagement.BySessionIdCookie;
    using Nancy.TinyIoc;

    public class Bootstrapper : DefaultNancyBootstrapper
    {
        protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines)
        {
            StaticConfiguration.DisableErrorTraces = false;

            var sessionConfig = new InProcSessionsConfiguration
            {
                SessionTimeout = TimeSpan.FromMinutes(3),
                CacheTrimInterval = TimeSpan.FromMinutes(2),
                SessionIdentificationMethod = new BySessionIdCookieIdentificationMethod(CryptographyConfiguration.Default)
            };
            InProcSessions.Enable(pipelines, sessionConfig);

            base.ApplicationStartup(container, pipelines);
        }
    }
}