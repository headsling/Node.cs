using System;
using Manos.Http;
using Manos.IO;
using node.common;
using System.Diagnostics;
namespace webserver
{
    public class Webserver : INodeProgram
    {
        public int Main( string[] args )
        {
            new HttpServer( ( IHttpTransaction t ) =>
            {
                Console.WriteLine( "got connection {0}", t.Request.Path );

                t.Response.End("<H1>Hello World!</H1>");


            }, IOLoop.Instance ).Listen( "127.0.0.1", 8080 );


            Console.WriteLine( "listening on 8080" );

            return 0;
        }
    }
}

