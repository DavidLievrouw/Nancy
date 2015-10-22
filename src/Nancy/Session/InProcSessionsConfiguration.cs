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
            : this(CryptographyConfiguration.Default, InProcSessionIdentificationMethodConfiguration.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InProcSessionsConfiguration"/> class.
        /// </summary>
        public InProcSessionsConfiguration(CryptographyConfiguration cryptographyConfiguration)
            : this(cryptographyConfiguration, InProcSessionIdentificationMethodConfiguration.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InProcSessionsConfiguration"/> class.
        /// </summary>
        public InProcSessionsConfiguration(CryptographyConfiguration cryptographyConfiguration,
            InProcSessionIdentificationMethodConfiguration sessionIdentificationMethodConfiguration)
        {
            this.CryptographyConfiguration = cryptographyConfiguration;
            this.SessionIdentificationMethodConfiguration = sessionIdentificationMethodConfiguration;
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
        /// Gets or sets the session identification method configuration
        /// </summary>
        public InProcSessionIdentificationMethodConfiguration SessionIdentificationMethodConfiguration {
            get;
            set;
        }
        
        /// <summary>
        /// Gets or sets the time after which a memory session expires
        /// </summary>
        public TimeSpan SessionTimeout
        {
            get;
            set;
        }

        /// <summary>
        /// Gets a value indicating whether the configuration is valid or not.
        /// </summary>
        public virtual bool IsValid {
            get {
                if (this.SessionIdentificationMethodConfiguration == null) {
                    return false;
                }

                if (this.SessionIdentificationMethodConfiguration.SessionIdentificationMethod == null) {
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

        public static InProcSessionsConfiguration Default = new InProcSessionsConfiguration();
    }
}