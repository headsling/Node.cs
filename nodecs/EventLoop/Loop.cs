using System;
using System.Threading;
using NodeCS.Exceptions;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Collections.Generic;

namespace NodeCS.EventLoop
{
    public class Loop : ILoop
    {
        private readonly ITimerManager timerManager;
        private Thread thread;
        private bool running;

        private readonly List<Socket> reads;
        private readonly List<Socket> writes;
        private readonly List<Socket> errors;

        private readonly Dictionary<Socket,ISocketHandler> socketsToHandlers;

        public Loop( ITimerManager timerManager )
        {
            Preconditions.IsNotNull<ArgumentNullException>( timerManager, "timerManager" );
            this.timerManager = timerManager;

            reads = new List<Socket>();
            writes = new List<Socket>();
            errors = new List<Socket>();

            socketsToHandlers = new Dictionary<Socket, ISocketHandler>();

        }

        public ITimerManager TimerManager { get { return timerManager; }}

        public void Start()
        {
            Preconditions.IsNull( thread, "Thread is running" );
            Preconditions.IsFalse( running, "Thread is running" );

            thread = new Thread( run );
            running = true;

            thread.Start();
        }

        public void RegisterRead( Socket socket )
        {
            Preconditions.IsFalse( reads.Contains( socket ), "Socket already registered" );

            reads.Add( socket );

            if( ! errors.Contains( socket )) errors.Add( socket );
        }

        public void RegsiterWriteable( Socket socket )
        {
        }

        private void run()
        {
            while( running )
            {
            }
        }

        public void Stop()
        {
            Preconditions.IsNotNull( thread, "Thread not running" );

            running = false;

            if( Thread.CurrentThread != thread ) return;

            thread.Join( TimeSpan.FromSeconds( 2 ));

            if( thread.IsAlive ) thread.Abort();

            thread = null;
        }
    }
}

