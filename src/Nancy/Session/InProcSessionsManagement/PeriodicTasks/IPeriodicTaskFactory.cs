using System;

namespace Nancy.Session.InProcSessionsManagement.PeriodicTasks
{
    internal interface IPeriodicTaskFactory
    {
        IPeriodicTask Create(Action action);
    }
}