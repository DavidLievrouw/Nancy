namespace Nancy.Tests.Unit.Sessions
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using FakeItEasy;
    using Nancy.Session;
    using Xunit;

    public class InProcSessionCacheFixture : IDisposable
    {
        private readonly InProcSession activeSession;
        private readonly InProcSession expiredSession;
        private readonly ISystemClock fakeSystemClock;
        private readonly InProcSessionCache inProcSessionCache;
        private readonly DateTime nowUtc;
        private readonly int numberOfSessions;

        public InProcSessionCacheFixture()
        {
            this.nowUtc = new DateTime(2015, 10, 20, 21, 36, 14, DateTimeKind.Utc);
            this.fakeSystemClock = A.Fake<ISystemClock>();
            this.ConfigureSystemClock_ToReturn(this.nowUtc);
            SystemClockAmbientContext.Current = this.fakeSystemClock;

            this.inProcSessionCache = new InProcSessionCache();
            this.expiredSession = new InProcSession(A.Dummy<ISession>(), this.nowUtc.AddMinutes(-20),
                TimeSpan.FromMinutes(15));
            this.activeSession = new InProcSession(A.Dummy<ISession>(), this.nowUtc.AddMinutes(-3),
                TimeSpan.FromMinutes(15));
            this.inProcSessionCache.Set(this.expiredSession);
            this.inProcSessionCache.Set(this.activeSession);
            this.numberOfSessions = 2;
        }

        public void Dispose()
        {
            this.inProcSessionCache.Dispose();
        }

        [Fact]
        public void Count_returns_number_of_elements()
        {
            var actual = inProcSessionCache.Count;
            Assert.Equal(this.numberOfSessions, actual);
        }

        [Fact]
        public void Set_adds_new_element()
        {
            var extraSession = new InProcSession(A.Dummy<ISession>(), this.nowUtc, TimeSpan.FromMinutes(15));
            inProcSessionCache.Set(extraSession);

            var expected = this.numberOfSessions + 1;
            var actual = inProcSessionCache.Count;
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Set_does_not_add_when_element_already_is_cached()
        {
            inProcSessionCache.Set(this.activeSession);
            var actual = inProcSessionCache.Count;
            Assert.Equal(this.numberOfSessions, actual);
        }

        [Fact]
        public void Get_gets_element_if_id_is_found()
        {
            var idToFind = activeSession.Id;
            var actual = inProcSessionCache.Get(idToFind);
            Assert.Equal(this.activeSession, actual);
        }

        [Fact]
        public void Get_returns_null_if_id_is_not_found()
        {
            var nonExistingId = Guid.NewGuid();
            var actual = inProcSessionCache.Get(nonExistingId);
            Assert.Null(actual);
        }

        [Fact]
        public void Get_removes_value_if_it_is_expired_and_returns_null()
        {
            var idToFind = expiredSession.Id;
            var actualSession = inProcSessionCache.Get(idToFind);
            Assert.Null(actualSession);

            var expectedCount = this.numberOfSessions - 1;
            var actualCount = inProcSessionCache.Count;
            Assert.Equal(expectedCount, actualCount);
        }

        [Fact]
        public void When_disposed_then_cannot_access_Count()
        {
            inProcSessionCache.Dispose();
            Assert.Throws<ObjectDisposedException>(() => inProcSessionCache.Count);
        }

        [Fact]
        public void When_disposed_then_cannot_access_Get()
        {
            var idToFind = Guid.NewGuid();
            inProcSessionCache.Dispose();
            Assert.Throws<ObjectDisposedException>(() => inProcSessionCache.Get(idToFind));
        }

        [Fact]
        public void When_disposed_then_cannot_access_Set()
        {
            var extraSession = new InProcSession(A.Dummy<ISession>(), this.nowUtc, TimeSpan.FromMinutes(15));
            inProcSessionCache.Dispose();
            Assert.Throws<ObjectDisposedException>(() => inProcSessionCache.Set(extraSession));
        }

        [Fact]
        public void Collection_is_thread_safe()
        {
            const int numberOfThreads = 1000;
            var threadAction = new ThreadStart(() =>
            {
                var extraSession1 = new InProcSession(A.Dummy<ISession>(), this.nowUtc.AddMinutes(-20),
                    TimeSpan.FromMinutes(15));
                var extraSession2 = new InProcSession(A.Dummy<ISession>(), this.nowUtc, TimeSpan.FromMinutes(15));
                inProcSessionCache.Set(extraSession1);
                Thread.Sleep(20);
                inProcSessionCache.Get(extraSession1.Id);
                inProcSessionCache.Set(extraSession2);
                inProcSessionCache.Get(extraSession2.Id);
            });

            var threads = new List<Thread>();
            for (var i = 0; i < numberOfThreads; i++)
            {
                var newThread = new Thread(threadAction);
                newThread.Start();
                threads.Add(newThread);
            }

            foreach (var thread in threads)
            {
                thread.Join();
            }

            var expectedNumberOfSessions = numberOfThreads + this.numberOfSessions;
            var actualNumberOfSessions = inProcSessionCache.Count;

            Assert.Equal(expectedNumberOfSessions, actualNumberOfSessions);
        }

        private void ConfigureSystemClock_ToReturn(DateTime nowUtc)
        {
            A.CallTo(() => this.fakeSystemClock.NowUtc).Returns(nowUtc);
        }
    }
}