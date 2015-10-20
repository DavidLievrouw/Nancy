namespace Nancy.Session.Cache
{
    using System.Threading;

    internal class HeldWriteLock : HeldLock
    {
        public HeldWriteLock(ReaderWriterLockSlim wrappedLock) : base(wrappedLock)
        {
        }

        public override void Acquire()
        {
            this.wrappedLock.EnterWriteLock();
        }

        public override void Dispose()
        {
            if (this.wrappedLock.IsWriteLockHeld) this.wrappedLock.ExitWriteLock();
        }
    }
}