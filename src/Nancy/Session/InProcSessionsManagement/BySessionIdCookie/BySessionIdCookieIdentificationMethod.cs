namespace Nancy.Session.InProcSessionsManagement.BySessionIdCookie
{
    using System;

    /// <summary>
    /// Identification method for in-process memory based sessions, using a cookie that contains the session identifier.
    /// </summary>
    public class BySessionIdCookieIdentificationMethod : IInProcSessionIdentificationMethod
    {
        internal const string DefaultCookieName = "_nsid";
        private readonly InProcSessionsConfiguration configuration;
        private readonly ICookieDataProvider cookieDataProvider;
        private readonly IHmacValidator hmacValidator;
        private readonly ICookieDataDecrypter cookieDataDecrypter;
        private readonly ISessionIdFactory sessionIdFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="BySessionIdCookieIdentificationMethod"/> class.
        /// </summary>
        public BySessionIdCookieIdentificationMethod(InProcSessionsConfiguration configuration) : this(
            configuration, 
            new CookieDataProvider(), 
            new HmacValidator(), 
            new CookieDataDecrypter(),
            new SessionIdFactory())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BySessionIdCookieIdentificationMethod"/> class.
        /// </summary>
        internal BySessionIdCookieIdentificationMethod(
            InProcSessionsConfiguration configuration,
            ICookieDataProvider cookieDataProvider,
            IHmacValidator hmacValidator,
            ICookieDataDecrypter cookieDataDecrypter,
            ISessionIdFactory sessionIdFactory)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));
            if (cookieDataProvider == null) throw new ArgumentNullException(nameof(cookieDataProvider));
            if (hmacValidator == null) throw new ArgumentNullException(nameof(hmacValidator));
            if (cookieDataDecrypter == null) throw new ArgumentNullException(nameof(cookieDataDecrypter));
            if (sessionIdFactory == null) throw new ArgumentNullException(nameof(sessionIdFactory));
            this.configuration = configuration;
            this.cookieDataProvider = cookieDataProvider;
            this.hmacValidator = hmacValidator;
            this.cookieDataDecrypter = cookieDataDecrypter;
            this.sessionIdFactory = sessionIdFactory;
            this.CookieName = DefaultCookieName;
        }

        /// <summary>
        /// Cookie name for storing session id.
        /// </summary>
        public string CookieName {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the domain of the session cookie.
        /// </summary>
        public string Domain {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the path of the session cookie.
        /// </summary>
        public string Path {
            get;
            set;
        }

        /// <summary>
        /// Load the session identifier from the specified context.
        /// </summary>
        /// <param name="context">The current context.</param>
        /// <returns>The identifier of the session for the current request.</returns>
        public Guid GetCurrentSessionId(NancyContext context)
        {
            if (context == null) throw new ArgumentNullException("context");

            var cookieData = this.cookieDataProvider.ProvideCookieData(context.Request);
            if (cookieData == null) return this.sessionIdFactory.CreateNew();
            var isHmacValid = this.hmacValidator.IsValidHmac(cookieData);
            if (!isHmacValid) return this.sessionIdFactory.CreateNew();

            var decryptedSessionId = this.cookieDataDecrypter.DecryptCookieData(cookieData.SessionId);
            if (decryptedSessionId == null) return this.sessionIdFactory.CreateNew();

            return this.sessionIdFactory.CreateFrom(decryptedSessionId) ?? this.sessionIdFactory.CreateNew();
        }

        /// <summary>
        /// Save the session in the specified context.
        /// </summary>
        /// <param name="sessionId">The identifier of the session.</param>
        /// <param name="context">The current context.</param>
        public void SaveSessionId(Guid sessionId, NancyContext context)
        {
            throw new NotImplementedException();
        }
    }
}