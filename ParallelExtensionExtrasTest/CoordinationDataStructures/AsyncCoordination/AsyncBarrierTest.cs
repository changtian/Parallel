using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Async;

namespace ParallelExtensionExtrasTest.CoordinationDataStructures.AsyncCoordination
{
    [TestClass]
    public class AsyncBarrierTest
    {
        [TestMethod]
        public void SignalAndWaitTest()
        {
            AsyncBarrier barrier = new AsyncBarrier(3);
            barrier.SignalAndWait();
        }
    }
}
