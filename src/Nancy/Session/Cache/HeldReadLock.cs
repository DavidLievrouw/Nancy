namespace Nancy.Session.Cache
{
    using System.Threading;

    internal class HeldReadLock : HeldLock
    {
        public HeldReadLock(ReaderWriterLockSlim wrappedLock) : base(wrappedLock)
        {
        }

        public override void Acquire()
        {
            this.wrappedLock.EnterReadLock();
        }

        public override void Dispose()
        {
            if (this.wrappedLock.IsReadLockHeld) this.wrappedLock.ExitReadLock();
        }
    }
}