using System;

namespace Nancy.Tests.Unit.Sessions.InProcSessionsManagement.PeriodicTasks
{
    using Nancy.Session.InProcSessionsManagement.PeriodicTasks;
    using Xunit;

    public class PeriodicTaskFactoryFixture
    {
        private PeriodicTaskFactory periodicTaskFactory;

        public PeriodicTaskFactoryFixture()
        {
            this.periodicTaskFactory = new PeriodicTaskFactory();
        }

        [Fact]
        public void Given_null_action_then_throws()
        {
            Assert.Throws<ArgumentNullException>(() => this.periodicTaskFactory.Create(null));
        }

        [Fact]
        public void Given_valid_action_then_returns_new_periodic_task()
        {
            Action action = () => Console.WriteLine("ok");
            var actual = this.periodicTaskFactory.Create(action);

            Assert.NotNull(actual);
            Assert.IsAssignableFrom<IPeriodicTask>(actual);
        }
    }
}