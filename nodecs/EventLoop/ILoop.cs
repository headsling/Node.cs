using System;

namespace NodeCS.EventLoop
{
    public interface ILoop
    {
        void Start();
        void Stop();

        ITimerManager TimerManager { get; }
    }
}

