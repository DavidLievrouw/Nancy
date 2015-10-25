namespace Nancy.Session.InProcSessionsManagement.ByQueryStringParam
{
    using System;
    using Nancy.Cryptography;

    /// <summary>
    /// Identification method for in-process memory based sessions, using a querystring parameter, that contains the session identifier.
    /// </summary>
    public class ByQueryStringParamIdentificationMethod : IByQueryStringParamIdentificationMethod
    {
        private const string DefaultParameterName = "_nsid";
        private readonly IEncryptionProvider encryptionProvider;
        private readonly IHmacProvider hmacProvider;
        private readonly IHmacValidator hmacValidator;
        private readonly ISessionIdentificationDataProvider sessionIdentificationDataProvider;
        private readonly ISessionIdFactory sessionIdFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="ByQueryStringParamIdentificationMethod"/> class.
        /// </summary>
        public ByQueryStringParamIdentificationMethod(CryptographyConfiguration cryptoConfig)
        {
            if (cryptoConfig == null) throw new ArgumentNullException("cryptoConfig");
            this.encryptionProvider = cryptoConfig.EncryptionProvider;
            this.hmacProvider = cryptoConfig.HmacProvider;
            this.sessionIdentificationDataProvider = new SessionIdentificationDataProvider(this.hmacProvider, this);
            this.hmacValidator = new HmacValidator(this.hmacProvider);
            this.sessionIdFactory = new SessionIdFactory();
            this.ParameterName = DefaultParameterName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ByQueryStringParamIdentificationMethod"/> class.
        /// </summary>
        internal ByQueryStringParamIdentificationMethod(
            IEncryptionProvider encryptionProvider,
            IHmacProvider hmacProvider,
            ISessionIdentificationDataProvider sessionIdentificationDataProvider,
            IHmacValidator hmacValidator,
            ISessionIdFactory sessionIdFactory)
        {
            if (encryptionProvider == null) throw new ArgumentNullException("encryptionProvider");
            if (hmacProvider == null) throw new ArgumentNullException("hmacProvider");
            if (sessionIdentificationDataProvider == null) throw new ArgumentNullException("configuration");
            if (hmacValidator == null) throw new ArgumentNullException("configuration");
            if (sessionIdFactory == null) throw new ArgumentNullException("configuration");
            this.encryptionProvider = encryptionProvider;
            this.hmacProvider = hmacProvider;
            this.sessionIdentificationDataProvider = sessionIdentificationDataProvider;
            this.hmacValidator = hmacValidator;
            this.sessionIdFactory = sessionIdFactory;
            this.ParameterName = DefaultParameterName;
        }

        /// <summary>
        /// Load the session identifier from the specified context.
        /// </summary>
        /// <param name="context">The current context.</param>
        /// <returns>The identifier of the session for the current request.</returns>
        public Guid GetCurrentSessionId(NancyContext context)
        {
            if (context == null) throw new ArgumentNullException("context");

            var queryStringData = this.sessionIdentificationDataProvider.ProvideDataFromQuery(context.Request);
            if (queryStringData == null) return this.sessionIdFactory.CreateNew();
            var isHmacValid = this.hmacValidator.IsValidHmac(queryStringData);
            if (!isHmacValid) return this.sessionIdFactory.CreateNew();

            var decryptedSessionId = this.encryptionProvider.Decrypt(queryStringData.SessionId);
            if (string.IsNullOrEmpty(decryptedSessionId)) return this.sessionIdFactory.CreateNew();

            return this.sessionIdFactory.CreateFrom(decryptedSessionId) ?? this.sessionIdFactory.CreateNew();
        }

        /// <summary>
        /// Save the session in the specified context.
        /// </summary>
        /// <param name="sessionId">The identifier of the session.</param>
        /// <param name="context">The current context.</param>
        /// <returns>The early exit response, or null, if everything is OK.</returns>
        public Response SaveSessionId(Guid sessionId, NancyContext context)
        {
            if (context == null) throw new ArgumentNullException("context");
            if (context.Request == null) throw new ArgumentException("The specified context does not contain a request", "context");
            if (sessionId == Guid.Empty) throw new ArgumentException("The specified session id cannot be empty", "sessionId");

            // If it doesn't contain a session id, then replace the response with 302 and add location header
            // Otherwise, do nothing
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets or sets the querystring parameter name in which the session id is stored.
        /// </summary>
        public string ParameterName { get; set; }
    }
}