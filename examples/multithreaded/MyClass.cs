using System;
using node.common;
using Manos.IO;
using System.Threading;
namespace multithreaded
{
	public class MyClass : INodeProgram
	{
		public int Main( string[] args )
		{
			Thread.CurrentThread.Name = "LoopThread";

			Boundary boundary = new Boundary( );
			
			Thread t = new Thread( () => 
	        {
				
				Console.WriteLine( "{0} thread running", Thread.CurrentThread.Name );
				boundary.ExecuteOnTargetLoop( () => 
	            {
					Console.WriteLine( "Boundary call executed on  {0}", Thread.CurrentThread.Name );
				});
			});
					
			t.IsBackground = true;
			t.Name = "BH Thread";
			
			t.Start();
			
			return 0;
		}
	}
}

