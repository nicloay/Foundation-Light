using System;

namespace Foundation.Architecture
{
    /// <summary>
    /// A platform agnostic timer with callback helper
    /// </summary>
    public class RoutineTimer : IDisposable
    {
        public TimeSpan Interval { get; private set; }
        public Action Callback { get; private set; }
        public bool IsRunning { get; private set; }

        private IRoutineService service;
        private IDisposable routine;
        private double delta;

        public RoutineTimer(TimeSpan interval, Action callback, IRoutineService service)
        {
            Callback = callback;
            Interval = interval;
            this.service = service;
        }

        public void Dispose()
        {
            Stop();
            Callback = null;
        }

        public RoutineTimer Stop()
        {
            if (routine != null)
            {
                routine.Dispose();
                routine = null;
            }

            IsRunning = false;

            return this;
        }

        public RoutineTimer Start()
        {
            if (!IsRunning)
            {
                IsRunning = true;

                routine = service.RunLoop(Handler);
            }

            return this;
        }

        void Handler(double d)
        {
            if(!IsRunning)
                return;

            delta += d;

            if (delta >= Interval.TotalMilliseconds)
            {
                delta = 0;
                Callback();
            }
        }
    }
}
