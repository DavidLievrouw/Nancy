namespace Nancy.Session.InProcSessionsManagement.BySessionIdCookie
{
    using System;
    using Nancy.Cryptography;

    /// <summary>
    /// Identification method for in-process memory based sessions, using a cookie that contains the session identifier.
    /// </summary>
    public class BySessionIdCookieIdentificationMethod : IBySessionIdCookieIdentificationMethod
    {
        const string DefaultCookieName = "_nsid";
        private readonly IEncryptionProvider encryptionProvider;
        private readonly IHmacProvider hmacProvider;
        private readonly ISessionIdentificationDataProvider sessionIdentificationDataProvider;
        private readonly IHmacValidator hmacValidator;
        private readonly ISessionIdFactory sessionIdFactory;
        private readonly ICookieFactory cookieFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="BySessionIdCookieIdentificationMethod"/> class.
        /// </summary>
        public BySessionIdCookieIdentificationMethod(CryptographyConfiguration cryptoConfig)
        {
            if (cryptoConfig == null) throw new ArgumentNullException("cryptoConfig");
            this.encryptionProvider = cryptoConfig.EncryptionProvider;
            this.hmacProvider = cryptoConfig.HmacProvider;
            this.sessionIdentificationDataProvider = new SessionIdentificationDataProvider(this.hmacProvider, this);
            this.hmacValidator = new HmacValidator(this.hmacProvider);
            this.sessionIdFactory = new SessionIdFactory();
            this.cookieFactory = new CookieFactory(this);
            this.CookieName = DefaultCookieName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BySessionIdCookieIdentificationMethod"/> class.
        /// </summary>
        internal BySessionIdCookieIdentificationMethod(
            IEncryptionProvider encryptionProvider,
            IHmacProvider hmacProvider,
            ISessionIdentificationDataProvider sessionIdentificationDataProvider,
            IHmacValidator hmacValidator,
            ISessionIdFactory sessionIdFactory,
            ICookieFactory cookieFactory)
        {
            if (encryptionProvider == null) throw new ArgumentNullException("encryptionProvider");
            if (hmacProvider == null) throw new ArgumentNullException("hmacProvider");
            if (sessionIdentificationDataProvider == null) throw new ArgumentNullException("configuration");
            if (hmacValidator == null) throw new ArgumentNullException("configuration");
            if (sessionIdFactory == null) throw new ArgumentNullException("configuration");
            if (cookieFactory == null) throw new ArgumentNullException("configuration");
            this.encryptionProvider = encryptionProvider;
            this.hmacProvider = hmacProvider;
            this.sessionIdentificationDataProvider = sessionIdentificationDataProvider;
            this.hmacValidator = hmacValidator;
            this.sessionIdFactory = sessionIdFactory;
            this.cookieFactory = cookieFactory;
            this.CookieName = DefaultCookieName;
        }

        /// <summary>
        /// Gets or sets the cookie name in which the session id is stored.
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
        public SessionId GetCurrentSessionId(NancyContext context)
        {
            if (context == null) throw new ArgumentNullException("context");

            var cookieData = this.sessionIdentificationDataProvider.ProvideDataFromCookie(context.Request);
            if (cookieData == null) return this.sessionIdFactory.CreateNew();
            var isHmacValid = this.hmacValidator.IsValidHmac(cookieData);
            if (!isHmacValid) return this.sessionIdFactory.CreateNew();
            
            var decryptedSessionId = this.encryptionProvider.Decrypt(cookieData.SessionId);
            if (string.IsNullOrEmpty(decryptedSessionId)) return this.sessionIdFactory.CreateNew();

            return this.sessionIdFactory.CreateFrom(decryptedSessionId) ?? this.sessionIdFactory.CreateNew();
        }

        /// <summary>
        /// Save the session in the specified context.
        /// </summary>
        /// <param name="sessionId">The identifier of the session.</param>
        /// <param name="context">The current context.</param>
        /// <returns>The early exit response, or null, if everything is OK.</returns>
        public Response SaveSessionId(SessionId sessionId, NancyContext context)
        {
            if (sessionId == null) throw new ArgumentNullException("sessionId");
            if (context == null) throw new ArgumentNullException("context");
            if (context.Response == null) throw new ArgumentException("The specified context does not contain a response to modify", "context");
            if (sessionId.IsEmpty) throw new ArgumentException("The specified session id cannot be empty", "sessionId");
            
            var encryptedSessionId = this.encryptionProvider.Encrypt(sessionId.ToString());
            var hmacBytes = this.hmacProvider.GenerateHmac(encryptedSessionId);

            var cookieData = new SessionIdentificationData
            {
                SessionId = encryptedSessionId,
                Hmac = hmacBytes
            };

            var cookie = this.cookieFactory.CreateCookie(cookieData);
            context.Response.WithCookie(cookie);

            return null;
        }
    }
}