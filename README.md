Node.cs
=======

### Evented I/O for C# .net ###

Node.cs takes its inspriation from node.js, providing an extremely simple mechanism for building high performance single threaded applications. Node.cs is built on the event loop and http stack from Manos De Mono (see https://github.com/jacksonh/manos). Node.cs will also allow for interaction with threads, where interaction with the loop for asynchronous operation is not possible. 

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

### Timers ###

Node.cs defines a simple mechanism to have an action take place after some
specified time in the future

	var timers = Timers.Instance;
	int handle = timers.ScheduleTimer( TimeSpan.FromSeconds( 5 ),
   			     Console.WriteLine( "hello in the future" );
                      
Scheduling a timer returns a handle to that timer that can be used to cancel it
before it fires

	timers.CancelTimer( handle );

### Task Completion ###

A common issue with frameworks such as this are the deep, deep nests that get
created as a result of chaining multiple indipendant async calls within calls.
Another common issue is the ability to know when a loop of async calls have completed so that further action can be taken. 

Node.cs defines a class Complete that allows you to indicate when an async
method is executing and when that method has completed.

	Complete c = new Complete();
	Timers t = Timers.Instance;
  
	var ts = TimeSpan.FromSeconds( 5 );

	int ii = 0;

	c.AsyncWork( () => t.ScheduleTimer( ts, h => {ii++; c.AsyncWorkComplete();} ));
	c.AsyncWork( () => t.ScheduleTimer( ts, h => {ii++;c.AsyncWorkComplete();} ));

	c.OnComplete( () => Console.WriteLine( "Completed " + ii));

A timeout can also be specified to call you back if the async actions have not 
completed in the specified time :

	c.OnComplete( (rec ) => Console.WriteLine( "Completed " + ii + res ),TimeSpan.FromSeconds( 2 ));
			

### Threading ###

Even though Node.cs is aimed at single threaded use cases, there might be times
where you'll want to kick off another thread for async functionality that is 
not yet provided by the framework.

Node.cs defines a class Boundary that aims to make interacting with your main
node app from other threads simple.

	Thread.CurrentThread.Name = "LoopThread";
	var boundary = Boundary.Instance;
	Thread t = new Thread( () => 
	{
		Console.WriteLine( "{0} thread running", Thread.CurrentThread.Name );
		boundary.ExecuteOnTargetLoop( () => 
		{
			Console.WriteLine( "Boundary call executed on  {0}", 
					Thread.CurrentThread.Name );
		});
	});
	t.Name = "BH Thread";
	t.Start();

	% node example.multithreaded.dll
	Starting program
	BH Thread thread running
	Boundary call executed on  LoopThread

### UDP ###

UDP Message Receiving is supported, sending support will be added shortly.
	
	UDPReceiver rec = new UDPReceiver( loop );
	rec.Listen( "10.0.2.15", 6656 );
	rec.OnRead( ( u, b, c, rep ) => Console.WriteLine( rep + " " + Encoding.ASCII.GetString( b, 0, c )));

