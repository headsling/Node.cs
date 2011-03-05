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
using node.common.Exceptions;
using Libev;
namespace node.common
{
    /// <summary>
    /// Provides the abilitiy to react to the completion of multiple or actions
    /// note: no exception handling!
    /// </summary>
    public class Complete
    {
        public int WorkPending { get; private set; }

        private readonly IOLoop loop;

        public Complete( ) : this( IOLoop.Instance ) {}
        public Complete( IOLoop loop )
        {
            Preconditions.IsNotNull( loop, "IOLoop has not been defined - can't set a timeout" );
            this.loop = loop;
        }

        private Action whenComplete;
        private Action<bool> timedWhenComplete;
        private TimerWatcher watcher;

        public void AsyncWork( Action work )
        {
            Preconditions.IsTrue( whenComplete == null && timedWhenComplete == null, "Cannot add work once onComplete is called" );
            WorkPending++;

            // oh nothing will go wrong here ..
            work();
        }

        public void AsyncWorkComplete()
        {
            --WorkPending;

            if( WorkPending == 0 ) fireComplete();
        }

        public void OnComplete( Action whenComplete )
        {
            if( WorkPending == 0 ) whenComplete();
            else this.whenComplete = whenComplete;
        }

        /// <summary>
        /// true if all is well, false if timed out
        /// </summary>
        public void OnComplete( Action<bool> whenComplete, TimeSpan timeOut )
        {

            if( WorkPending == 0 ) whenComplete( true );
            else
            {
                timedWhenComplete = whenComplete;

                watcher = new TimerWatcher( timeOut, TimeSpan.MaxValue, loop.EventLoop, ( l, w, et ) =>
                {
                    w.Stop();
                    timedWhenComplete = null;
                    whenComplete( false );
                });
                watcher.Start();
            }
        }

        private void fireComplete()
        {
            if( whenComplete != null ) whenComplete();
            else if( timedWhenComplete != null )
            {
                watcher.Stop();

                timedWhenComplete( true );
            }
        }
    }
}

