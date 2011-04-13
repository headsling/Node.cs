// 
//  Copyright (C) 2011 Robin Duerden (rduerden@gmail.com)
// 
//  Permission is hereby granted, free of charge, to any person obtaining
//  a copy of this software and associated documentation files (the
//  "Software"), to deal in the Software without restriction, including
//  without limitation the rights to use, copy, modify, merge, publish,
//  distribute, sublicense, and/or sell copies of the Software, and to
//  permit persons to whom the Software is furnished to do so, subject to
//  the following conditions:
// 
//  The above copyright notice and this permission notice shall be
//  included in all copies or substantial portions of the Software.
// 
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
//  EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
//  MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
//  NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
//  LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
//  OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
//  WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// 
// 
// 
using System;
using Manos.IO;
using System.Collections.Generic;
using Libev;
using node.common.Exceptions;
namespace node.common
{
    public class Timers
    {
        public static readonly Timers Instance = new Timers( IOLoop.Instance );

        private readonly IOLoop loop;
        private TimerWatcher[] watchers;
        private readonly Queue<int> freeList;

        public Timers( Manos.IO.IOLoop loop, int initialSize = 2 )
        {
            this.loop = loop;
            freeList = new Queue<int>( initialSize );

            growWatchers( initialSize );
        }

        private void growWatchers( int newSize )
        {
            int currentSize;
            if( watchers == null )
            {
                watchers = new TimerWatcher[newSize];
                currentSize = 0;
            }
            else
            {
                currentSize = watchers.Length;
                var w = new TimerWatcher[newSize];
                Array.Copy( watchers, w, currentSize - 1 );
                watchers = w;
            }

            for( int i = currentSize; i < newSize; i++ ) freeList.Enqueue( i );

        }

        public int ScheduleTimer( TimeSpan after, Action<int> onTimer )
        {
            if( freeList.Count == 0 ) growWatchers( watchers.Length * 2 );

            int ret = freeList.Dequeue();

            TimerWatcher tw = new TimerWatcher( after, TimeSpan.MaxValue, (LibEvLoop)loop.EventLoop, ( l, w, et ) =>
            {
                w.Stop();
                watchers[ret] = null;
                freeList.Enqueue( ret );
                onTimer( ret );
            } );
            watchers[ret] = tw;
            tw.Start();
            return ret;
        }

        public int ScheduleRepeatingTimer( TimeSpan interval, Action<int> onTimer )
        {
            if( freeList.Count == 0 ) growWatchers( watchers.Length * 2 );

            int ret = freeList.Dequeue();

            TimerWatcher tw = new TimerWatcher( interval, interval, (LibEvLoop)loop.EventLoop, ( l, w, et ) => onTimer( ret ));

            watchers[ret] = tw;
            tw.Start();
            return ret;
        }

        public void CancelTimer( int handle )
        {
            Preconditions.IsNotNull( watchers[handle], "timer not registered" );

            watchers[handle].Stop();
            watchers[handle] = null;
            freeList.Enqueue( handle );
        }
    }
}

