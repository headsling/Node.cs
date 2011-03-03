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

                t.Response.Write( "<H1>Hello World!</H1>" );
                t.Response.End();


            }, IOLoop.Instance ).Listen( "10.0.2.15", 8080 );


            Console.WriteLine( "listening on 8080" );

            return 0;
        }
    }
}

