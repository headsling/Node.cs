using System;
using System.Collections.Generic;
using System.Diagnostics;
namespace NodeCS.EventLoop
{
    public class TimerManager : ITimerManager
    {
        private readonly Heap<OnTimerHandler,long> heap;

        public TimerManager( int initialCapacity = 16 )
        {
            heap = new Heap<OnTimerHandler,long>( initialCapacity, ( x, y ) => y.CompareTo( x ));
        }

        public int ScheduleTimer( DateTime dueTime, OnTimerHandler onTimer )
        {
            return heap.Push( onTimer, dueTime.Ticks );
        }

        public DateTime FireTimers()
        {
            long now = DateTime.Now.Ticks;

            long nextTicks = DateTime.MinValue.Ticks;

            while( heap.Count > 0 &&
                  ( nextTicks = heap.PeekPriority() ) <= now )
            {
                OnTimerHandler handler = heap.Pop();

                if( handler == null ) continue;

                try
                {
                    handler();
                }
                catch( Exception ex )
                {
                    Trace.WriteLine( ex );
                }
            }

            return new DateTime( nextTicks );
        }

        public void CancelTimer( int timerHandle )
        {
            heap.ClearItem( timerHandle );
        }
    }


}

