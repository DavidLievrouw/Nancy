namespace Nancy.Session
{
    using System;
    using Nancy.Cryptography;
    using Nancy.Session.InProcSessionsManagement;
    using Nancy.Session.InProcSessionsManagement.BySessionIdCookie;

    /// <summary>
    /// Configuration options for in-process memory based sessions
    /// </summary>
    public class InProcSessionsConfiguration
    {
        private const int DefaultSessionTimeoutMinutes = 20;
        private const int DefaultCacheTrimIntervalMinutes = 30;

        /// <summary>
        /// Initializes a new instance of the <see cref="InProcSessionsConfiguration"/> class.
        /// </summary>
        public InProcSessionsConfiguration()
        {
            this.CryptographyConfiguration = CryptographyConfiguration.Default;
            this.SessionIdentificationMethod = new BySessionIdCookieIdentificationMethod(this);
            this.SessionTimeout = TimeSpan.FromMinutes(DefaultSessionTimeoutMinutes);
            this.CacheTrimInterval = TimeSpan.FromMinutes(DefaultCacheTrimIntervalMinutes);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InProcSessionsConfiguration"/> class.
        /// </summary>
        public InProcSessionsConfiguration(CryptographyConfiguration cryptographyConfiguration)
        {
            this.CryptographyConfiguration = cryptographyConfiguration;
            this.SessionIdentificationMethod = new BySessionIdCookieIdentificationMethod(this);
            this.SessionTimeout = TimeSpan.FromMinutes(DefaultSessionTimeoutMinutes);
            this.CacheTrimInterval = TimeSpan.FromMinutes(DefaultCacheTrimIntervalMinutes);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InProcSessionsConfiguration"/> class.
        /// </summary>
        public InProcSessionsConfiguration(CryptographyConfiguration cryptographyConfiguration,
             IInProcSessionIdentificationMethod sessionIdentificationMethod)
        {
            this.CryptographyConfiguration = cryptographyConfiguration;
            this.SessionIdentificationMethod = sessionIdentificationMethod;
            this.SessionTimeout = TimeSpan.FromMinutes(DefaultSessionTimeoutMinutes);
            this.CacheTrimInterval = TimeSpan.FromMinutes(DefaultCacheTrimIntervalMinutes);
        }

        /// <summary>
        /// Gets or sets the cryptography configuration
        /// </summary>
        public CryptographyConfiguration CryptographyConfiguration {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the method that is used to identify the session from the context
        /// </summary>
        public IInProcSessionIdentificationMethod SessionIdentificationMethod
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the time after which a memory session expires
        /// </summary>
        public TimeSpan SessionTimeout {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the time after which a the expired sessions are cleaned up.
        /// </summary>
        public TimeSpan CacheTrimInterval
        {
            get;
            set;
        }

        /// <summary>
        /// Gets a value indicating whether the configuration is valid or not.
        /// </summary>
        public bool IsValid {
            get {
                if (this.SessionIdentificationMethod == null) {
                    return false;
                }

                if (this.CryptographyConfiguration == null) {
                    return false;
                }

                if (this.CryptographyConfiguration.EncryptionProvider == null) {
                    return false;
                }

                if (this.CryptographyConfiguration.HmacProvider == null) {
                    return false;
                }

                if (this.SessionTimeout <= TimeSpan.Zero) {
                    return false;
                }

                if (this.CacheTrimInterval < TimeSpan.Zero)
                {
                    return false;
                }

                return true;
            }
        }

        /// <summary>
        /// An instance of the <see cref="InProcSessionsConfiguration"/> class, using its default values. 
        /// </summary>
        public static readonly InProcSessionsConfiguration Default = new InProcSessionsConfiguration();
    }
}