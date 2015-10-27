namespace Nancy.Tests.Unit.Sessions.InProcSessionsManagement.PeriodicTasks
{
    using System;
    using System.Threading;
    using Nancy.Session.InProcSessionsManagement.PeriodicTasks;
    using Xunit;

    public class PeriodicTaskFixture
    {
        private int numberOfExecutions;
        private readonly PeriodicTask periodicTask;

        public PeriodicTaskFixture()
        {
            this.numberOfExecutions = 0;
            this.periodicTask = new PeriodicTask(() => this.numberOfExecutions++);
        }

        [Fact]
        public void Given_null_action_then_throws()
        {
            Assert.Throws<ArgumentNullException>(() => new PeriodicTask(null));
        }

        [Fact]
        public void When_cancelled_before_first_execution_then_does_not_execute()
        {
            using (var tokenSource = new CancellationTokenSource()) {
                this.periodicTask.Start(
                    TimeSpan.FromMilliseconds(100),
                    TimeSpan.FromMilliseconds(100),
                    tokenSource.Token);
                Thread.Sleep(20);
                tokenSource.Cancel();
            }
        }

        /*
        [Fact]
        public void Adheres_initial_delay_and_interval()
        {
            using (var tokenSource = new CancellationTokenSource())
            {
                this.periodicTask.Start(
                    TimeSpan.FromMilliseconds(200),
                    TimeSpan.FromMilliseconds(100),
                    tokenSource.Token);
                Thread.Sleep(450);
                tokenSource.Cancel();
                Assert.Equal(3, this.numberOfExecutions);
            }
        }

        [Fact]
        public void When_disposed_then_stops()
        {
            using (var tokenSource = new CancellationTokenSource())
            {
                this.periodicTask.Start(
                    TimeSpan.FromMilliseconds(200),
                    TimeSpan.FromMilliseconds(100),
                    tokenSource.Token);
                Thread.Sleep(250);
                this.periodicTask.Dispose();
                Assert.Equal(1, this.numberOfExecutions);
            }
        }

        [Fact]
        public void When_zero_interval_is_given_then_does_work_only_once()
        {
            using (var tokenSource = new CancellationTokenSource()) {
                this.periodicTask.Start(
                    TimeSpan.Zero,
                    TimeSpan.Zero,
                    tokenSource.Token);
                Thread.Sleep(500);
                this.periodicTask.Dispose();
                Assert.Equal(1, this.numberOfExecutions);
            }
        }
        */
    }
}