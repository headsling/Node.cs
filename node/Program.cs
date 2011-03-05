//
//  Copyright (C) 2011 Robin Duerden (rduerden@gmail.com)
// 
//  Permission is hereby granted, free of charge, to any person obtaining
//  a copy of this software and associated documentation files (the
//  "Software"), to deal in the Software without restriction, including
//  without limitation the rights to use, copy, modify, merge, publish,
//  distribute, sublicense, and/or sell copies of the Software, and to
//  permit persons to whom the Software is furnished to do so, subject to
//  the following conditions:
// 
//  The above copyright notice and this permission notice shall be
//  included in all copies or substantial portions of the Software.
// 
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
//  EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
//  MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
//  NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
//  LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
//  OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
//  WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// 
// 
// 


using System;
using Manos.IO;
using System.Reflection;
using System.IO;
using node.common;
using Manos;
using Libev;
namespace node
{
	public class Program
	{
		public static int Main( string[] args )
		{
            GlobalErrorHandler.Instance.SetGlobalErrorHandler( ( o, e ) =>
            {
                Console.WriteLine( e.ExceptionObject );
                IOLoop.Instance.Stop();
                Environment.Exit( -1 );
            });

			if( args.Length == 0 )
            {
                Console.Error.WriteLine( "node yourdll.dll [args[]]" );
                return -1;
            }

            string dll = args[0];
            if( ! dll.EndsWith( ".dll" )) dll += ".dll";

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

            Manos.Timeout t = new Manos.Timeout( TimeSpan.FromSeconds( 1 ), RepeatBehavior.Single, null,
                                                ( a, o ) =>
            {
                Console.WriteLine( "Starting program" );

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

