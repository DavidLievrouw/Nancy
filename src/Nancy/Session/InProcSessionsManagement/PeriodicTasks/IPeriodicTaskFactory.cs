using System;

namespace Nancy.Session.InProcSessionsManagement.PeriodicTasks
{
    public interface IPeriodicTaskFactory
    {
        IPeriodicTask Create(Action action);
    }
}