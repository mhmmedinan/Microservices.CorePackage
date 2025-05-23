﻿namespace Core.Resiliency.Retry;

[AttributeUsage(AttributeTargets.Class)]
public class RetryPolicyAttribute : Attribute
{
    private int _retryCount = 3;
    private int _sleepDuration = 200;

    /// <summary>
    ///     Gets or sets the amount of times to retry the execution.
    ///     Each retry the sleep duration is incremented by <see cref="SleepDuration" />.
    ///     Defaults to 3 times.
    /// </summary>
    public int RetryCount
    {
        get => _retryCount;
        set
        {
            if (value < 1) throw new ArgumentException("Retry count must be higher than 1.", nameof(value));

            _retryCount = value;
        }
    }

    /// <summary>
    ///     Gets or sets the sleep duration in milliseconds.
    ///     Each retry the sleep duration gets incremented by this value.
    ///     Defaults to 200 milliseconds.
    /// </summary>
    public int SleepDuration
    {
        get => _sleepDuration;
        set
        {
            if (value < 1) throw new ArgumentException("Sleep duration must be higher than 1ms.", nameof(value));

            _sleepDuration = value;
        }
    }
}
