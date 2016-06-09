using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;

namespace ParallelExtensionExtrasTest.CoordinationDataStructures
{
    [TestClass]
    public class ActionCountdownEventTest
    {
        [TestMethod]
        public void AddCountTest()
        {
            Action action = ()=>
            {
                for (int i=0; i < 10;i++)
                {
                    System.Diagnostics.Debug.WriteLine(i);
                }
            };
            ActionCountdownEvent ace = new ActionCountdownEvent(5, action);
            ace.AddCount();
            int k = 0;
            while (true)
            {
                System.Diagnostics.Debug.WriteLine("----------------------------");
                k++;
                if (k > 10)
                {
                    ace.Signal();
                }
            }

        }
    }
}
