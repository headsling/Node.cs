using System;
using NUnit.Framework;
using System.Diagnostics;
using NodeCS.Exceptions;
namespace NodeCS.test
{
	[TestFixture]
	public class PreconditionsTests
	{
		[Test]
		public void TestIt()
		{
			Trace.WriteLine("starting");
				
			Assert.Throws<Exception>( () => Preconditions.IsTrue( false, "Test Failed" ) );
			Assert.DoesNotThrow( () => Preconditions.IsTrue( true, "Test Failed" ) );

			Assert.Throws<ApplicationException>( () => Preconditions.IsTrue<ApplicationException>( false, "Test Failed" ) );
		}
	}
}

