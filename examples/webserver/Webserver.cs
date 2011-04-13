using System;
using 
Manos.Http;
using Manos.IO;
using node.common;
using System.Diagnostics;
using Libev;

namespace webserver
{
    public class Webserver : INodeProgram
    {
        public int Main( string[] args )
        {
            new HttpServer( ( IHttpTransaction t ) =>
            {
                
				string addr = t.Request.Socket.Address;
                Console.WriteLine( "got connection from {0}  asking for  {1}", addr, t.Request.Path );
				;
                t.Response.Write( "<H1>Hello World!</H1>" );
                t.Response.End();
                 
            }, IOLoop.Instance.CreateSocketStream(), true ).Listen( "127.0.0.1", 8080 );

            Console.WriteLine( "listening on 8080" );

            return 0;
        }
    }
}

