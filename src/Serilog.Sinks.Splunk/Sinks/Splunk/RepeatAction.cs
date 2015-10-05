using System;
using System.Threading;
using System.Threading.Tasks;

namespace Serilog.Sinks.Splunk
{
    internal static class RepeatAction
    {
        public static Task OnInterval(TimeSpan pollInterval, Action action, CancellationToken token,
            TaskCreationOptions taskCreationOptions, TaskScheduler taskScheduler)
        {
            return Task.Factory.StartNew(() =>
            {
                for (;;)
                {
                    if (token.WaitCancellationRequested(pollInterval))
                        break;
                    action();
                }
            }, token, taskCreationOptions, taskScheduler);
        }

        public static Task OnInterval(TimeSpan pollInterval, Action action, CancellationToken token)
        {
            return OnInterval(pollInterval, action, token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

        public static bool WaitCancellationRequested(this CancellationToken token, TimeSpan timeout)
        {
            return token.WaitHandle.WaitOne(timeout);
        }
    }
}