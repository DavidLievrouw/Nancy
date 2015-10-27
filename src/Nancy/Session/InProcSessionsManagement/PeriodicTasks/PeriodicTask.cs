namespace Nancy.Session.InProcSessionsManagement.PeriodicTasks
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    internal class PeriodicTask : IPeriodicTask
    {
        private readonly Action action;
        private bool isDisposed;

        public PeriodicTask(Action action)
        {
            if (action == null) {
                throw new ArgumentNullException("action");
            }
            this.action = action;
            this.isDisposed = false;
        }

        public Task Start(TimeSpan initialDelay, TimeSpan interval, CancellationToken cancellationToken)
        {
            this.CheckDisposed();

            Action mainAction = () =>
            {
                const TaskCreationOptions attachedToParent = TaskCreationOptions.AttachedToParent;

                if (this.IsStopRequested(cancellationToken)) {
                    return;
                }

                if (initialDelay > TimeSpan.Zero) {
                    Thread.Sleep(initialDelay);
                }

                while (true) {
                    if (this.IsStopRequested(cancellationToken)) {
                        break;
                    }
                    Task.Factory.StartNew(() =>
                    {
                        if (this.IsStopRequested(cancellationToken)) {
                            return;
                        }
                        this.action();
                    }, cancellationToken, attachedToParent, TaskScheduler.Current);
                    if (this.IsStopRequested(cancellationToken) || interval <= TimeSpan.Zero) {
                        break;
                    }
                    Thread.Sleep(interval);
                }
            };

            return Task.Factory.StartNew(mainAction, cancellationToken);
        }

        public void Dispose()
        {
            this.isDisposed = true;
        }

        private void CheckDisposed()
        {
            if (this.isDisposed) {
                throw new ObjectDisposedException(this.GetType().Name);
            }
        }

        private bool IsStopRequested(CancellationToken cancellationToken)
        {
            return cancellationToken.IsCancellationRequested || this.isDisposed;
        }
    }
}