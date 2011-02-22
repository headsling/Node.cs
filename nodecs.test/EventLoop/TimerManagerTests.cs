using System;
using NUnit.Framework;
using NodeCS.EventLoop;
namespace EventLoop
{
    [TestFixture]
    public class TimerManagerTests
    {
        [Test]
        public void TestIt()
        {
            TimerManager timerManager = new TimerManager();

            DateTime date = DateTime.Now;
            Console.WriteLine( date.ToLongTimeString() );

            timerManager.ScheduleTimer( DateTime.Now + TimeSpan.FromSeconds( 2 ), () => Assert.AreEqual( TimeSpan.FromSeconds( 2 ), DateTime.Now - date   ));
            int e = timerManager.ScheduleTimer( DateTime.Now + TimeSpan.FromSeconds( 3 ), () => {throw new Exception("shouldn't have done that" );} );
            timerManager.ScheduleTimer( DateTime.Now + TimeSpan.FromSeconds( 8 ), () => Assert.AreEqual( TimeSpan.FromSeconds( 8 ), DateTime.Now - date   ));

            timerManager.CancelTimer( e );

            while( timerManager.FireTimers() != DateTime.MinValue ) {}

        }
    }
}

