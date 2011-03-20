using System;
using Manos.IO;
using Libev;
using Manos;
using Manos.Http;
using System.Threading;
using node.common;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Drawing;
using System.Net;

namespace ConsoleTests
{
    class MainClass
    {
        private static IOLoop loop = IOLoop.Instance;

        public static void XZZ( string[] args )
        {
            Thread t = new Thread( () =>
            {
                while( true )
                {
                  Console.WriteLine( "{0}:Calling Thread {1}", DateTime.Now.ToLongTimeString(), Thread.CurrentThread.Name );
                    Thread.Sleep( 75 );
                }
            });
            t.Start();
        }

        public static void Main( string[] args )
        {
            IOLoop loop = IOLoop.Instance;


            // string s = "http://98.139.197.254/5251/5543646565_8992081d9f_b_d.jpg";
            string s = "http://msnbcmedia2.msn.com/j/MSNBC/Components/Photo/_new/g-110320-cvr-libya-12p.grid-8x2.jpg";
            HttpRequest req = new HttpRequest( s );

            MemoryStream ms = new MemoryStream();
            req.OnRawBodyData = ( resp, buffer, offset, len ) =>
            {
                if( buffer == null )
                {
                    if( ((IHttpResponse)resp).StatusCode != 200 )
                    {

                        string ss = Encoding.ASCII.GetString( ms.ToArray() );

                        Console.WriteLine( "failed to get any data " + ss );
                        return;

                    }

                    // we're done
                    ms.Position = 0;

                    ExifExtractor ee = new ExifExtractor( ms );

                    foreach( var kvp in ee.Properties )
                    {
                        Console.WriteLine( "{0}:{1}", kvp.Key, kvp.Value );
                    }

                }
                else
                {
                    ms.Write( buffer.Bytes, offset, len );
                }

            };

            Boundary b = Boundary.Instance;

            req.AddressResolver = ( addr, onComplete ) =>
            {
                ThreadPool.QueueUserWorkItem( state => {
                    try
                    {
                        var addresses = Dns.GetHostAddresses( addr );

                        b.ExecuteOnTargetLoop( () => onComplete( addresses[0] ));
                    }
                    catch( Exception )
                    {
                        onComplete( null );
                    }
                } );
            };

            req.Method = HttpMethod.HTTP_GET;
            req.Execute();

            loop.Start();
        }
        public static void MainXXR2( string[] args )
        {

            IOLoop loop = IOLoop.Instance;

            UDPReceiver rec = new UDPReceiver( loop );
            rec.Listen( "10.0.2.15", 6656 );

            rec.OnRead( ( u, b, c, rep ) => Console.WriteLine( rep + " " + Encoding.ASCII.GetString( b, 0, c )));

            loop.Start();
        }
        public static void bgThread( string[] args )
        {

            IOLoop loop = IOLoop.Instance;
            Thread.CurrentThread.Name = "LoopThread";

            Boundary boundary = new Boundary( );

            Thread t = new Thread( () =>
            {

                Console.WriteLine( "{0} thread running", Thread.CurrentThread.Name );
                boundary.ExecuteOnTargetLoop( () =>
                {
                    Console.WriteLine( "Boundary call executed on  {0}", Thread.CurrentThread.Name );
                    IOLoop.Instance.Stop();
                });
            });


            t.Name = "BH Thread";

            t.Start();

            loop.Start();
        }
        public static void XXMain( string[] args )
        {
            Thread.CurrentThread.Name = "bob";

            Queue<int> q = new Queue<int>();

            TimerWatcher tw = new TimerWatcher( TimeSpan.FromSeconds( 1 ), TimeSpan.FromSeconds( 1 ), loop.EventLoop,
                                               ( l, w, et ) => {

                int count;
                lock( q )
                {
                    count = q.Count;
                }

                Console.WriteLine( "Count {0}",count );
            } );

            tw.Start();

            AsyncWatcher aw = new AsyncWatcher( loop.EventLoop, ( l, w, et ) =>
            {
                int x;
                int count;
                lock( q )
                {
                    x = q.Dequeue();
                    count = q.Count;
                }

                Console.WriteLine( "{0}:Calling Thread {1} (2)", DateTime.Now.ToLongTimeString(), Thread.CurrentThread.Name, count );
            });
                    aw.Start();


            for( int i = 0; i < 8; i++ )
            {
            Thread t = new Thread( () =>
            {
                for( int x = 0; x < 100; x++ )
                {
                    lock( q ) q.Enqueue( x );
                    aw.Send();
                Console.WriteLine( "{0}:BG  Thread {1} (2)", DateTime.Now.ToLongTimeString(), Thread.CurrentThread.Name );

                }
            });
            t.Name = "C" + i;
            t.Start();
            }

            loop.Start();
        }

        public static void TestAsyncCompletion()
        {

            int retCode = 0;

            Complete c = new Complete( );

            Console.WriteLine( DateTime.Now );

            Timers t = Timers.Instance;

            int ii = 0;
            c.AsyncWork( () => t.ScheduleTimer( TimeSpan.FromSeconds( 5 ), h => {ii++; c.AsyncWorkComplete();} ));
            c.AsyncWork( () => t.ScheduleTimer( TimeSpan.FromSeconds( 10 ), h => {ii++;c.AsyncWorkComplete();} ));
            c.AsyncWork( () => t.ScheduleTimer( TimeSpan.FromSeconds( 15 ), h => {ii++;c.AsyncWorkComplete();} ));

            c.OnComplete( (res) => Console.WriteLine( "All done " + ii + " " + res ), TimeSpan.FromSeconds( 7 ));
        }

    }
}

