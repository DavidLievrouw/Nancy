namespace Nancy.Session.InProcSessionsManagement.ByQueryStringParam
{
    using System;

    /// <summary>
    /// Identification method for in-process memory based sessions, using a querystring parameter, that contains the session identifier.
    /// </summary>
    public class ByQueryStringParamIdentificationMethod : IByQueryStringParamIdentificationMethod
    {
        const string DefaultParameterName = "_nsid";

        /// <summary>
        /// Initializes a new instance of the <see cref="ByQueryStringParamIdentificationMethod"/> class.
        /// </summary>
        public ByQueryStringParamIdentificationMethod()
        {
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


            // Id it doesn't contain a session id, then create a new session id
            throw new NotImplementedException();
        }

        /// <summary>
        /// Save the session in the specified context.
        /// </summary>
        /// <param name="sessionId">The identifier of the session.</param>
        /// <param name="context">The current context.</param>
        /// <returns>The early exit response, or null, if everything is OK.</returns>
        public Response SaveSessionId(Guid sessionId, NancyContext context)
        {
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