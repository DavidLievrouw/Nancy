namespace Nancy.Session.InProcSessionsManagement.Cache
{
    using System.Threading;

    internal class HeldUpgradeableReadLock : HeldLock
    {
        public HeldUpgradeableReadLock(ReaderWriterLockSlim wrappedLock) : base(wrappedLock)
        {
        }

        public override void Acquire()
        {
            this.wrappedLock.EnterUpgradeableReadLock();
        }

        public override void Dispose()
        {
            if (this.wrappedLock.IsUpgradeableReadLockHeld) this.wrappedLock.ExitUpgradeableReadLock();
        }
    }
}