using System;
using Manos.IO;
using System.Reflection;
using System.IO;
using node.common;
using Manos;
namespace node
{
	public class Program
	{
		public static int Main( string[] args )
		{
			if( args.Length == 0 )
            {
                Console.Error.WriteLine( "node yourdll.dll [args[]]" );
                return -1;
            }

            string dll = args[0];

            if( ! File.Exists( dll ))
            {
                Console.Error.WriteLine( "dll '{0}' not found", dll );
                return -1;
            }

            Assembly asy;
            try
            {
                asy = Assembly.LoadFile( args[0] );
            }
            catch( Exception ex )
            {
                Console.Error.WriteLine( "failed to load dll '{0}'. Exception was {1}", dll, ex.Message );
                return -1;
            }

            Type[] types = asy.GetTypes();

            INodeProgram program = null;

            foreach( var type in types )
            {
                if( typeof(INodeProgram).IsAssignableFrom( type ) )
                {
                    try
                    {
                        program = (INodeProgram)asy.CreateInstance( type.FullName );

                        break;
                    }
                    catch( Exception ex )
                    {
                        Console.Error.WriteLine( "failed to type '{0}'. Exception was {1}", type.FullName, ex.Message );
                        return -1;
                    }
                }
            }

            if( program == null )
            {
                Console.Error.WriteLine( "Assembly '{0}' doesn't appear to have a INodeProgram type", asy.FullName );
                return -1;
            }

            string[] progArgs = new string[args.Length -1];

            for( int i = 1; i < args.Length; i++ ) progArgs[i] = args[i];

            IOLoop loop = IOLoop.Instance;

            int retCode = 0;

            Manos.Timeout t = new Manos.Timeout( TimeSpan.FromMilliseconds( 1 ), RepeatBehavior.Single, null,
                                                ( a, o ) =>
            {
                try
                {
                    retCode = program.Main( progArgs );
                }
                catch( Exception ex )
                {
                    Console.Error.WriteLine( "NodeProgram failed on startup {0}", ex );

                    loop.Stop();
                    retCode = -1;
                }

                if( retCode != 0 ) loop.Stop();
            } );

            loop.AddTimeout( t );

            loop.Start();

            return retCode;
		}
	}
}

