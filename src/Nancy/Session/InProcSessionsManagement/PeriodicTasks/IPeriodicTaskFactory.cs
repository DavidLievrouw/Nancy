namespace Nancy.Session.InProcSessionsManagement.PeriodicTasks
{
    using System;

    internal interface IPeriodicTaskFactory
    {
        IPeriodicTask Create(Action action);
    }
}