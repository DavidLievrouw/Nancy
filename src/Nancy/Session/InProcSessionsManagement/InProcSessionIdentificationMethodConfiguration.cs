namespace Nancy.Session.InProcSessionsManagement
{
    using System;

    public class InProcSessionIdentificationMethodConfiguration
    {
        public InProcSessionIdentificationMethodConfiguration() : this(new BySessionIdCookieIdentificationMethod())
        {
        }

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

        public static InProcSessionIdentificationMethodConfiguration Default =
            new InProcSessionIdentificationMethodConfiguration();
    }
}