﻿namespace Nancy.Tests.Unit.Sessions.InProcSessionsManagement.PeriodicTasks
{
    using System;
    using Nancy.Session.InProcSessionsManagement.PeriodicTasks;

    public class TimerForUnitTests : ITimer
    {
        private Action timerAction;
        private TimeSpan timerInterval;

        private bool isStarted;
        private TimeSpan elapsedTime;

        #region ITimer methods

        public void StartTimer(Action action, TimeSpan interval)
        {
            this.isStarted = true;
            this.timerAction = action;
            this.timerInterval = interval;
            this.elapsedTime = new TimeSpan();
        }

        public bool IsStarted()
        {
            return this.isStarted;
        }

        public void StopTimer()
        {
            this.isStarted = false;
        }

        #endregion

        public void ElapseSeconds(double seconds)
        {
            var newElapsedTime = this.elapsedTime + TimeSpan.FromSeconds(seconds);
            if (this.isStarted) {
                var executionCountBefore = this.timerInterval <= TimeSpan.Zero
                    ? 0
                    : this.elapsedTime.Ticks/this.timerInterval.Ticks;
                var executionCountAfter = this.timerInterval <= TimeSpan.Zero
                    ? 0
                    : newElapsedTime.Ticks/this.timerInterval.Ticks;

                for (var i = 0; i < executionCountAfter - executionCountBefore; i++) {
                    this.timerAction();
                }
            }
            this.elapsedTime = newElapsedTime;
        }
    }
}