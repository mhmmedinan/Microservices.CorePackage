using Core.Abstractions.Scheduler;
using Hangfire;

namespace Core.Scheduling.Hangfire.Scheduler;

public interface IHangfireScheduler:IScheduler
{
    string Enqueue<T>(
        T command,
        string parentJobId,
        JobContinuationOptions continuationOption,
        string? description = null)
        where T : IInternalCommand;

    string Enqueue(
        ScheduleSerializedObject scheduleSerializedObject,
        string parentJobId,
        JobContinuationOptions continuationOption,
        string? description = null);
}
