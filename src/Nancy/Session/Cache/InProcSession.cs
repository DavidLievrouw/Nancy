namespace Nancy.Session.Cache
{
    using System;

    internal class InProcSession
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InProcSession"/> class.
        /// </summary>
        /// <param name="wrappedSession">The real Nancy session to wrap.</param>
        /// <param name="creationUtc">The UTC time when the session was created.</param>
        /// <param name="timeout">The time after which the session should expire.</param>
        public InProcSession(ISession wrappedSession, DateTime creationUtc, TimeSpan timeout) : this(Guid.NewGuid(), wrappedSession, creationUtc, timeout)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InProcSession"/> class.
        /// </summary>
        /// <param name="id">The unique session identifier.</param>
        /// <param name="wrappedSession">The real Nancy session to wrap.</param>
        /// <param name="creationUtc">The UTC time when the session was created.</param>
        /// <param name="timeout">The time after which the session should expire.</param>
        internal InProcSession(Guid id, ISession wrappedSession, DateTime creationUtc, TimeSpan timeout)
        {
            if (wrappedSession == null) throw new ArgumentNullException(nameof(wrappedSession));
            this.Id = id;
            this.WrappedSession = wrappedSession;
            this.CreationUtc = creationUtc;
            this.Timeout = timeout;
        }

        /// <summary>
        /// Gets the unique identifier of the session.
        /// </summary>
        public Guid Id { get; private set; }

        /// <summary>
        /// The real Nancy session that is wrapped.
        /// </summary>
        public ISession WrappedSession { get; private set; }

        /// <summary>
        /// The UTC time when the session was created.
        /// </summary>
        public DateTime CreationUtc { get; private set; }

        /// <summary>
        /// The time after which the session should expire.
        /// </summary>
        public TimeSpan Timeout { get; private set; }

        /// <summary>
        /// Checks whether this session has expired.
        /// </summary>
        /// <param name="nowUtc">The current UTC time.</param>
        /// <returns>True if the session has expired, otherwise false.</returns>
        public bool IsExpired(DateTime nowUtc)
        {
            return nowUtc > this.CreationUtc.Add(this.Timeout);
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            var otherSession = obj as InProcSession;
            if (otherSession == null) return false;

            return this.Id == otherSession.Id;
        }

        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }
    }
}