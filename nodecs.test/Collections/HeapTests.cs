using System;
using NUnit.Framework;
using System.Diagnostics;
using System.Collections.Generic;

namespace NodeCS.test
{
	[TestFixture]
	public class HeapTests
	{
        [Test]
        public void TestDecendingSort()
        {
            Random r = new Random();

            Heap<int,int> heap = new Heap<int, int>( 10000, ( x, y ) => y.CompareTo( x ) );

            Stopwatch sw = new Stopwatch();

            sw.Start();

            SortedList<int,int> list = new SortedList<int,int>();

            for( int i = 0; i < 10000; i++ )
            {
                int j = r.Next(  );
                list.Add( j, j );
                heap.Push( j, j );
            }

            Assert.IsTrue( heap.Verify() );

            int index = 0;
            while( heap.Count > 0 )
            {
                var x = heap.Pop();
                var y = list.Values[index++];

                Assert.AreEqual( x, y );
            }


            Assert.IsTrue( heap.Verify() );

            sw.Stop();

            Console.WriteLine( "Time for 10000 " + sw.ElapsedMilliseconds );


        }
		[Test]
		public void TestAscendingSort()
		{

            Random r = new Random();

            Heap<int,int> heap = new Heap<int, int>();

            Stopwatch sw = new Stopwatch();

            sw.Start();

            SortedList<int,int> list = new SortedList<int,int>();

            for( int i = 0; i < 10000; i++ )
            {
                int j = r.Next(  );
                list.Add( j, j );
                heap.Push( j, j );
            }

            Assert.IsTrue( heap.Verify() );

            while( heap.Count > 0 )
            {
                var x = heap.Pop();
                var y = list.Values[heap.Count];

                Assert.AreEqual( x, y );
            }


            Assert.IsTrue( heap.Verify() );

            sw.Stop();

            Console.WriteLine( "Time for 10000 " + sw.ElapsedMilliseconds );


		}
	}
}

