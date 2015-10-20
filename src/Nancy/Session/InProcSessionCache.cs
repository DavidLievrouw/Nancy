﻿namespace Nancy.Session
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;

    /// <summary>
    /// Cache object that holds the saved sessions.
    /// </summary>
    internal class InProcSessionCache : IEnumerable<InProcSession>, IDisposable
    {
        private readonly ReaderWriterLockSlim rwlock;
        private readonly List<InProcSession> sessions;
        private bool isDisposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="InProcSessionCache"/> class.
        /// </summary>
        public InProcSessionCache()
        {
            this.sessions = new List<InProcSession>();
            this.rwlock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
        }

        /// <summary>
        /// Gets the number of sessions that are currently held in cache.
        /// </summary>
        public int Count
        {
            get
            {
                this.CheckDisposed();
                try
                {
                    this.rwlock.EnterReadLock();
                    return this.sessions.Count;
                }
                finally
                {
                    if (this.rwlock.IsReadLockHeld) this.rwlock.ExitReadLock();
                }
            }
        }

        /// <summary>
        /// Clean resources that are held by this object.
        /// </summary>
        public void Dispose()
        {
            this.sessions.Clear();
            this.rwlock.Dispose();
            this.isDisposed = true;
        }

        public IEnumerator<InProcSession> GetEnumerator()
        {
            this.CheckDisposed();
            return this.sessions.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            this.CheckDisposed();
            return this.GetEnumerator();
        }

        /// <summary>
        /// Add a new item to the cache.
        /// </summary>
        /// <param name="session">The item to add.</param>
        public void Set(InProcSession session)
        {
            if (session == null) throw new ArgumentNullException("session");
            this.CheckDisposed();

            try
            {
                this.rwlock.EnterWriteLock();
                if (!this.sessions.Contains(session)) this.sessions.Add(session);
            }
            finally
            {
                if (this.rwlock.IsWriteLockHeld) this.rwlock.ExitWriteLock();
            }
        }

        /// <summary>
        /// Gets the session with the specified identifier from the cache.
        /// </summary>
        /// <param name="id">The identifier of the session.</param>
        /// <returns>The session with the specified identifier, or null, if none was not found.</returns>
        public InProcSession Get(Guid id)
        {
            this.CheckDisposed();

            try
            {
                this.rwlock.EnterUpgradeableReadLock();

                var foundSession = this.sessions.SingleOrDefault(session => session.Id == id);

                // CQS violation, for convenience
                if (foundSession != null && foundSession.IsExpired(SystemClockAmbientContext.Current.NowUtc))
                {
                    try
                    {
                        this.rwlock.EnterWriteLock();
                        this.sessions.Remove(foundSession);
                        foundSession = null;
                    }
                    finally
                    {
                        if (this.rwlock.IsWriteLockHeld) this.rwlock.ExitWriteLock();
                    }
                }

                return foundSession;
            }
            finally
            {
                if (this.rwlock.IsUpgradeableReadLockHeld) this.rwlock.ExitUpgradeableReadLock();
            }
        }

        private void CheckDisposed()
        {
            if (this.isDisposed) throw new ObjectDisposedException(this.GetType().Name);
        }
    }
}