namespace Nancy.Tests.Unit.Sessions.Cache
{
    using System;
    using FakeItEasy;
    using Nancy.Session;
    using Nancy.Session.Cache;
    using Xunit;

    public class InProcSessionFixture
    {
        private readonly ISystemClock fakeSystemClock;
        private readonly ISession wrappedSession;

        public InProcSessionFixture()
        {
            this.wrappedSession = A.Fake<ISession>();
            this.fakeSystemClock = A.Fake<ISystemClock>();
        }

        [Fact]
        public void Given_null_wrapped_session_fails()
        {
            Assert.Throws<ArgumentNullException>(() => new InProcSession(null, DateTime.Now, TimeSpan.FromMinutes(15)));
        }

        [Fact]
        public void Equals_other_session_with_same_id()
        {
            var sessionId = Guid.NewGuid();
            var inProcSession1 = new InProcSession(sessionId, this.wrappedSession, DateTime.Now, TimeSpan.FromSeconds(3));
            var inProcSession2 = new InProcSession(sessionId, this.wrappedSession, DateTime.Now, TimeSpan.FromSeconds(3));

            var actual = inProcSession1.Equals(inProcSession2);

            Assert.True(actual);
        }

        [Fact]
        public void When_expired_isexpired_returns_true()
        {
            SystemClockAmbientContext.Current = this.fakeSystemClock;
            var creationTime = new DateTime(2015, 10, 20, 21, 19, 0, DateTimeKind.Utc);
            var timeout = TimeSpan.FromMinutes(10);
            this.ConfigureSystemClock_ToReturn(creationTime.AddMinutes(11));
            var inProcSession = new InProcSession(this.wrappedSession, creationTime, timeout);

            var actual = inProcSession.IsExpired(SystemClockAmbientContext.Current.NowUtc);

            Assert.True(actual);
        }

        [Fact]
        public void When_not_expired_isexpired_returns_false()
        {
            SystemClockAmbientContext.Current = this.fakeSystemClock;
            var creationTime = new DateTime(2015, 10, 20, 21, 19, 0, DateTimeKind.Utc);
            var timeout = TimeSpan.FromMinutes(10);
            this.ConfigureSystemClock_ToReturn(creationTime.AddMinutes(2));
            var inProcSession = new InProcSession(this.wrappedSession, creationTime, timeout);

            var actual = inProcSession.IsExpired(SystemClockAmbientContext.Current.NowUtc);

            Assert.False(actual);
        }

        private void ConfigureSystemClock_ToReturn(DateTime nowUtc)
        {
            A.CallTo(() => this.fakeSystemClock.NowUtc).Returns(nowUtc);
        }
    }
}