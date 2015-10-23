﻿namespace Nancy.Tests.Unit.Sessions.InProcSessionsManagement.PeriodicTasks
{
    using System.Threading;
    using Nancy.Session.InProcSessionsManagement.PeriodicTasks;
    using Xunit;

    public class CancellationTokenSourceFactoryFixture
    {
        private readonly CancellationTokenSourceFactory cancellationTokenSourceFactory;

        public CancellationTokenSourceFactoryFixture()
        {
            this.cancellationTokenSourceFactory = new CancellationTokenSourceFactory();
        }

        [Fact]
        public void Create_returns_new_cancellation_token_source()
        {
            var actual = this.cancellationTokenSourceFactory.Create();
            Assert.NotNull(actual);
            Assert.IsAssignableFrom<CancellationTokenSource>(actual);
        }
    }
}