Node.cs
=======

### Evented I/O for C# .net ###

Node.cs takes it's inspriation from node.js, providing an extremely simple mechanism for building high performance single threaded applications. Node.cs is built on the event loop and http stack from Manos De Mono (see https://github.com/jacksonh/manos). Node.cs will also allow for interaction with threads, where interaction with the loop for asynchronous operation is not possible. 

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

to run this you would simply call node against your compiled assembly - 

	% node example.webserver.dll
        listening on 8080

### Change Log ###

02/26/2011

* Initial loop based on manos (https://github.com/jacksonh/manos)
