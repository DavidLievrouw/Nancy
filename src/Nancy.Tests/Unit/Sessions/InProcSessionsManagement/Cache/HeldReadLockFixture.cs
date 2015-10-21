namespace Nancy.Tests.Unit.Sessions.InProcSessionsManagement.Cache
{
    using System;
    using System.Threading;
    using Nancy.Session.InProcSessionsManagement.Cache;
    using Xunit;

    public class HeldReadLockFixture : IDisposable
    {
        private readonly ReaderWriterLockSlim wrappedLock;

        public HeldReadLockFixture()
        {
            this.wrappedLock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
        }

        public void Dispose()
        {
            this.wrappedLock.Dispose();
        }

        [Fact]
        public void Given_null_readerwriterlock_then_throws()
        {
            Assert.Throws<ArgumentNullException>(() => new HeldReadLock(null));
        }

        public void When_creating_then_acquires_lock()
        {
            using (new HeldReadLock(this.wrappedLock))
            {
                var actual = this.wrappedLock.IsReadLockHeld;
                Assert.True(actual);
            }
        }

        public void When_disposing_then_releases_lock()
        {
            using (new HeldReadLock(this.wrappedLock))
            {
            }

            var actual = this.wrappedLock.IsReadLockHeld;
            Assert.False(actual);
        }
    }
}