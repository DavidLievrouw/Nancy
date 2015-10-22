namespace Nancy.Session.InProcSessionsManagement
{
    using System;

    /// <summary>
    /// Configuration options for in-process memory based sessions identification method
    /// </summary>
    public class InProcSessionIdentificationMethodConfiguration
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InProcSessionIdentificationMethodConfiguration"/> class.
        /// </summary>
        public InProcSessionIdentificationMethodConfiguration() : this(new BySessionIdCookieIdentificationMethod())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InProcSessionIdentificationMethodConfiguration"/> class.
        /// </summary>
        public InProcSessionIdentificationMethodConfiguration(IInProcSessionIdentificationMethod sessionIdentificationMethod)
        {
            if (sessionIdentificationMethod == null) throw new ArgumentNullException("sessionIdentificationMethod");
            this.SessionIdentificationMethod = sessionIdentificationMethod;
        }

        /// <summary>
        /// Gets or sets the method that is used to identify the session from the context
        /// </summary>
        public IInProcSessionIdentificationMethod SessionIdentificationMethod {
            get;
            set;
        }

        /// <summary>
        /// An instance of the <see cref="InProcSessionsConfiguration"/> class, using its default values. 
        /// </summary>
        public static readonly InProcSessionIdentificationMethodConfiguration Default =
            new InProcSessionIdentificationMethodConfiguration();
    }
}