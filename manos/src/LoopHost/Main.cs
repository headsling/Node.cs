using System;
using Libev;
using Manos.IO;
using Manos;
using System.Threading;

namespace LoopHost
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			Console.WriteLine ("Hello World!");
			
			Thread.CurrentThread.Name = "Cheese";
			
			IOLoop iOLoop = IOLoop.Instance;
		
			Manos.Timeout t = new Manos.Timeout( TimeSpan.FromSeconds( 1 ), RepeatBehavior.Forever, null, ( a, o ) => Console.WriteLine( "Alive " + Thread.CurrentThread.Name ) );
				
			iOLoop.AddTimeout( t );
			
			iOLoop.Start();
			
			
			//Console.ReadLine();
			
		}
	}
}

