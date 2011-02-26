using System;
using node.common;
using Libev;
using Manos.IO;
namespace timer
{
    public class MyClass : INodeProgram
    {
        public int Main( string[] args )
        {

            TimerWatcher tw = new TimerWatcher( TimeSpan.FromSeconds( 1 ), TimeSpan.FromSeconds( 1 ), IOLoop.Instance.EventLoop,
                                               ( l, w, et ) => Console.WriteLine( "{0}: Beep", DateTime.Now ));
            tw.Start();


            return 0;

        }
    }
}

