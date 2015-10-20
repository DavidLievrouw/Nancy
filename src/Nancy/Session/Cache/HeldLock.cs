namespace Nancy.Session.Cache
{
    using System;
    using System.Threading;

    internal abstract class HeldLock : IHeldLock
    {
        protected readonly ReaderWriterLockSlim wrappedLock;

        public HeldLock(ReaderWriterLockSlim wrappedLock)
        {
            if (wrappedLock == null) throw new ArgumentNullException("wrappedLock");
            this.wrappedLock = wrappedLock;
            this.Acquire();
        }

        public abstract void Dispose();

        public abstract void Acquire();
    }
}