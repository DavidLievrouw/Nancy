namespace Nancy.Session.InProcSessionsManagement.Cache
{
    using System;

    internal class InProcSessionWrapper
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InProcSessionWrapper"/> class.
        /// </summary>
        /// <param name="id">The unique session identifier.</param>
        /// <param name="wrappedSession">The real Nancy session to wrap.</param>
        /// <param name="lastSave">The UTC time when the session was created.</param>
        /// <param name="timeout">The time after which the session should expire.</param>
        public InProcSessionWrapper(Guid id, ISession wrappedSession, DateTime lastSave, TimeSpan timeout)
        {
            if (wrappedSession == null) throw new ArgumentNullException("wrappedSession");
            if (id == Guid.Empty) throw new ArgumentException("The session Id cannot be empty.", "id");
            this.Id = id;
            this.WrappedSession = wrappedSession;
            this.LastSave = lastSave;
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
        /// The UTC time when the session was last saved.
        /// </summary>
        public DateTime LastSave { get; private set; }

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
            return nowUtc > this.LastSave.Add(this.Timeout);
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            var otherSession = obj as InProcSessionWrapper;
            if (otherSession == null) return false;

            return this.Id == otherSession.Id;
        }

        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }
    }
}