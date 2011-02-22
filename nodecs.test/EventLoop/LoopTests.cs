using System;
using NUnit.Framework;
using System.Runtime.CompilerServices;
using System.Net.Sockets;
using System.Net;
namespace EventLoop
{
    [TestFixtureAttribute]
    public class LoopTests
    {
        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        private extern static void Select_internal (ref Socket [] sockets,
                            int microSeconds,
                            out int error);

        [Test]
        public void TestIt()
        {

            Console.WriteLine(
                              "dfsdf" );

            Socket[] sockets = new Socket[1];

            Socket s = new Socket( AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp );
            s.Blocking = false;

            s.Bind( new IPEndPoint( IPAddress.Parse( "127.0.0.1" ), 5445 ));

            s.Listen( 10 );

            sockets[0] = s;

            int error;
            Select_internal( ref sockets, (int) (TimeSpan.FromSeconds( 2 ).Ticks / 100), out error );

            Console.WriteLine( error );
            if( sockets == null ) return;

            Console.WriteLine( sockets[0] == null );

        }
    }
}

