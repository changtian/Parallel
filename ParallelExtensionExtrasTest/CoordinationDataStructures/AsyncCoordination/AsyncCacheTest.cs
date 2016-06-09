using System;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace ParallelExtensionExtrasTest.CoordinationDataStructures.AsyncCoordination
{
    [TestClass]
    public class AsyncCacheTest
    {
        [TestMethod]
        public void GetValueTest()
        {
            AsyncCache<string, string> ac = new AsyncCache<string, string>((name) =>
            {
                return new System.Threading.Tasks.Task<string>(() => { return 
                    "current name is :" +name  + Thread.CurrentThread.Name; });
            });

            ac.SetValue("KeyA", "ValueA");

            var valueA = ac.GetValue("KeyA");
            string actualResult = valueA.Result;

            for (int i = 0; i < 10; i++)
            {
                TaskFactory tf = new TaskFactory();
                tf.StartNew(() => 
                {
                    ac.SetValue("KeyA", "ValueA" + i);
                });
            }
        }
    }
}
