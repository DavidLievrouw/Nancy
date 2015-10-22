using System;

namespace Nancy.Session.InProcSessionsManagement
{
    /// <summary>
    /// Identification method for in-process memory based sessions, using a cookie that contains the session identifier.
    /// </summary>
    public class BySessionIdCookieIdentificationMethod : IInProcSessionIdentificationMethod
    {
        internal const string DefaultCookieName = "_nsid";

        /// <summary>
        /// Initializes a new instance of the <see cref="BySessionIdCookieIdentificationMethod"/> class.
        /// </summary>
        public BySessionIdCookieIdentificationMethod()
        {
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
            throw new NotImplementedException();
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