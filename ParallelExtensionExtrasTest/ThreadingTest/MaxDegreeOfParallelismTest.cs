using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ParallelExtensionExtrasTest.ThreadingTest
{
    [TestClass]
    public class MaxDegreeOfParallelismTest
    {
        [TestMethod]
        public void TestParallelism()
        {
            var locker = new Object();
            int count = 0;
            Parallel.For
                (0
                 , 1000
                 , new ParallelOptions { MaxDegreeOfParallelism = 8 }
                 , (i) =>
                 {
                     Interlocked.Increment(ref count);
                     lock (locker)
                     {
                         System.Diagnostics.Debug.WriteLine("Number of active threads:" + count);
                         Thread.Sleep(10);
                     }
                     Interlocked.Decrement(ref count);
                 }
                );
        }
    }
}
