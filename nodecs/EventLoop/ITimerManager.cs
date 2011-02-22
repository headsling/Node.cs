using System;
namespace NodeCS.EventLoop
{
    public delegate void OnTimerHandler();

    public interface ITimerManager
    {
        int ScheduleTimer( DateTime dueTime, OnTimerHandler onTimer );
        void CancelTimer( int timerHandle );

        /// <summary>
        /// returns the due time of the next timer in the heap 
        /// </summary>
        /// <returns>
        /// A <see cref="DateTime"/>
        /// </returns>
        DateTime FireTimers( );
    }
}

