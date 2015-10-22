using System;

namespace Nancy.Session.InProcSessionsManagement
{
    public class BySessionIdCookieIdentificationMethod : IInProcSessionIdentificationMethod
    {
        internal const string DefaultCookieName = "_nsid";

        public BySessionIdCookieIdentificationMethod()
        {
            this.CookieName = DefaultCookieName;
        }

        /// <summary>
        /// Cookie name for storing session id
        /// </summary>
        public string CookieName {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the domain of the session cookie
        /// </summary>
        public string Domain {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the path of the session cookie
        /// </summary>
        public string Path {
            get;
            set;
        }

        public ISession LoadSession(NancyContext context)
        {
            throw new NotImplementedException();
        }

        public void SaveSession(NancyContext context)
        {
            throw new NotImplementedException();
        }
    }
}