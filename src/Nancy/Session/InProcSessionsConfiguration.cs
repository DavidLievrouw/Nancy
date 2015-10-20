namespace Nancy.Session
{
    using System;
    using Nancy.Cryptography;

    /// <summary>
    /// Configuration options for memory based sessions
    /// </summary>
    public class InProcSessionsConfiguration
    {
        internal const string DefaultCookieName = "_nsid";

        /// <summary>
        /// Initializes a new instance of the <see cref="InProcSessionsConfiguration"/> class.
        /// </summary>
        public InProcSessionsConfiguration() : this(CryptographyConfiguration.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InProcSessionsConfiguration"/> class.
        /// </summary>
        public InProcSessionsConfiguration(CryptographyConfiguration cryptographyConfiguration)
        {
            this.CryptographyConfiguration = cryptographyConfiguration;
            this.CookieName = DefaultCookieName;
            this.SessionTimeout = TimeSpan.FromMinutes(20);
        }

        /// <summary>
        /// Gets or sets the cryptography configuration
        /// </summary>
        public CryptographyConfiguration CryptographyConfiguration { get; set; }

        /// <summary>
        /// Gets or sets the time after which a memory session expires
        /// </summary>
        public TimeSpan SessionTimeout { get; set; }

        /// <summary>
        /// Cookie name for storing session id
        /// </summary>
        public string CookieName { get; set; }

        /// <summary>
        /// Gets or sets the domain of the session cookie
        /// </summary>
        public string Domain { get; set; }

        /// <summary>
        /// Gets or sets the path of the session cookie
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Gets a value indicating whether the configuration is valid or not.
        /// </summary>
        public virtual bool IsValid
        {
            get
            {
                if (string.IsNullOrEmpty(this.CookieName))
                {
                    return false;
                }

                if (this.CryptographyConfiguration == null)
                {
                    return false;
                }

                if (this.CryptographyConfiguration.EncryptionProvider == null)
                {
                    return false;
                }

                if (this.CryptographyConfiguration.HmacProvider == null)
                {
                    return false;
                }

                if (this.SessionTimeout <= TimeSpan.Zero)
                {
                    return false;
                }

                return true;
            }
        }

        public static InProcSessionsConfiguration Default = new InProcSessionsConfiguration();
    }
}