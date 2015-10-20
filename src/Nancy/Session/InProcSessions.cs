namespace Nancy.Session
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Nancy.Bootstrapper;
    using Nancy.Cookies;
    using Nancy.Cryptography;
    using Nancy.Helpers;
    using Nancy.Session.Cache;

    /// <summary>
    /// Memory based session storage
    /// </summary>
    public class InProcSessions
    {
        private const int SessionExpirationBufferSeconds = 30;

        private readonly InProcSessionCache cache;
        private readonly InProcSessionsConfiguration currentConfiguration;

        /// <summary>
        /// Initializes a new instance of the <see cref="InProcSessions"/> class.
        /// </summary>
        /// <param name="configuration">Cookie based sessions configuration.</param>
        public InProcSessions(InProcSessionsConfiguration configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException("configuration");
            }

            if (!configuration.IsValid)
            {
                throw new ArgumentException("Configuration is invalid", "configuration");
            }
            this.currentConfiguration = configuration;
            this.cache = new InProcSessionCache();
        }

        /// <summary>
        /// Gets the cookie name that the session is stored in
        /// </summary>
        /// <value>Cookie name</value>
        public string CookieName
        {
            get { return this.currentConfiguration.CookieName; }
        }

        /// <summary>
        /// Initialise and add memory based session hooks to the application pipeline
        /// </summary>
        /// <param name="pipelines">Application pipelines</param>
        /// <param name="configuration">Cookie based sessions configuration.</param>
        public static void Enable(IPipelines pipelines, InProcSessionsConfiguration configuration)
        {
            if (pipelines == null)
            {
                throw new ArgumentNullException("pipelines");
            }

            var sessionStore = new InProcSessions(configuration);

            pipelines.BeforeRequest.AddItemToStartOfPipeline(ctx => LoadSession(ctx, sessionStore));
            pipelines.AfterRequest.AddItemToEndOfPipeline(ctx => SaveSession(ctx, sessionStore));
        }

        /// <summary>
        /// Initialise and add cookie based session hooks to the application pipeline with the default encryption provider.
        /// </summary>
        /// <param name="pipelines">Application pipelines</param>
        /// <returns>Formatter selector for choosing a non-default serializer</returns>
        public static void Enable(IPipelines pipelines)
        {
            Enable(pipelines, InProcSessionsConfiguration.Default);
        }

        /// <summary>
        /// Save the session into the response
        /// </summary>
        /// <param name="session">Session to save</param>
        /// <param name="response">Response to save into</param>
        public void Save(ISession session, Response response)
        {
            if (session == null) return;

            var sessionItemsDictionary = new Dictionary<string, object>();
            if (!(session is NullSessionProvider))
            {
                foreach (var kvp in session)
                {
                    sessionItemsDictionary[kvp.Key] = kvp.Value;
                }
            }

            if (sessionItemsDictionary.Count < 1)
            {
                var cookie = new NancyCookie(currentConfiguration.CookieName, Guid.Empty.ToString(), true)
                {
                    Expires = DateTime.UtcNow.AddDays(-1),
                    Domain = this.currentConfiguration.Domain,
                    Path = this.currentConfiguration.Path
                };
                response.WithCookie(cookie);
            }
            else
            {
                var sessionTimeout =
                    this.currentConfiguration.SessionTimeout.Add(TimeSpan.FromSeconds(SessionExpirationBufferSeconds));
                var newSession = new InProcSession(session, SystemClockAmbientContext.Current.NowUtc, sessionTimeout);
                this.cache.Set(newSession);

                var cryptographyConfiguration = this.currentConfiguration.CryptographyConfiguration;
                var encryptedData = cryptographyConfiguration.EncryptionProvider.Encrypt(newSession.Id.ToString());
                var hmacBytes = cryptographyConfiguration.HmacProvider.GenerateHmac(encryptedData);
                var cookieData = string.Format("{0}{1}", Convert.ToBase64String(hmacBytes), encryptedData);

                // Do not set cookie expiration, in order to abandon the session when the browser is closed
                var cookie = new NancyCookie(this.currentConfiguration.CookieName, cookieData, true)
                {
                    Domain = this.currentConfiguration.Domain,
                    Path = this.currentConfiguration.Path
                };
                response.WithCookie(cookie);
            }
        }

        /// <summary>
        /// Loads the session from the request
        /// </summary>
        /// <param name="request">Request to load from</param>
        /// <returns>ISession containing the load session values</returns>
        public ISession Load(Request request)
        {
            if (request.Cookies.ContainsKey(this.currentConfiguration.CookieName))
            {
                var cookieString = request.Cookies[this.currentConfiguration.CookieName];
                var hmacProvider = this.currentConfiguration.CryptographyConfiguration.HmacProvider;
                var encryptionProvider = this.currentConfiguration.CryptographyConfiguration.EncryptionProvider;

                var cookieData = HttpUtility.UrlDecode(cookieString);
                var hmacLength = Base64Helpers.GetBase64Length(hmacProvider.HmacLength);
                if (cookieData.Length < hmacLength)
                {
                    return new Session();
                }

                var hmacString = cookieData.Substring(0, hmacLength);
                var encryptedCookie = cookieData.Substring(hmacLength);

                var hmacBytes = Convert.FromBase64String(hmacString);
                var newHmac = hmacProvider.GenerateHmac(encryptedCookie);
                var hmacValid = HmacComparer.Compare(newHmac, hmacBytes, hmacProvider.HmacLength);
                if (!hmacValid)
                {
                    return new Session();
                }

                var sessionIdString = encryptionProvider.Decrypt(encryptedCookie);
                
                Guid sessionId;
                if (Guid.TryParse(sessionIdString, out sessionId))
                {
                    var session = this.cache.Get(sessionId);
                    if (session != null) return this.cache.Get(sessionId).WrappedSession;
                }
            }

            return new Session();
        }

        /// <summary>
        /// Saves the request session into the response
        /// </summary>
        /// <param name="context">Nancy context</param>
        /// <param name="sessionStore">Session store</param>
        private static void SaveSession(NancyContext context, InProcSessions sessionStore)
        {
            sessionStore.Save(context.Request.Session, context.Response);
        }

        /// <summary>
        /// Loads the request session
        /// </summary>
        /// <param name="context">Nancy context</param>
        /// <param name="sessionStore">Session store</param>
        /// <returns>Always returns null</returns>
        private static Response LoadSession(NancyContext context, InProcSessions sessionStore)
        {
            if (context.Request == null)
            {
                return null;
            }

            context.Request.Session = sessionStore.Load(context.Request);

            return null;
        }
    }
}