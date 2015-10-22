namespace Nancy.Session
{
    using System;
    using Nancy.Cryptography;
    using Nancy.Session.InProcSessionsManagement;

    /// <summary>
    /// Configuration options for in-process memory based sessions
    /// </summary>
    public class InProcSessionsConfiguration
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InProcSessionsConfiguration"/> class.
        /// </summary>
        public InProcSessionsConfiguration()
            : this(CryptographyConfiguration.Default, new BySessionIdCookieIdentificationMethod())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InProcSessionsConfiguration"/> class.
        /// </summary>
        public InProcSessionsConfiguration(CryptographyConfiguration cryptographyConfiguration)
            : this(cryptographyConfiguration, new BySessionIdCookieIdentificationMethod())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InProcSessionsConfiguration"/> class.
        /// </summary>
        public InProcSessionsConfiguration(CryptographyConfiguration cryptographyConfiguration,
             IInProcSessionIdentificationMethod sessionIdentificationMethod)
        {
            this.CryptographyConfiguration = cryptographyConfiguration;
            this.SessionIdentificationMethod = sessionIdentificationMethod;
            this.SessionTimeout = TimeSpan.FromMinutes(20);
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
        /// Gets a value indicating whether the configuration is valid or not.
        /// </summary>
        public virtual bool IsValid {
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

                return true;
            }
        }

        /// <summary>
        /// An instance of the <see cref="InProcSessionsConfiguration"/> class, using its default values. 
        /// </summary>
        public static readonly InProcSessionsConfiguration Default = new InProcSessionsConfiguration();
    }
}