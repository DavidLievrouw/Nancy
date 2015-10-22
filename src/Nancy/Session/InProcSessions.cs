namespace Nancy.Session
{
    using System;
    using Nancy.Bootstrapper;
    using Nancy.Cookies;
    using Nancy.Cryptography;
    using Nancy.Helpers;
    using Nancy.Session.InProcSessionsManagement;
    using Nancy.Session.InProcSessionsManagement.Cache;

    /// <summary>
    /// In-process memory based session storage
    /// </summary>
    public class InProcSessions
    {
        private const int SessionExpirationBufferSeconds = 20;

        private readonly InProcSessionCache cache;
        private readonly InProcSessionsConfiguration currentConfiguration;
        private readonly ISystemClock systemClock;

        /// <summary>
        /// Initializes a new instance of the <see cref="InProcSessions"/> class.
        /// </summary>
        /// <param name="configuration">Cookie based sessions configuration.</param>
        public InProcSessions(InProcSessionsConfiguration configuration) : this(configuration, new RealSystemClock())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InProcSessions"/> class.
        /// </summary>
        /// <param name="configuration">Cookie based sessions configuration.</param>
        /// <param name="systemClock">The system clock to use.</param>
        internal InProcSessions(InProcSessionsConfiguration configuration, ISystemClock systemClock)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException("configuration");
            }

            if (!configuration.IsValid)
            {
                throw new ArgumentException("Configuration is invalid", "configuration");
            }

            // Create DI object graph
            this.systemClock = systemClock;

            this.currentConfiguration = configuration;
            this.systemClock = systemClock;
            this.cache = new InProcSessionCache(this.systemClock);
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
        /// <param name="sessionId">The unique identifier of the session.</param>
        /// <param name="session">Session to save</param>
        /// <param name="response">Response to save into</param>
        public void Save(Guid sessionId, ISession session, Response response)
        {
            if (session == null || !session.HasChanged) return;

            if (!(session is NullSessionProvider) && session.Count > 0)
            {
                var sessionTimeout =
                    this.currentConfiguration.SessionTimeout.Add(TimeSpan.FromSeconds(SessionExpirationBufferSeconds));
                var inProcSession = new InProcSession(sessionId, session, this.systemClock.NowUtc, sessionTimeout);
                this.cache.Set(inProcSession);

                var cryptographyConfiguration = this.currentConfiguration.CryptographyConfiguration;
                var encryptedData = cryptographyConfiguration.EncryptionProvider.Encrypt(inProcSession.Id.ToString());
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
                    if (session != null) return session;
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
            // ToDo: Improvement: Trim the session store async and periodically
            sessionStore.cache.Trim();

            var sessionId = Guid.NewGuid();

            if (context.Request.Cookies.ContainsKey(sessionStore.currentConfiguration.CookieName))
            {
                var cookieString = context.Request.Cookies[sessionStore.currentConfiguration.CookieName];
                var hmacProvider = sessionStore.currentConfiguration.CryptographyConfiguration.HmacProvider;
                var encryptionProvider = sessionStore.currentConfiguration.CryptographyConfiguration.EncryptionProvider;

                var cookieData = HttpUtility.UrlDecode(cookieString);
                var hmacLength = Base64Helpers.GetBase64Length(hmacProvider.HmacLength);
                if (cookieData.Length >= hmacLength)
                {
                    var hmacString = cookieData.Substring(0, hmacLength);
                    var encryptedCookie = cookieData.Substring(hmacLength);

                    var hmacBytes = Convert.FromBase64String(hmacString);
                    var newHmac = hmacProvider.GenerateHmac(encryptedCookie);
                    var hmacValid = HmacComparer.Compare(newHmac, hmacBytes, hmacProvider.HmacLength);
                    if (hmacValid)
                    {
                        var sessionIdString = encryptionProvider.Decrypt(encryptedCookie);
                        Guid.TryParse(sessionIdString, out sessionId);
                    }
                }
            }

            sessionStore.Save(sessionId, context.Request.Session, context.Response);
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