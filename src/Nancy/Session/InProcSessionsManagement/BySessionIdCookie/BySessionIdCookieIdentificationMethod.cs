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
        private readonly ISessionIdFactory sessionIdFactory;
        private readonly ICookieFactory cookieFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="BySessionIdCookieIdentificationMethod"/> class.
        /// </summary>
        public BySessionIdCookieIdentificationMethod(InProcSessionsConfiguration configuration) : this(
            configuration, 
            new CookieDataProvider(), 
            new HmacValidator(), 
            new SessionIdFactory(),
            new CookieFactory())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BySessionIdCookieIdentificationMethod"/> class.
        /// </summary>
        internal BySessionIdCookieIdentificationMethod(
            InProcSessionsConfiguration configuration,
            ICookieDataProvider cookieDataProvider,
            IHmacValidator hmacValidator,
            ISessionIdFactory sessionIdFactory,
            ICookieFactory cookieFactory)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));
            if (cookieDataProvider == null) throw new ArgumentNullException(nameof(cookieDataProvider));
            if (hmacValidator == null) throw new ArgumentNullException(nameof(hmacValidator));
            if (sessionIdFactory == null) throw new ArgumentNullException(nameof(sessionIdFactory));
            if (cookieFactory == null) throw new ArgumentNullException(nameof(cookieFactory));
            this.configuration = configuration;
            this.cookieDataProvider = cookieDataProvider;
            this.hmacValidator = hmacValidator;
            this.sessionIdFactory = sessionIdFactory;
            this.cookieFactory = cookieFactory;
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

            var encryptionProvider = this.configuration.CryptographyConfiguration.EncryptionProvider;
            var decryptedSessionId = encryptionProvider.Decrypt(cookieData.SessionId);
            if (string.IsNullOrEmpty(decryptedSessionId)) return this.sessionIdFactory.CreateNew();

            return this.sessionIdFactory.CreateFrom(decryptedSessionId) ?? this.sessionIdFactory.CreateNew();
        }

        /// <summary>
        /// Save the session in the specified context.
        /// </summary>
        /// <param name="sessionId">The identifier of the session.</param>
        /// <param name="context">The current context.</param>
        public void SaveSessionId(Guid sessionId, NancyContext context)
        {
            if (context == null) throw new ArgumentNullException("context");
            if (context.Response == null) throw new ArgumentException("The specified context does not contain a response to modify", "context");
            if (sessionId == Guid.Empty) throw new ArgumentException("The specified session id cannot be empty", "sessionId");

            var encryptionProvider = this.configuration.CryptographyConfiguration.EncryptionProvider;
            var encryptedSessionId = encryptionProvider.Encrypt(sessionId.ToString());

            var hmacProvider = this.configuration.CryptographyConfiguration.HmacProvider;
            var hmacBytes = hmacProvider.GenerateHmac(encryptedSessionId);

            var cookieData = new CookieData
            {
                SessionId = encryptedSessionId,
                Hmac = hmacBytes
            };

            var cookie = this.cookieFactory.CreateCookie(cookieData);
            context.Response.WithCookie(cookie);
        }
    }
}